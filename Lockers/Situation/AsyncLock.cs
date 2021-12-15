using Lockers.Internal.Extensions;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lockers;

/// <summary>
/// Provides non-blocking synchronization lock.
/// </summary>
/// <remarks>Calls <see cref="Thread.MemoryBarrier"/> upon lock take and before lock release.</remarks>
public sealed class AsyncLock : IDisposable
{
  private readonly AutoResetEvent autoResetEvent = new(true);
  private bool disposed;

  /// <remarks>
  /// Use <see cref="Timeout.InfiniteTimeSpan"/> for no timeout and <see cref="TimeSpan.Zero"/> for immediate timeout.
  /// </remarks>
  public async Task<Unlocker> AutoLock ( CancellationToken ct, TimeSpan maxWaitTime, TaskScheduler scheduler )
  {
    if (await Lock (ct, maxWaitTime, scheduler))
      return new Unlocker (autoResetEvent);

    return default;
  }

  /// <remarks>
  /// Use <see cref="Timeout.InfiniteTimeSpan"/> for no timeout and <see cref="TimeSpan.Zero"/> for immediate timeout.
  /// </remarks>
  public async Task<bool> Lock ( CancellationToken ct, TimeSpan maxWaitTime, TaskScheduler scheduler )
  {
    if (scheduler is null)
      throw new ArgumentNullException (nameof (scheduler));

    if (await autoResetEvent.WaitOneAsync (ct, maxWaitTime, scheduler))
    {
      Thread.MemoryBarrier ();
      return true;
    }

    return false;
  }

  public bool Unlock ()
  {
    Thread.MemoryBarrier ();
    return autoResetEvent.Set ();
  }

  public void Dispose ()
  {
    if (disposed)
      return;

    autoResetEvent.Dispose ();

    disposed = true;
  }
}
