# AGENTS.md

This file provides guidance to agents when working with code in this repository.

## Commands

- Restore/build the whole solution: `dotnet restore CheckAlgorithm.sln` and `dotnet build CheckAlgorithm.sln`.
- Run all tests: `dotnet test Tests/Tests.csproj`.
- Run one MSTest: `dotnet test Tests/Tests.csproj --filter "FullyQualifiedName=Tests.RunTests.BasicRunTest"`.
- Run the console scenario: `dotnet run --project CheckAlgorithm/CheckAlgorithm.csproj`.
- Coverage is available through the referenced collector: `dotnet test Tests/Tests.csproj --collect:"XPlat Code Coverage"`.
- No repository lint or formatter configuration was found; preserve the edited file's existing formatting and namespace style.

## Project-specific contracts

- [`Runner.ExecuteStep()`](RunAlgorithm.Core/Runner.cs:64) clones the context for every candidate branch; implementations of [`IContext.Clone()`](RunAlgorithm.Core/IContext.cs:5) must provide the branch isolation the algorithm expects.
- A step returning `false` becomes `RunResult.Stopped`; thrown exceptions become `RunResult.Failed` and are evaluated later, rather than escaping the run.
- [`VariantsStep`](RunAlgorithm.Core/Steps/VariantsStep.cs:3) is only a branch marker: its `Execute` is not called, nested compound steps are rejected, and leaf `Variants` properties intentionally throw `NotImplementedException`.
- Terminal validation runs only after no actor can advance; validators receive the branch's cloned context and final actor results.
- [`SmartExecutor`](RunAlgorithm.Core/Runtime/SmartExecutor.cs:3) switches to thread-pool execution when a batch exceeds `Environment.ProcessorCount * 2`; actor/step implementations must tolerate that shared execution model.
- The test project references only `RunAlgorithm.Core`; tests for [`TwoPhaseCommit`](TwoPhaseCommit/Actor.cs:8) require adding that project reference before use.
- [`CheckAlgorithm`](CheckAlgorithm/Program.cs:9) is the composition/demo executable; algorithm behavior belongs in the core and scenario projects.
