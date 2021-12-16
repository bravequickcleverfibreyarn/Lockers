using Microsoft.VisualStudio.TestTools.UnitTesting;

using Software9119.Lockers;

using System.Threading;

namespace LockersTests.Tests;

[TestClass]
sealed public class UnlockerTests
{
  [TestMethod]
  public void Dispose__SetsEventWaitHandle ()
  {
    using EventWaitHandle ewh = new (false, EventResetMode.AutoReset);

    using (new Unlocker (ewh)) { }

    Assert.IsTrue (ewh.WaitOne (0));
  }

  [TestMethod]
  public void Unlock__SetsEventWaitHandle ()
  {
    using EventWaitHandle ewh     = new (false, EventResetMode.AutoReset);
    Unlocker unlocker             = new (ewh);

    Assert.IsTrue (unlocker.Unlock ());
    Assert.IsTrue (ewh.WaitOne (0));
    Assert.IsTrue (unlocker.Unlock ());
    Assert.IsTrue (unlocker.Unlock ());
  }

  [TestMethod]
  public void TryUnlockWithDefault__CannotUnlock ()
  {
    Unlocker unlocker = default;
    Assert.IsFalse (unlocker.CanUnlock);
    Assert.IsFalse (unlocker.Unlock ());
  }

  [TestMethod]
  public void TryUnlockWithNew__CannotUnlock ()
  {
    Unlocker unlocker = new();
    Assert.IsFalse (unlocker.CanUnlock);
    Assert.IsFalse (unlocker.Unlock ());
  }
}
