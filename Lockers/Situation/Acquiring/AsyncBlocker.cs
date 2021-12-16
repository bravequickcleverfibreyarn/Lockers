using Software9119.Lockers.Extensions;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Software9119.Lockers;

/// <summary>
/// Provides non-blocking access synchronization, i.e. does not call <see cref="Thread.MemoryBarrier"/>.
/// </summary>
sealed public class AsyncBlocker : IDisposable
{
  readonly private AutoResetEvent are = new(true);
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
  }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
