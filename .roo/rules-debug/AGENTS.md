# Debug mode guidance

- Inspect the path and cloned context at [`Runner.ExecuteStep()`](RunAlgorithm.Core/Runner.cs:64): `ApplicationException` is logged at Debug, while other exceptions are logged at Information; the demo console minimum is Information.
- A failure is recorded as a branch result and validated at a terminal state, so an apparently successful `Runner.Run()` call can contain failures in [`RunStatistics`](RunAlgorithm.Core/RunStatistics.cs:1).
- Path strings use zero-based actor indexes in [`StepPath`](RunAlgorithm.Core/Runtime/StepPath.cs:5), and progress events are emitted only per 100,000 processed results in [`Runner`](RunAlgorithm.Core/Runner.cs:15).
- Large executor batches run on thread-pool workers through [`AsyncWrapper`](RunAlgorithm.Core/Runtime/AsyncWrapper.cs:3); reproduce concurrency-sensitive issues with enough actor variants to cross the threshold.
