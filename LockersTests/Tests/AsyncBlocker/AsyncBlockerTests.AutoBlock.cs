using Microsoft.VisualStudio.TestTools.UnitTesting;

using Software9119.Lockers;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LockersTests.Tests;

[TestClass]
sealed public class AsyncBlockerTests_AutoBlock
{
  [TestMethod]
  async public Task CancellationRequested__TaskIsCancelled ()
  {

    using AsyncBlocker blocker           = new ();
    using CancellationTokenSource cts    = new ();

    TaskScheduler scheduler = TaskScheduler.Current;
    _ = await blocker.AutoBlock (CancellationToken.None, Timeout.InfiniteTimeSpan, scheduler);

    Task<Unblocker> test = blocker.AutoBlock(cts.Token, Timeout.InfiniteTimeSpan, scheduler);

    cts.Cancel ();

    AggregateException aggregateException = Assert.ThrowsException<AggregateException> (() => test.Wait ());

    ReadOnlyCollection<Exception> innerExceptions = aggregateException.InnerExceptions;
    Assert.AreEqual (1, innerExceptions.Count);
    Assert.AreEqual (typeof (TaskCanceledException), innerExceptions [0]!.GetType ());
  }

  [TestMethod]
  async public Task Blocking__CannotBlock ()
  {
    using AsyncBlocker blocker  = new ();
    TaskScheduler scheduler     = TaskScheduler.Current;

    _ = await blocker.AutoBlock (CancellationToken.None, Timeout.InfiniteTimeSpan, scheduler);

    Unblocker unblocker = await blocker.AutoBlock (CancellationToken.None, TimeSpan.Zero, scheduler);

    Assert.IsFalse (unblocker.CanUnblock);
  }

  [TestMethod]
  async public Task TryBlock__Blocking ()
  {

    using AsyncBlocker blocker  = new ();
    Unblocker unblocker         = await blocker.AutoBlock(CancellationToken.None, TimeSpan.Zero, TaskScheduler.Current);

    Assert.IsTrue (unblocker.CanUnblock);
  }

  [TestMethod]
  async public Task TryUnblock__Unblocked ()
  {

    using AsyncBlocker blocker  = new ();

    TaskScheduler scheduler   = TaskScheduler.Current;
    Unblocker unblocker       = await blocker.AutoBlock (CancellationToken.None, Timeout.InfiniteTimeSpan, scheduler);

    _ = unblocker.Unblock ();

    unblocker = await blocker.AutoBlock (CancellationToken.None, Timeout.InfiniteTimeSpan, scheduler);

    // If blocking is not entered, code bellow will never execute.
    Assert.IsTrue (true);
  }

  [TestMethod]
  async public Task WaitWithTimeout__TimeoutExpires ()
  {

    using AsyncBlocker blocker  = new ();
    TaskScheduler scheduler     = TaskScheduler.Current;

    _ = await blocker.AutoBlock (CancellationToken.None, Timeout.InfiniteTimeSpan, scheduler);

    const uint timeoutSeconds = 1;
    var sw = Stopwatch.StartNew();

    Unblocker unblocker = await blocker.AutoBlock
    (
      CancellationToken.None,
      TimeSpan.FromSeconds(timeoutSeconds),
      scheduler
    );

    sw.Stop ();

    double elapsedSeconds = sw.Elapsed.TotalSeconds;

    Assert.IsTrue (timeoutSeconds <= elapsedSeconds);
    Assert.IsTrue (1 > elapsedSeconds - timeoutSeconds);
    Assert.IsFalse (unblocker.CanUnblock);
  }

  [TestMethod]
  public void PassNullTaskScheduler__ThrowsArgumentNullException ()
  {
    using AsyncBlocker blocker  = new ();

    AggregateException aggregateException = Assert.ThrowsException<AggregateException>
    (
      () => blocker.AutoBlock (CancellationToken.None, Timeout.InfiniteTimeSpan, null!).Wait ()
    );

    ReadOnlyCollection<Exception> innerExceptions = aggregateException.InnerExceptions;
    Assert.AreEqual (1, innerExceptions.Count);
    Assert.AreEqual (typeof (ArgumentNullException), innerExceptions [0]!.GetType ());
  }
}
