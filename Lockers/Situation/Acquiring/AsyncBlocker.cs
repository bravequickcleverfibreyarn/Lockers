using Software9119.Lockers.Extensions;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Software9119.Lockers;

/// <summary>
/// Provides non-blocking access synchronization, i.e. does not call <see cref="Thread.MemoryBarrier"/>.
/// </summary>
public class AsyncBlocker : IDisposable
{
  /// <summary>
  /// <see cref="EventWaitHandle"/> that is used for synced access.
  /// </summary>
#pragma warning disable CA1051 // Do not declare visible instance fields
  readonly protected AutoResetEvent are = new(true);
#pragma warning restore CA1051 // Do not declare visible instance fields
  private bool disposed;

  /// <remarks>
  /// Use <see cref="Timeout.InfiniteTimeSpan"/> for no timeout and <see cref="TimeSpan.Zero"/> for immediate timeout.
  /// </remarks>
  async public Task<Unblocker> AutoBlock ( CancellationToken ct, TimeSpan maxWaitTime, TaskScheduler scheduler )
  {
    if (scheduler is null)
      throw new ArgumentNullException (nameof (scheduler));

    if (await are.WaitOneAsync (ct, maxWaitTime, scheduler))
      return new Unblocker (are);

    return default;
  }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public void Dispose ()
  {
    if (disposed)
      return;

    are.Dispose ();

    disposed = true;

    Dispose (true);
    GC.SuppressFinalize (this);
  }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  virtual protected void Dispose ( bool disposing ) { }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
