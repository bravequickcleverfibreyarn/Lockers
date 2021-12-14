using System;
using System.Threading;

namespace Lockers;

public struct EventWaitHandleReleaser : IDisposable
{
  private readonly EventWaitHandle? eventWaitHandle;

  private bool disposed;

  public EventWaitHandleReleaser ( EventWaitHandle eventWaitHandle )
  {
    this.eventWaitHandle = eventWaitHandle;

    disposed = false;
  }

  public void Dispose ()
  {
    if (disposed)
      return;

    _ = Release ();

    disposed = true;
  }

  /// <summary>
  /// Calls <see cref="Thread.MemoryBarrier"/>, then <see cref="EventWaitHandle.Set"/>.
  /// </summary>
  public bool Release ()
  {
    if (eventWaitHandle is null)
      return false;
        
    Thread.MemoryBarrier ();
    return eventWaitHandle.Set ();
  }
}
