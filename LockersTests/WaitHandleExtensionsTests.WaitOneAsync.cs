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

    using EventWaitHandle autoresetEvent = new (false, EventResetMode.AutoReset);    

    using CancellationTokenSource cts = new ();
    // Set up cancellable call.
    Task<bool> test = autoresetEvent.WaitOneAsync (cts.Token, Timeout.InfiniteTimeSpan, TaskScheduler.Current);

    cts.Cancel ();

    AggregateException aggregateException = Assert.ThrowsException<AggregateException> (() => test.Wait ());
    Assert.AreEqual (aggregateException.InnerException!.GetType (), typeof (TaskCanceledException));
  }

  [TestMethod]
  public async Task WaitHandleBlocks__CannotTakeWaithHandle ()
  {
    using EventWaitHandle autoresetEvent = new (false, EventResetMode.AutoReset);

    Assert.IsFalse
    (
      await autoresetEvent.WaitOneAsync (CancellationToken.None, TimeSpan.Zero, TaskScheduler.Current)
    );
  }

  [TestMethod]
  public async Task TakeWaithHandle_WaitHandleTaken ()
  {

    using EventWaitHandle autoresetEvent = new (true, EventResetMode.AutoReset);

    Assert.IsTrue
    (
      await autoresetEvent.WaitOneAsync (CancellationToken.None, Timeout.InfiniteTimeSpan, TaskScheduler.Current)
    );

    Assert.IsFalse (autoresetEvent.WaitOne (0));
  }

  [TestMethod]
  public async Task WaitWithTimeout_TimeoutExpires ()
  {

    using EventWaitHandle autoresetEvent = new (false, EventResetMode.AutoReset);

    const uint timeoutSeconds = 3;
    var sw = Stopwatch.StartNew();

    bool taken = await autoresetEvent.WaitOneAsync 
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
}