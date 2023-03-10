using Microsoft.VisualStudio.TestTools.UnitTesting;

using Software9119.Lockers;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LockersTests.Tests;

[TestClass]
sealed public class AsyncLockTests_AutoLock
{
  [TestMethod]
  async public Task CancellationRequested__TaskIsCancelled ()
  {

    using AsyncLock asyncLock           = new ();
    using CancellationTokenSource cts   = new ();

    TaskScheduler scheduler = TaskScheduler.Current;
    _ = await asyncLock.AutoLock (CancellationToken.None, Timeout.InfiniteTimeSpan, scheduler);

    Task<Unlocker> test = asyncLock.AutoLock(cts.Token, Timeout.InfiniteTimeSpan, scheduler);

    cts.Cancel ();

    AggregateException aggregateException = Assert.ThrowsException<AggregateException> (() => test.Wait ());
    
    ReadOnlyCollection<Exception> innerExceptions = aggregateException.InnerExceptions;
    Assert.AreEqual (1, innerExceptions.Count);
    Assert.AreEqual (typeof (TaskCanceledException), innerExceptions [0]!.GetType ());
  }

  [TestMethod]
  async public Task Locked__CannotTakeLock ()
  {
    using AsyncLock asyncLock   = new ();
    TaskScheduler scheduler     = TaskScheduler.Current;

    _ = await asyncLock.AutoLock (CancellationToken.None, Timeout.InfiniteTimeSpan, scheduler);

    Unlocker unlocker = await asyncLock.AutoLock (CancellationToken.None, TimeSpan.Zero, scheduler);

    Assert.IsFalse (unlocker.CanUnlock);
  }

  [TestMethod]
  async public Task TryLock__LockTaken ()
  {

    using AsyncLock asyncLock   = new ();
    Unlocker unlocker           = await asyncLock.AutoLock (CancellationToken.None, TimeSpan.Zero, TaskScheduler.Current);

    Assert.IsTrue (unlocker.CanUnlock);
  }

  [TestMethod]
  async public Task TryUnlock__Unlocked ()
  {

    using AsyncLock asyncLock   = new ();
    
    TaskScheduler scheduler     = TaskScheduler.Current;
    Unlocker unlocker           = await asyncLock.AutoLock (CancellationToken.None, Timeout.InfiniteTimeSpan, scheduler);

    _ = unlocker.Unlock ();

    unlocker = await asyncLock.AutoLock (CancellationToken.None, Timeout.InfiniteTimeSpan, scheduler);

    // If blocking is not entered, code bellow will never execute.
    Assert.IsTrue (true);
  }

  [TestMethod]
  async public Task WaitWithTimeout__TimeoutExpires ()
  {

    using AsyncLock asyncLock   = new ();

    TaskScheduler scheduler     = TaskScheduler.Current;
    _ = await asyncLock.AutoLock (CancellationToken.None, Timeout.InfiniteTimeSpan, scheduler);

    const uint timeoutSeconds = 1;
    var sw = Stopwatch.StartNew();

    Unlocker unlocker = await asyncLock.AutoLock
    (
      CancellationToken.None,
      TimeSpan.FromSeconds(timeoutSeconds),
      scheduler
    );

    sw.Stop ();

    double elapsedSeconds = sw.Elapsed.TotalSeconds;

    Assert.IsTrue (timeoutSeconds <= elapsedSeconds);
    Assert.IsTrue (1 > elapsedSeconds - timeoutSeconds);
    Assert.IsFalse (unlocker.CanUnlock);
  }

  [TestMethod]
  public void PassNullTaskScheduler__ThrowsArgumentNullException ()
  {
    using AsyncLock asyncLock   = new ();

    AggregateException aggregateException = Assert.ThrowsException<AggregateException>
    (
      () => asyncLock.AutoLock (CancellationToken.None, Timeout.InfiniteTimeSpan, null!).Wait ()
    );

    ReadOnlyCollection<Exception> innerExceptions = aggregateException.InnerExceptions;
    Assert.AreEqual (1, innerExceptions.Count);
    Assert.AreEqual (typeof (ArgumentNullException), innerExceptions [0]!.GetType ());
  }
}
