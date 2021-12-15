using Lockers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading;

namespace LockersTests;

[TestClass]
public class UnlockerTests
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
  public void Unlock__CannotUnlockWithDefault ()
  {
    Unlocker unlocker = default;
    Assert.IsFalse (unlocker.CanUnlock);
    Assert.IsFalse (unlocker.Unlock ());
  }

  [TestMethod]
  public void Unlock__CannotUnlockWithNew ()
  {
    Unlocker unlocker = new();
    Assert.IsFalse (unlocker.CanUnlock);
    Assert.IsFalse (unlocker.Unlock ());
  }
}
