using System;
using FluentAssertions;
using Localizer;
using Localizer.DataModel.Default;
using Localizer.Helpers;
using Xunit;

namespace LocalizerTest.Helper
{
    public class ExtensionsTest
    {
        [Theory]
        [InlineData(typeof(BasicItemFile), 1, "Items")]
        [InlineData(typeof(BasicNPCFile), 1, "NPCs")]
        [InlineData(typeof(BasicBuffFile), 1, "Buffs")]
        [InlineData(typeof(BasicPrefixFile), 1, "Prefixes")]
        [InlineData(typeof(BasicProjectileFile), 1, "Projectiles")]
        public void ModTranslationOwnerField_Correct(Type type, int count, string name)
        {
            type.ModTranslationOwnerField().Length.Should().Be(count);
            type.ModTranslationOwnerField()[0].Name.Should().Be(name);
        }
        
        [Fact]
        public void ModTranslationOwnerFieldName_Correct()
        {
            var prop = typeof(BasicItemFile).ModTranslationOwnerField()[0];
            prop.ModTranslationOwnerFieldName().Should().Be("items");
        }
        
        [Fact]
        public void ModTranslationProp_Correct()
        {
            var props = typeof(ItemEntry).ModTranslationProp();
            props.Length.Should().Be(2);
            props.Should().ContainSingle(p => p.Name == "Name");
            props.Should().ContainSingle(p => p.Name == "Tooltip");
        }
    }
}
