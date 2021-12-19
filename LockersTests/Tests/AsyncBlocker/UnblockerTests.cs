using Microsoft.VisualStudio.TestTools.UnitTesting;

using Software9119.Lockers;

using System.Threading;

namespace LockersTests.Tests;

[TestClass]
sealed public class UnblockerTests
{

  [TestMethod]
  public void Dispose__SetsEventWaitHandle ()
  {
    using EventWaitHandle ewh = new (false, EventResetMode.AutoReset);

    using (new Unblocker (ewh)) { }

    Assert.IsTrue (ewh.WaitOne (0));
  }

  [TestMethod]
  public void Unblock__SetsEventWaitHandle ()
  {
    using EventWaitHandle ewh     = new (false, EventResetMode.AutoReset);
    Unblocker unblocker           = new (ewh);

    Assert.IsTrue (unblocker.Unblock ());
    Assert.IsTrue (ewh.WaitOne (0));
    Assert.IsTrue (unblocker.Unblock ());
    Assert.IsTrue (unblocker.Unblock ());
  }

  [TestMethod]
  public void TryUnblockWithDefault__CannotUnblock ()
  {
    Unblocker unblocker = default;
    Assert.IsFalse (unblocker.CanUnblock);
    Assert.IsFalse (unblocker.Unblock ());
  }


  [TestMethod]
  public void TryUnblockWithNew__CannotUnblock ()
  {
    Unblocker unblocker = new();
    Assert.IsFalse (unblocker.CanUnblock);
    Assert.IsFalse (unblocker.Unblock ());
  }
}
