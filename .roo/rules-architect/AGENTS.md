# Architect mode guidance

- Keep the dependency direction `CheckAlgorithm`/`TwoPhaseCommit` -> `RunAlgorithm.Core`; `Tests` currently references only Core.
- The algorithm's state boundary is [`IContext.Clone()`](RunAlgorithm.Core/IContext.cs:5), not the actor objects: actor step lists are reused across branches, while contexts are expected to diverge.
- Branch expansion is represented by [`StepPath`](RunAlgorithm.Core/Runtime/StepPath.cs:5) and [`RunActorStep`](RunAlgorithm.Core/Runtime/RunActorStep.cs:3); terminal results are aggregated by executors and sent to the host in batches.
- Preserve the intentional restriction in [`VariantsStep`](RunAlgorithm.Core/Steps/VariantsStep.cs:3): variants cannot contain other variant lists.
- Any architectural change to executor parallelism must account for host result aggregation under [`IRunnerHost.AddResults()`](RunAlgorithm.Core/Runtime/IRunnerHost.cs:1) and for shared actor/step instances.
