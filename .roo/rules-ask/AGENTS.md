# Ask mode guidance

- The core flow is branch enumeration: [`Runner`](RunAlgorithm.Core/Runner.cs:13) creates [`ExecItem`](RunAlgorithm.Core/Runtime/ExecItem.cs:3) branches, [`SmartExecutor`](RunAlgorithm.Core/Runtime/SmartExecutor.cs:3) drains them, and [`IValidator`](RunAlgorithm.Core/IValidator.cs:3) evaluates only terminal states.
- [`TwoPhaseCommit.Context`](TwoPhaseCommit/Context.cs:10) uses shallow object cloning plus token/document state; its validator treats multiple completed actors or zero documents as failures when no branch already failed.
- Test fixtures use [`StringValidator`](Tests/Utils/StringValidator.cs:1) to encode actor results as `R/E/S/F` strings, while [`SmartValidator`](Tests/Utils/SmartValidator.cs:1) exercises scenario outcomes.
- The executable's dependency-injection registrations in [`Program.Main()`](CheckAlgorithm/Program.cs:11) are the canonical wiring example; the test project does not reference the scenario project.
