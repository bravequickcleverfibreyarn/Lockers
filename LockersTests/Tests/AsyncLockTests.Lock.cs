using Lockers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LockersTests.Tests;

[TestClass]
sealed public class AsyncLockTests_Lock
{
  [TestMethod]
  async public Task CancellationRequested__TaskIsCancelled ()
  {

    using AsyncLock asyncLock           = new ();
    using CancellationTokenSource cts   = new ();

    TaskScheduler scheduler = TaskScheduler.Current;
    _ = await asyncLock.Lock (CancellationToken.None, Timeout.InfiniteTimeSpan, scheduler);

    Task<bool> test = asyncLock.Lock(cts.Token, Timeout.InfiniteTimeSpan, scheduler);

    cts.Cancel ();

    AggregateException aggregateException = Assert.ThrowsException<AggregateException> (() => test.Wait ());
    Assert.AreEqual (1, aggregateException.InnerExceptions.Count);
    Assert.AreEqual (typeof (TaskCanceledException), aggregateException.InnerExceptions [0]!.GetType ());
  }

  [TestMethod]
  async public Task Locked__CannotTakeLock ()
  {
    using AsyncLock asyncLock   = new ();
    TaskScheduler scheduler     = TaskScheduler.Current;

    _ = await asyncLock.Lock (CancellationToken.None, Timeout.InfiniteTimeSpan, scheduler);

    Assert.IsFalse
    (
      await asyncLock.Lock (CancellationToken.None, TimeSpan.Zero, scheduler)
    );
  }

  [TestMethod]
  async public Task TryLock__LockTaken ()
  {

    using AsyncLock asyncLock   = new ();

    Assert.IsTrue
    (
      await asyncLock.Lock (CancellationToken.None, TimeSpan.Zero, TaskScheduler.Current)
    );
  }

  [TestMethod]
  async public Task WaitWithTimeout__TimeoutExpires ()
  {

    using AsyncLock asyncLock   = new ();

    TaskScheduler scheduler     = TaskScheduler.Current;
    _ = await asyncLock.Lock (CancellationToken.None, Timeout.InfiniteTimeSpan, scheduler);

    const uint timeoutSeconds = 3;
    var sw = Stopwatch.StartNew();

    bool taken = await asyncLock.Lock
    (
      CancellationToken.None,
      TimeSpan.FromSeconds(timeoutSeconds),
      scheduler
    );

    sw.Stop ();

    double elapsedSeconds = sw.Elapsed.TotalSeconds;

    Assert.IsTrue (timeoutSeconds <= elapsedSeconds);
    Assert.IsTrue (1 > elapsedSeconds - timeoutSeconds);
    Assert.IsFalse (taken);
  }

  [TestMethod]
  public void PassNullTaskScheduler__ThrowsArgumentNullException ()
  {
    using AsyncLock asyncLock   = new ();

    AggregateException aggregateException  = Assert.ThrowsException<AggregateException>
    (
      () => asyncLock.Lock (CancellationToken.None, Timeout.InfiniteTimeSpan, null!).Wait ()
    );

    Assert.AreEqual (1, aggregateException.InnerExceptions.Count);
    Assert.AreEqual (typeof (ArgumentNullException), aggregateException.InnerExceptions [0]!.GetType ());
  }
}
