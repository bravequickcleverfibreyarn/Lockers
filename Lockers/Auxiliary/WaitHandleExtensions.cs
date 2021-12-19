
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Software9119.Lockers.Extensions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  static public class WaitHandleExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  {
    /// <remarks>
    /// Use <see cref="Timeout.InfiniteTimeSpan"/> for no timeout and <see cref="TimeSpan.Zero"/> for immediate timeout.
    /// </remarks>
    /// <exception cref="ArgumentNullException">When any reference is <see langword="null"/>.</exception>
    /// <exception cref="TaskCanceledException" />
    static public Task<bool> WaitOneAsync ( this WaitHandle wh, CancellationToken ct, TimeSpan maxWaitTime, TaskScheduler scheduler )
    {
      Validate (wh, scheduler);
      return Internal.Extensions.WaitHandleExtensions.WaitOneAsync (wh, ct, (int) maxWaitTime.TotalMilliseconds, scheduler);
    }

    /// <remarks>
    /// Use <c>0</c> for no timeout and <c>-1</c> for immediate timeout.
    /// </remarks>
    /// <exception cref="ArgumentNullException">When any reference is <see langword="null"/>.</exception>
    /// <exception cref="TaskCanceledException" />
    static public Task<bool> WaitOneAsync ( this WaitHandle wh, CancellationToken ct, int maxWaitTime, TaskScheduler scheduler )
    {
      Validate (wh, scheduler);
      return Internal.Extensions.WaitHandleExtensions.WaitOneAsync (wh, ct, maxWaitTime, scheduler);
    }

    static void Validate ( WaitHandle wh, TaskScheduler scheduler )
    {
      if (wh is null)
        throw new ArgumentNullException (nameof (wh));

      if (scheduler is null)
        throw new ArgumentNullException (nameof (scheduler));
    }
  }
}

namespace Software9119.Lockers.Internal.Extensions
{
  static class WaitHandleExtensions
  {
    static public Task<bool> WaitOneAsync ( this WaitHandle wh, CancellationToken ct, TimeSpan maxWaitTime, TaskScheduler scheduler )
    {
      return WaitOneAsync (wh, ct, (int) maxWaitTime.TotalMilliseconds, scheduler);
    }

    static public Task<bool> WaitOneAsync ( this WaitHandle wh, CancellationToken ct, int maxWaitTime, TaskScheduler scheduler )
    {
      TaskCompletionSource<bool> tcs      = new ();
      CancellationTokenRegistration ctr   = default;

      if (ct != CancellationToken.None)
        ctr = ct.Register (() => tcs.TrySetCanceled (ct));

      RegisteredWaitHandle rwh = ThreadPool.RegisterWaitForSingleObject
      (
        wh,
        (_, timedOut) => tcs.TrySetResult(!timedOut),
        null,
        maxWaitTime,
        true
      );

      Task<bool> acquisition = tcs.Task;

      _ = acquisition.ContinueWith
      (
        async __ =>
        {
          if (ctr != default)
            await ctr.DisposeAsync ();

          _ = rwh.Unregister (null);
        },
        scheduler
      );

      return acquisition;
    }
  }
}