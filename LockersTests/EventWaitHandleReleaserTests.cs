using Lockers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading;

namespace LockersTests;

[TestClass]
public class EventWaitHandleReleaserTests
{
  [TestMethod]
  public void Dispose__ReleasesEventWaitHandle ()
  {
    using EventWaitHandle ewh = new (false, EventResetMode.AutoReset);

    using (new EventWaitHandleReleaser (ewh)) { }

    Assert.IsTrue (ewh.WaitOne (0));
  }

  [TestMethod]
  public void Release__ReleasesEventWaitHandle ()
  {
    using EventWaitHandle ewh     = new (false, EventResetMode.AutoReset);
    EventWaitHandleReleaser ewhr  = new (ewh);

    Assert.IsTrue (ewhr.Release ());
    Assert.IsTrue (ewhr.Release ());
    Assert.IsTrue (ewh.WaitOne (0));
  }

  [TestMethod]
  public void Release__CannotReleaseWithDefault ()
  {
    EventWaitHandleReleaser ewhr  = default;
    Assert.IsFalse (ewhr.Release ());
  }

  [TestMethod]
  public void Release__CannotReleaseWithNew ()
  {
    EventWaitHandleReleaser ewhr  = new();
    Assert.IsFalse (ewhr.Release ());
  }
}
