using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Software9119.Lockers;

public struct Unlocker : IDisposable
{
  readonly private EventWaitHandle? eventWaitHandle;

  private bool disposed;

  public Unlocker ( EventWaitHandle eventWaitHandle )
  {
    this.eventWaitHandle = eventWaitHandle;

    disposed = false;
  }

  /// <returns>
  /// Whether there is <see cref="EventWaitHandle"/>
  /// that can be <see cref="EventWaitHandle.Set"/>.
  /// </returns>
  /// <remarks>
  /// If lock is taken, returns <see langword="true"/>.
  /// </remarks>
  [MemberNotNullWhen (true, nameof (eventWaitHandle))]
  public bool CanUnlock => eventWaitHandle is not null;

  public void Dispose ()
  {
    if (disposed)
      return;

    _ = Unlock ();

    disposed = true;
  }

  /// <summary>
  /// Calls <see cref="Thread.MemoryBarrier"/>, then <see cref="EventWaitHandle.Set"/>.
  /// </summary>
  public bool Unlock ()
  {
    if (CanUnlock)
    {
      Thread.MemoryBarrier ();
      return eventWaitHandle.Set ();
    }

    return false;
  }
}
