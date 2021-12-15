using Lockers.Extensions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LockersTests;

[TestClass]
public class WaitHandleExtensionsTests_WaitOneAsync
{
  [TestMethod]
  public void CancellationRequested__TaskIsCancelled ()
  {

    using EventWaitHandle ewh = new (false, EventResetMode.AutoReset);

    using CancellationTokenSource cts = new ();
    // Set up cancellable call.
    Task<bool> test = ewh.WaitOneAsync (cts.Token, Timeout.InfiniteTimeSpan, TaskScheduler.Current);

    cts.Cancel ();

    AggregateException aggregateException = Assert.ThrowsException<AggregateException> (() => test.Wait ());
    Assert.AreEqual (aggregateException.InnerException!.GetType (), typeof (TaskCanceledException));
  }

  [TestMethod]
  public async Task WaitHandleBlocks__CannotTakeWaithHandle ()
  {
    using EventWaitHandle ewh = new (false, EventResetMode.AutoReset);

    Assert.IsFalse
    (
      await ewh.WaitOneAsync (CancellationToken.None, TimeSpan.Zero, TaskScheduler.Current)
    );
  }

  [TestMethod]
  public async Task TakeWaithHandle_WaitHandleTaken ()
  {

    using EventWaitHandle ewh = new (true, EventResetMode.AutoReset);

    Assert.IsTrue
    (
      await ewh.WaitOneAsync (CancellationToken.None, Timeout.InfiniteTimeSpan, TaskScheduler.Current)
    );

    Assert.IsFalse (ewh.WaitOne (0));
  }

  [TestMethod]
  public async Task WaitWithTimeout_TimeoutExpires ()
  {

    using EventWaitHandle ewh = new (false, EventResetMode.AutoReset);

    const uint timeoutSeconds = 3;
    var sw = Stopwatch.StartNew();

    bool taken = await ewh.WaitOneAsync
    (
      CancellationToken.None,
      TimeSpan.FromSeconds(timeoutSeconds),
      TaskScheduler.Current
    );

    sw.Stop ();

    Assert.IsTrue (timeoutSeconds < sw.Elapsed.TotalSeconds);
    Assert.IsTrue (sw.Elapsed.TotalSeconds - timeoutSeconds < 1);
    Assert.IsFalse (taken);
  }

  [TestMethod]
  public void PassNullTaskScheduler__ThrowsArgumentNullException ()
  {
    using EventWaitHandle ewh = new (default, default);

    _ = Assert.ThrowsException<ArgumentNullException>
    (
      () => ewh.WaitOneAsync (default, default (TimeSpan), scheduler: null!).Wait ()
    );

    _ = Assert.ThrowsException<ArgumentNullException>
    (
      () => ewh.WaitOneAsync (default, default (int), scheduler: null!).Wait ()
    );
  }

  [TestMethod]
  public void PassNullEventWaitHandle__ThrowsArgumentNullException ()
  {
    using EventWaitHandle? ewh  = null;
    TaskScheduler scheduler     = TaskScheduler.Default;

    _ = Assert.ThrowsException<ArgumentNullException>
    (
      () => ewh!.WaitOneAsync (default, default (TimeSpan), scheduler).Wait ()
    );

    _ = Assert.ThrowsException<ArgumentNullException>
    (
      () => ewh!.WaitOneAsync (default, default (int), scheduler).Wait ()
    );
  }
}