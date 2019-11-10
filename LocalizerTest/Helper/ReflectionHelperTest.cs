using System.Reflection;
using FluentAssertions;
using Localizer;
using Localizer.Helpers;
using Xunit;

namespace LocalizerTest.Helper
{
    class Foo
    {
        private string bar = "Bar";

        private static string staticBar = "StillBar";

        private string Baz { get; } = "Baz";
        
        private static string StaticBaz { get; } = "StillBaz";

        private string Qux(string qux)
        {
            return $"Qux: {qux}";
        }

        private static string StaticQux(string qux)
        {
            return $"StillQux: {qux}";
        }
    }

    class Foo2 : Foo
    {
        
    }
    
    public class ReflectionHelperTest
    {
        [Fact]
        public void GetFieldRecursively_Correct()
        {
            var foo = new Foo2();
            foo.GetType().GetFieldRecursively("bar").GetValue(foo).Should().Be("Bar");
            
            (typeof(Foo2).GetFieldRecursively("staticBar").GetValue(foo) as string).Should().Be("StillBar");
        }
        
        [Fact]
        public void Field_Correct()
        {
            var foo = new Foo();
            (foo.Field("bar") as string).Should().Be("Bar");
            
            (typeof(Foo).Field("staticBar") as string).Should().Be("StillBar");
        }
        
        [Fact]
        public void SetField_Correct()
        {
            var foo = new Foo();
            foo.SetField("bar", "Bar2");
            (foo.Field("bar") as string).Should().Be("Bar2");
            
            typeof(Foo).SetField("staticBar", "StillBar2");
            (typeof(Foo).Field("staticBar") as string).Should().Be("StillBar2");
        }
        
        [Fact]
        public void Prop_Correct()
        {
            var foo = new Foo();
            (foo.Prop("Baz") as string).Should().Be("Baz");
            
            (typeof(Foo).Prop("StaticBaz") as string).Should().Be("StillBaz");
        }
        
        [Fact]
        public void Method_Correct()
        {
            var foo = new Foo();
            (foo.Method("Qux", new object[] { "qux" }) as string)
                .Should().Be("Qux: qux");
            (foo.Method("Qux", "qux") as string)
                .Should().Be("Qux: qux");
            
            (typeof(Foo).Method("StaticQux", new object[] { "qux" }) as string)
                .Should().Be("StillQux: qux");
            (typeof(Foo).Method("StaticQux", "qux") as string)
                .Should().Be("StillQux: qux");
        }
    }
}
