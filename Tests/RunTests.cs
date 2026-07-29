using Microsoft.Extensions.Logging.Abstractions;
using RunAlgorithm.Core;
using RunAlgorithm.Core.Steps;
using SyncRootFolders;
using Tests.Utils;

namespace Tests
    {
    [TestClass]
    public class RunTests
        {
        /// <summary>
        /// Тест проверяет, что все считается и результат валидации передается на выход
        /// </summary>
        [TestMethod]
        public void BasicRunTest()
            {
            var a1 = new SimpleActor(new SimpleStep(), new SimpleStep());
            var a2 = new SimpleActor(new SimpleStep(), new SimpleStep());

            var ctx = new CountingContext();
            var validator = new StringValidator();

            var runner = new Runner(new[] { a1, a2 }, ctx, validator, new NullLogger<Runner>());

            var stat = runner.Run();
            Assert.AreEqual(6, stat.Total);
            Assert.AreEqual(6, stat.PositiveResults);
            Assert.AreEqual(0, stat.NegativeResults);
            Assert.AreEqual(0, stat.Failures);

            ctx = new CountingContext();
            validator = new StringValidator(CheckResult.SuccessNegative);
            runner = new Runner(new[] { a1, a2 }, ctx, validator, new NullLogger<Runner>());

            stat = runner.Run();
            Assert.AreEqual(6, stat.Total);
            Assert.AreEqual(0, stat.PositiveResults);
            Assert.AreEqual(6, stat.NegativeResults);
            Assert.AreEqual(0, stat.Failures);

            ctx = new CountingContext();
            validator = new StringValidator(CheckResult.Failure);
            runner = new Runner(new[] { a1, a2 }, ctx, validator, new NullLogger<Runner>());

            stat = runner.Run();
            Assert.AreEqual(6, stat.Total);
            Assert.AreEqual(0, stat.PositiveResults);
            Assert.AreEqual(0, stat.NegativeResults);
            Assert.AreEqual(6, stat.Failures);
            }

        /// <summary>
        /// Тест проверяет, что все считается и результат валидации передается на выход
        /// </summary>
        [TestMethod]
        public void RunWithFailureTest()
            {
            var a1 = new SimpleActor(new SimpleStep(), new FailureStep(), new SimpleStep());
            var a2 = new SimpleActor(new SimpleStep(), new SimpleStep());

            var ctx = new CountingContext();
            var validator = new SmartValidator();

            var runner = new Runner(new[] { a1, a2 }, ctx, validator, new NullLogger<Runner>());

            var stat = runner.Run();
            Assert.AreEqual(6, stat.Total);
            Assert.AreEqual(0, stat.PositiveResults);
            Assert.AreEqual(0, stat.NegativeResults);
            Assert.AreEqual(6, stat.Failures);

            a1 = new SimpleActor(new FailureStep(), new SimpleStep());
            a2 = new SimpleActor(new SimpleStep(), new SimpleStep());

            ctx = new CountingContext();
            validator = new SmartValidator();

            runner = new Runner(new[] { a1, a2 }, ctx, validator, new NullLogger<Runner>());

            stat = runner.Run();
            Assert.AreEqual(3, stat.Total);
            Assert.AreEqual(0, stat.PositiveResults);
            Assert.AreEqual(0, stat.NegativeResults);
            Assert.AreEqual(3, stat.Failures);
            }

        /// <summary>
        /// Тест проверяет, что все считается и результат валидации передается на выход
        /// </summary>
        [TestMethod]
        public void RunWithStopTest()
            {
            var a1 = new SimpleActor(new SimpleStep(), new SimpleStep(false), new SimpleStep());
            var a2 = new SimpleActor(new SimpleStep(), new SimpleStep());

            var ctx = new CountingContext();
            var validator = new SmartValidator();

            var runner = new Runner(new[] { a1, a2 }, ctx, validator, new NullLogger<Runner>());

            var stat = runner.Run();
            Assert.AreEqual(6, stat.Total);
            Assert.AreEqual(0, stat.PositiveResults);
            Assert.AreEqual(6, stat.NegativeResults);
            Assert.AreEqual(0, stat.Failures);

            a1 = new SimpleActor(new SimpleStep(false), new SimpleStep());
            a2 = new SimpleActor(new SimpleStep(), new SimpleStep());

            ctx = new CountingContext();
            validator = new SmartValidator();

            runner = new Runner(new[] { a1, a2 }, ctx, validator, new NullLogger<Runner>());

            stat = runner.Run();
            Assert.AreEqual(3, stat.Total);
            Assert.AreEqual(0, stat.PositiveResults);
            Assert.AreEqual(3, stat.NegativeResults);
            Assert.AreEqual(0, stat.Failures);
            }

        [TestMethod]
        public void ReadCommittedReadsOnlyCompletedTransactions()
            {
            var context = new Context();

            var firstTransaction = context.GetTransaction("first");

            var secondTransaction = context.GetTransaction("second");
            context.CloseTransaction("second");

            context.WriteInContent("first", true);

            Assert.IsFalse(context.ReadInContentImplicit());

            context.CloseTransaction("first");

            Assert.IsTrue(context.ReadInContentImplicit());
            Assert.AreEqual(1, firstTransaction);
            Assert.AreEqual(2, secondTransaction);
            }

        [TestMethod]
        public void ReadCommittedHidesActiveIsRootWrite()
            {
            var context = new Context();

            context.GetTransaction("first");

            context.GetTransaction("second");
            context.CloseTransaction("second");

            context.WriteIsRoot("first", false);

            Assert.IsTrue(context.ReadIsRootImplicit());

            context.CloseTransaction("first");

            Assert.IsFalse(context.ReadIsRootImplicit());
            }

        [TestMethod]
        public void ReadCommittedRefreshesSnapshotForEachRead()
            {
            var context = new Context();
            var transaction = context.GetTransaction("actor");
            context.WriteIsRoot("actor", false);

            Assert.IsTrue(context.ReadIsRootImplicit());

            context.CloseTransaction("actor");

            Assert.IsFalse(context.ReadIsRootImplicit());
            Assert.AreEqual(1, transaction);
            }

        [TestMethod]
        public void NeedCheckStartsFalseAndHidesActiveWrite()
            {
            var context = new Context();

            Assert.IsFalse(context.ReadNeedCheckImplicit());

            context.GetTransaction("actor");
            context.WriteNeedCheck("actor", true);

            Assert.IsFalse(context.ReadNeedCheckImplicit());

            context.CloseTransaction("actor");

            Assert.IsTrue(context.ReadNeedCheckImplicit());
            }

        [TestMethod]
        public void FixIsRootSetsIsRootAndNeedCheckWhenValuesDiffer()
            {
            var context = new Context();
            var actor = new Actor("actor", false, false);

            context.GetTransaction("actor");
            context.SaveIsRoot("actor", context.ReadIsRoot("actor"));
            new SyncRootFolders.Steps.FixIsRootStep("actor", false).Execute(context);

            Assert.IsTrue(context.ReadIsRootImplicit());
            Assert.IsFalse(context.ReadNeedCheckImplicit());

            context.CloseTransaction("actor");

            Assert.IsFalse(context.ReadIsRootImplicit());
            Assert.IsTrue(context.ReadNeedCheckImplicit());
            }

        [TestMethod]
        public void FixIsRootDoesNotWriteWhenValuesMatch()
            {
            var context = new Context();

            context.GetTransaction("actor");
            context.SaveIsRoot("actor", context.ReadIsRoot("actor"));
            new SyncRootFolders.Steps.FixIsRootStep("actor", true).Execute(context);

            Assert.IsTrue(context.ReadIsRootImplicit());
            Assert.IsFalse(context.ReadNeedCheckImplicit());

            context.CloseTransaction("actor");

            Assert.IsTrue(context.ReadIsRootImplicit());
            Assert.IsFalse(context.ReadNeedCheckImplicit());
            }

        [TestMethod]
        public void ValidatorUsesNeedCheckOnlyWhenValuesMatch()
            {
            var validator = new Validator();
            var context = new Context();

            context.GetTransaction("initial");
            context.WriteInContent("initial", true);
            context.CloseTransaction("initial");

            Assert.AreEqual(CheckResult.Failure, validator.EvaluateResult(context, Array.Empty<RunResult>()));

            context.GetTransaction("actor");
            context.WriteInContent("actor", true);
            context.WriteNeedCheck("actor", true);
            context.CloseTransaction("actor");

            Assert.AreEqual(CheckResult.SuccessNegative, validator.EvaluateResult(context, Array.Empty<RunResult>()));
            }

        [TestMethod]
        public void CheckActorStopsWhenNeedCheckIsFalse()
            {
            var actor = new CheckActor();

            Assert.IsFalse(actor.Steps[0].Execute(new Context()));
            }

        [TestMethod]
        public void CheckActorCorrectsMatchingValuesAndResetsNeedCheck()
            {
            var context = new Context();
            context.GetTransaction("setup");
            context.WriteInContent("setup", true);
            context.WriteNeedCheck("setup", true);
            context.CloseTransaction("setup");

            var actor = new CheckActor();
            Assert.IsTrue(actor.Steps[0].Execute(context));
            actor.Steps[1].Execute(context);
            actor.Steps[2].Execute(context);
            actor.Steps[3].Execute(context);
            actor.Steps[4].Execute(context);

            Assert.IsTrue(context.ReadIsRootImplicit());
            Assert.IsTrue(context.ReadNeedCheckImplicit());

            actor.Steps[5].Execute(context);

            Assert.IsFalse(context.ReadIsRootImplicit());
            Assert.IsFalse(context.ReadNeedCheckImplicit());
            }

        [TestMethod]
        public void CheckActorDoesNotChangeDifferentValues()
            {
            var context = new Context();
            context.GetTransaction("setup");
            context.WriteIsRoot("setup", false);
            context.WriteInContent("setup", true);
            context.WriteNeedCheck("setup", true);
            context.CloseTransaction("setup");

            var actor = new CheckActor();
            Assert.IsTrue(actor.Steps[0].Execute(context));
            actor.Steps[1].Execute(context);
            actor.Steps[2].Execute(context);
            actor.Steps[3].Execute(context);
            actor.Steps[4].Execute(context);
            actor.Steps[5].Execute(context);

            Assert.IsFalse(context.ReadIsRootImplicit());
            Assert.IsTrue(context.ReadInContentImplicit());
            Assert.IsFalse(context.ReadNeedCheckImplicit());
            }
        }
    }
