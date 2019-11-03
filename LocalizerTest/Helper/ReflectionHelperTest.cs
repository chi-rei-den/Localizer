using System.Reflection;
using FluentAssertions;
using Localizer;
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
    
    public class ReflectionHelperTest
    {
        [Fact]
        public void Field_Correct()
        {
            var foo = new Foo();
            (foo.Field("Bar") as string).Should().Be("Bar");
            
            (typeof(Foo).Field("Bar") as string).Should().Be("StillBar");
        }
        
        [Fact]
        public void Prop_Correct()
        {
            var foo = new Foo();
            (foo.Prop("Baz") as string).Should().Be("Baz");
            
            (typeof(Foo).Prop("Baz") as string).Should().Be("StillBaz");
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
