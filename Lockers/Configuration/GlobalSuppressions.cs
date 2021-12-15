﻿// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage ("Design", "CA1068:CancellationToken parameters must come last", Justification = "Not this case.", Scope = "type", Target = "~T:Lockers.Extensions.WaitHandleExtensions")]
[assembly: SuppressMessage ("Design", "CA1068:CancellationToken parameters must come last", Justification = "Not this case.", Scope = "type", Target = "~T:Lockers.Internal.Extensions.WaitHandleExtensions")]
[assembly: SuppressMessage ("Design", "CA1068:CancellationToken parameters must come last", Justification = "Not this case.", Scope = "type", Target = "~T:Lockers.AsyncLock")]

[assembly: SuppressMessage ("Style", "IDE0022:Use expression body for methods", Justification = "No room.", Scope = "member", Target = "~M:Lockers.Extensions.WaitHandleExtensions.WaitOneAsync(System.Threading.WaitHandle,System.Threading.CancellationToken,System.TimeSpan,System.Threading.Tasks.TaskScheduler)~System.Threading.Tasks.Task{System.Boolean}")]
[assembly: SuppressMessage ("Style", "IDE0022:Use expression body for methods", Justification = "No room.", Scope = "member", Target = "~M:Lockers.Internal.Extensions.WaitHandleExtensions.WaitOneAsync(System.Threading.WaitHandle,System.Threading.CancellationToken,System.TimeSpan,System.Threading.Tasks.TaskScheduler)~System.Threading.Tasks.Task{System.Boolean}")]

[assembly: SuppressMessage ("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Not relevant.", Scope = "type", Target = "~T:Lockers.Unlocker")]
