using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Software9119.Lockers;

public struct Unblocker : IDisposable
{

  readonly EventWaitHandle ewh;
  bool disposed;

  public Unblocker ( EventWaitHandle ewh )
  {
    this.ewh = ewh;
    disposed = false;
  }

  /// <returns>
  /// Whether there is <see cref="EventWaitHandle"/>
  /// that can be <see cref="EventWaitHandle.Set"/>.
  /// </returns>
  /// <remarks>
  /// When blocking started, returns <see langword="true"/>.
  /// </remarks>
  [MemberNotNullWhen (true, nameof (ewh))]
  public bool CanUnblock => ewh is not null;

  public void Dispose ()
  {
    if (disposed)
      return;

    _ = Unblock ();

    disposed = true;
  }

  public bool Unblock () => CanUnblock && ewh.Set ();
}
