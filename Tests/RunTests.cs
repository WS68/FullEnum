using Microsoft.Extensions.Logging.Abstractions;
using RunAlgorithm.Core;
using RunAlgorithm.Core.Steps;
using Tests.Utils;

namespace Tests
{
    [TestClass]
    public class RunTests
    {
        /// <summary>
        /// “ест провер€ет, что все считаетс€ и результат валидации передаетс€ на выход
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
        /// “ест провер€ет, что все считаетс€ и результат валидации передаетс€ на выход
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
        /// “ест провер€ет, что все считаетс€ и результат валидации передаетс€ на выход
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
    }
}