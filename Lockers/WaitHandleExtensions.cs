﻿using Lockers.Internal;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lockers
{
  static public class WaitHandleExtensions
  {
    static public Task<bool> WaitOneAsync (this WaitHandle wh, CancellationToken ct, TimeSpan maxWaitTime, TaskScheduler scheduler)
    {
      Validate (wh, scheduler);
      return WaitOneAsync (wh, ct, (uint) maxWaitTime.TotalMilliseconds, scheduler);
    }

    static public Task<bool> WaitOneAsync (this WaitHandle wh, CancellationToken ct, uint maxWaitTime, TaskScheduler scheduler)
    {
      Validate (wh, scheduler);
      return WaitHandleExtensions_UncheckedInputs.WaitOneAsync (wh, ct, maxWaitTime, scheduler);
    }

    private static void Validate (WaitHandle wh, TaskScheduler scheduler)
    {
      if (wh is null)
        throw new ArgumentNullException (nameof (wh));

      if (scheduler is null)
        throw new ArgumentNullException (nameof (scheduler));
    }
  }
}

namespace Lockers.Internal
{
  static internal class WaitHandleExtensions_UncheckedInputs
  {
    static public Task<bool> WaitOneAsync (WaitHandle wh, CancellationToken ct, TimeSpan maxWaitTime, TaskScheduler scheduler)
    {
      return WaitOneAsync (wh, ct, (uint) maxWaitTime.TotalMilliseconds, scheduler);
    }

    static public Task<bool> WaitOneAsync (WaitHandle wh, CancellationToken ct, uint maxWaitTime, TaskScheduler scheduler)
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