using System.Linq;
using FluentAssertions;
using Ninject;
using Xunit;

namespace LocalizerTest
{
    public interface IFoo
    {

    }

    public interface IBar<T> where T : IFoo
    {

    }

    public sealed class A : IFoo
    {

    }

    public sealed class B : IFoo
    {

    }

    public sealed class ABar : IBar<A>
    {

    }

    public sealed class BBar : IBar<B>
    {

    }

    public class NinjectTest
    {
        private StandardKernel _kernel;

        public NinjectTest()
        {
            _kernel = new StandardKernel();
        }

        [Fact]
        public void GenericTypeBinding()
        {
            _kernel.Bind(typeof(IBar<>)).To<ABar>().InSingletonScope();
            _kernel.Bind(typeof(IBar<>)).To<BBar>().InSingletonScope();

            var bars = _kernel.GetAll(typeof(IBar<>));

            bars.Count().Should().Be(2);
        }

        [Fact]
        public void MultiSingletonBinding()
        {
            _kernel.Bind(typeof(IBar<>), typeof(IBar<A>)).To<ABar>().InSingletonScope();

            var abar1 = _kernel.Get(typeof(IBar<>));
            var abar2 = _kernel.Get<IBar<A>>();

            abar1.GetType().Should().Be(abar2.GetType());
            abar1.Should().Be(abar2);
        }
    }
}
