using Software9119.Aid.Concurrency.Unchecked;

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
  bool disposed;

  /// <remarks>
  /// Use <see cref="Timeout.InfiniteTimeSpan"/> for no timeout and <see cref="TimeSpan.Zero"/> for immediate timeout.
  /// </remarks>
  /// <exception cref="ArgumentNullException">When <paramref name="scheduler"/> is <see langword="null"/>.</exception>
  /// <exception cref="TaskCanceledException" />
  async public Task<Unblocker> AutoBlock ( CancellationToken ct, TimeSpan maxWaitTime, TaskScheduler scheduler )
  {
    if (scheduler is null)
      throw new ArgumentNullException (nameof (scheduler));

    if (await are.WaitOneAsync (ct, maxWaitTime, scheduler).ConfigureAwait (false))
      return new Unblocker (are);

    return default;
  }

  public void Dispose ()
  {
    if (disposed)
      return;

    are.Dispose ();

    disposed = true;

    Dispose (true);
    GC.SuppressFinalize (this);
  }

  virtual protected void Dispose ( bool disposing ) { }
}
