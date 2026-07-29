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

            var runner = new Runner(new[] {a1, a2}, ctx, validator, new NullLogger<Runner>());

            var stat = runner.Run();
            Assert.AreEqual( 6, stat.Total );
            Assert.AreEqual( 6, stat.PositiveResults );
            Assert.AreEqual(0, stat.NegativeResults);
            Assert.AreEqual(0, stat.Failures);

            ctx = new CountingContext();
            validator = new StringValidator( CheckResult.SuccessNegative );
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
            var a1 = new SimpleActor(new SimpleStep(), new FailureStep(), new SimpleStep() );
            var a2 = new SimpleActor(new SimpleStep(), new SimpleStep());

            var ctx = new CountingContext();
            var validator = new SmartValidator();

            var runner = new Runner(new[] { a1, a2 }, ctx, validator, new NullLogger<Runner>());

            var stat = runner.Run();
            Assert.AreEqual(6, stat.Total);
            Assert.AreEqual(0, stat.PositiveResults);
            Assert.AreEqual(0, stat.NegativeResults);
            Assert.AreEqual(6, stat.Failures);

            a1 = new SimpleActor(new FailureStep(), new SimpleStep() );
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
            var a1 = new SimpleActor(new SimpleStep(), new SimpleStep( false ), new SimpleStep());
            var a2 = new SimpleActor(new SimpleStep(), new SimpleStep());

            var ctx = new CountingContext();
            var validator = new SmartValidator();

            var runner = new Runner(new[] { a1, a2 }, ctx, validator, new NullLogger<Runner>());

            var stat = runner.Run();
            Assert.AreEqual(6, stat.Total);
            Assert.AreEqual(0, stat.PositiveResults);
            Assert.AreEqual(6, stat.NegativeResults);
            Assert.AreEqual(0, stat.Failures);

            a1 = new SimpleActor(new SimpleStep( false ), new SimpleStep());
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
    }
}
