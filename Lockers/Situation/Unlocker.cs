using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Software9119.Lockers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public struct Unlocker : IDisposable
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
{
  readonly private EventWaitHandle? eventWaitHandle;

  private bool disposed;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public Unlocker ( EventWaitHandle eventWaitHandle )
  {
    this.eventWaitHandle = eventWaitHandle;

    disposed = false;
  }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

  /// <returns>
  /// Whether there is <see cref="EventWaitHandle"/>
  /// that can be <see cref="EventWaitHandle.Set"/>.
  /// </returns>
  /// <remarks>
  /// If lock is taken, returns <see langword="true"/>.
  /// </remarks>
  [MemberNotNullWhen (true, nameof (eventWaitHandle))]
  public bool CanUnlock => eventWaitHandle is not null;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public void Dispose ()
  {
    if (disposed)
      return;

    _ = Unlock ();

    disposed = true;
  }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

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
