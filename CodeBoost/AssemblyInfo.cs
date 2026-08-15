using System.Runtime.CompilerServices;

/* The test assembly targets a runtime that dispatches to the framework intrinsics, so the polyfill fallbacks it exists to replace are
 * reachable only through internals. Without this they would ship on netstandard2.0 having never been executed by anything. */
[assembly: InternalsVisibleTo("CodeBoost.Tests")]
