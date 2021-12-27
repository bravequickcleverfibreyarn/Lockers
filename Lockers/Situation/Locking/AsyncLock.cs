using Software9119.Lockers.Internal.Extensions;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Software9119.Lockers;

/// <summary>
/// Provides non-blocking synchronization lock.
/// </summary>
/// <remarks>Calls <see cref="Thread.MemoryBarrier"/> upon lock take and before lock release.</remarks>
public class AsyncLock : IDisposable
{
  /// <summary>
  /// <see cref="EventWaitHandle"/> that is used for synced access.
  /// </summary>
#pragma warning disable CA1051 // Do not declare visible instance fields
  readonly protected AutoResetEvent are = new(true);
#pragma warning restore CA1051 // Do not declare visible instance fields
  bool disposed;

  /// <remarks>
  /// Use <see cref="Timeout.InfiniteTimeSpan"/> for no timeout and <see cref="TimeSpan.Zero"/> for immediate timeout.
  /// </remarks>
  /// <exception cref="ArgumentNullException">When <paramref name="scheduler"/> is <see langword="null"/>.</exception>
  /// <exception cref="TaskCanceledException" />
  async public Task<Unlocker> AutoLock ( CancellationToken ct, TimeSpan maxWaitTime, TaskScheduler scheduler )
  {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
    if (await Lock (ct, maxWaitTime, scheduler))
      return new Unlocker (are);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

    return default;
  }

  /// <remarks>
  /// Use <see cref="Timeout.InfiniteTimeSpan"/> for no timeout and <see cref="TimeSpan.Zero"/> for immediate timeout.
  /// </remarks>
  /// <exception cref="ArgumentNullException">When <paramref name="scheduler"/> is <see langword="null"/>.</exception>
  /// <exception cref="TaskCanceledException" />
  async public Task<bool> Lock ( CancellationToken ct, TimeSpan maxWaitTime, TaskScheduler scheduler )
  {
    if (scheduler is null)
      throw new ArgumentNullException (nameof (scheduler));

    if (await are.WaitOneAsync (ct, maxWaitTime, scheduler).ConfigureAwait (false))
    {
      Thread.MemoryBarrier ();
      return true;
    }

    return false;
  }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public bool Unlock ()
  {
    Thread.MemoryBarrier ();
    return are.Set ();
  }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

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