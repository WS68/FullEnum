# Code mode guidance

- Preserve the existing file's namespace/brace style; there is no repository formatter or analyzer policy.
- Keep [`IContext.Clone()`](RunAlgorithm.Core/IContext.cs:5) semantics correct for branch isolation. The test helper [`CountingContext`](Tests/Utils/CountingContext.cs:10) intentionally returns itself, so do not generalize that shortcut to mutable production contexts.
- Treat [`VariantsStep`](RunAlgorithm.Core/Steps/VariantsStep.cs:3) as a branch marker, not an executable step; do not invoke its `Execute` method or nest it.
- Do not replace exception-to-`RunResult.Failed` handling in [`Runner.ExecuteStep()`](RunAlgorithm.Core/Runner.cs:64) with ordinary exception propagation without updating validator/test expectations.
