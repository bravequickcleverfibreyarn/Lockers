using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Software9119.Lockers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public struct Unblocker : IDisposable
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
{

  readonly private EventWaitHandle ewh;
  private bool disposed;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public Unblocker ( EventWaitHandle ewh )
  {
    this.ewh = ewh;
    disposed = false;
  }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

  /// <returns>
  /// Whether there is <see cref="EventWaitHandle"/>
  /// that can be <see cref="EventWaitHandle.Set"/>.
  /// </returns>
  /// <remarks>
  /// When blocking started, returns <see langword="true"/>.
  /// </remarks>
  [MemberNotNullWhen (true, nameof (ewh))]
  public bool CanUnblock => ewh is not null;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public void Dispose ()
  {
    if (disposed)
      return;

    _ = Unblock ();

    disposed = true;
  }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public bool Unblock () => CanUnblock && ewh.Set ();
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
