using Microsoft.VisualStudio.TestTools.UnitTesting;

using Software9119.Lockers;

using System.Threading;
using System.Threading.Tasks;

namespace LockersTests.Tests;

[TestClass]
sealed public class AsyncLockTests_Unlock
{

  [TestMethod]
  async public Task TryUnlock__Unlocked ()
  {

    using AsyncLock asyncLock   = new ();

    TaskScheduler scheduler     = TaskScheduler.Current;

    _ = await asyncLock.Lock (CancellationToken.None, Timeout.InfiniteTimeSpan, scheduler);
    _ = asyncLock.Unlock ();

    Assert.IsTrue
    (
      await asyncLock.Lock (CancellationToken.None, Timeout.InfiniteTimeSpan, scheduler)
    );
  }
}
