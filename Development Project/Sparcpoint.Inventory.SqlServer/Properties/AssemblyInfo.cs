using System.Runtime.CompilerServices;

// EVAL: We open internals only to the test assembly so we can validate
// the JsonList helper without exposing it in the public API.
[assembly: InternalsVisibleTo("Sparcpoint.Inventory.Tests")]
