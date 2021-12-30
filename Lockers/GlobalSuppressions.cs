// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage ("Design", "CA1063:Implement IDisposable Correctly", Justification = "False positive.", Scope = "member", Target = "~M:Software9119.Lockers.AsyncBlocker.Dispose")]
[assembly: SuppressMessage ("Design", "CA1063:Implement IDisposable Correctly", Justification = "False positive.", Scope = "member", Target = "~M:Software9119.Lockers.AsyncLock.Dispose")]

[assembly: SuppressMessage ("Design", "CA1068:CancellationToken parameters must come last", Justification = "Not this case.", Scope = "type", Target = "~T:Software9119.Lockers.AsyncLock")]
[assembly: SuppressMessage ("Design", "CA1068:CancellationToken parameters must come last", Justification = "Not this case.", Scope = "member", Target = "~M:Software9119.Lockers.AsyncBlocker.AutoBlock(System.Threading.CancellationToken,System.TimeSpan,System.Threading.Tasks.TaskScheduler)~System.Threading.Tasks.Task{Software9119.Lockers.Unblocker}")]

[assembly: SuppressMessage ("Style", "IDE0046:Convert to conditional expression", Justification = "Fine.", Scope = "member", Target = "~M:Software9119.Lockers.AsyncLock.AutoLock(System.Threading.CancellationToken,System.TimeSpan,System.Threading.Tasks.TaskScheduler)~System.Threading.Tasks.Task{Software9119.Lockers.Unlocker}")]
[assembly: SuppressMessage ("Style", "IDE0046:Convert to conditional expression", Justification = "Fine.", Scope = "member", Target = "~M:Software9119.Lockers.AsyncBlocker.AutoBlock(System.Threading.CancellationToken,System.TimeSpan,System.Threading.Tasks.TaskScheduler)~System.Threading.Tasks.Task{Software9119.Lockers.Unblocker}")]

[assembly: SuppressMessage ("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Not relevant.", Scope = "type", Target = "~T:Software9119.Lockers.Unlocker")]
[assembly: SuppressMessage ("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Not relevant.", Scope = "type", Target = "~T:Software9119.Lockers.Unblocker")]

