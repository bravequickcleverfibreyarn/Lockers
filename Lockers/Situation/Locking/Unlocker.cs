using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Software9119.Lockers;

public struct Unlocker : IDisposable
{
  readonly EventWaitHandle? ewh;

  bool disposed;

  public Unlocker ( EventWaitHandle ewh )
  {
    this.ewh = ewh;

    disposed = false;
  }

  /// <returns>
  /// Whether there is <see cref="EventWaitHandle"/>
  /// that can be <see cref="EventWaitHandle.Set"/>.
  /// </returns>
  /// <remarks>
  /// If lock is taken, returns <see langword="true"/>.
  /// </remarks>
  [MemberNotNullWhen (true, nameof (ewh))]
  public bool CanUnlock => ewh is not null;

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
      return ewh.Set ();
    }

    return false;
  }
}
