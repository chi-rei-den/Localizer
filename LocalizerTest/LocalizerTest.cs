using System;
using FluentAssertions;
using Localizer;
using Localizer.Attributes;
using Xunit;

namespace LocalizerTest
{
    [OperationTiming(OperationTiming.BeforeModCtor)]
    internal class FooA { }
    [OperationTiming(OperationTiming.BeforeModLoad)]
    internal class FooB { }
    [OperationTiming(OperationTiming.PostContentLoad)]
    internal class FooC { }
    [OperationTiming(OperationTiming.Any)]
    internal class FooD { }

    internal class FooE { }

    public class LocalizerTest
    {
        [Theory]
        [InlineData(OperationTiming.BeforeModCtor, OperationTiming.Any)]
        [InlineData(OperationTiming.BeforeModCtor, OperationTiming.BeforeModCtor)]
        [InlineData(OperationTiming.BeforeModLoad, OperationTiming.Any)]
        [InlineData(OperationTiming.BeforeModLoad, OperationTiming.BeforeModLoad)]
        [InlineData(OperationTiming.PostContentLoad, OperationTiming.Any)]
        [InlineData(OperationTiming.PostContentLoad, OperationTiming.PostContentLoad)]
        [InlineData(OperationTiming.PostContentLoad, OperationTiming.PostContentLoad | OperationTiming.BeforeModLoad)]
        public void CanDoOperationNow_True(OperationTiming state, OperationTiming operation)
        {
            Localizer.Localizer.State = state;
            Localizer.Localizer.CanDoOperationNow(operation).Should().BeTrue();
        }

        [Theory]
        [InlineData(OperationTiming.BeforeModCtor, typeof(FooA))]
        [InlineData(OperationTiming.BeforeModLoad, typeof(FooD))]
        [InlineData(OperationTiming.PostContentLoad, typeof(FooE))]
        [InlineData(OperationTiming.BeforeModCtor, typeof(FooD))]
        [InlineData(OperationTiming.BeforeModCtor, typeof(FooE))]
        public void CanDoOperationNow_Type_True(OperationTiming state, Type t)
        {
            Localizer.Localizer.State = state;
            Localizer.Localizer.CanDoOperationNow(t).Should().BeTrue();
        }

        [Theory]
        [InlineData(OperationTiming.BeforeModCtor, OperationTiming.BeforeModLoad)]
        [InlineData(OperationTiming.BeforeModCtor, OperationTiming.PostContentLoad)]
        [InlineData(OperationTiming.BeforeModLoad, OperationTiming.PostContentLoad)]
        [InlineData(OperationTiming.PostContentLoad, OperationTiming.BeforeModCtor)]
        [InlineData(OperationTiming.PostContentLoad, OperationTiming.BeforeModCtor | OperationTiming.BeforeModLoad)]
        public void CanDoOperationNow_False(OperationTiming state, OperationTiming operation)
        {
            Localizer.Localizer.State = state;
            Localizer.Localizer.CanDoOperationNow(operation).Should().BeFalse();
        }

        [Theory]
        [InlineData(OperationTiming.BeforeModCtor, typeof(FooB))]
        [InlineData(OperationTiming.BeforeModCtor, typeof(FooC))]
        [InlineData(OperationTiming.PostContentLoad, typeof(FooB))]
        [InlineData(OperationTiming.PostContentLoad, typeof(FooA))]
        public void CanDoOperationNow_Type_False(OperationTiming state, Type t)
        {
            Localizer.Localizer.State = state;
            Localizer.Localizer.CanDoOperationNow(t).Should().BeFalse();
        }
    }
}
