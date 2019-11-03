using FluentAssertions;
using Localizer.DataModel.Default;
using Xunit;
using ModUtils = Localizer.Utils;

namespace LocalizerTest.Helper
{
    public class Utils
    {
        [Fact]
        public void CreateEntryMappings_Correct()
        {
            var mappings = ModUtils.CreateEntryMappings(typeof(ItemEntry));

            mappings["DisplayName"].Name.Should().Be("Name");
            mappings["Tooltip"].Name.Should().Be("Tooltip");
        }

        [Fact]
        public void GetMethodBase_Correct()
        {
            var thisMethod = ModUtils.GetMethodBase<Utils>("System.Void LocalizerTest.Utils::GetMethodBase_Correct()");
            thisMethod.Name.Should().Be(nameof(GetMethodBase_Correct));
        }

        [Fact]
        public void GetMethodBase_Wrong()
        {
            var thisMethod = ModUtils.GetMethodBase<Utils>("");
            thisMethod.Should().BeNull();
        }

        [Fact]
        public void GetTranslationEntry_Correct()
        {
            var testEntry = new ItemEntry
            {
                Name = new BaseEntry
                {
                    Origin = "Test",
                    Translation = "Case"
                },
                Tooltip = new BaseEntry
                {
                    Origin = "Test",
                    Translation = "Case"
                }
            };

            var nameTranslation = ModUtils.GetTranslationOfEntry(testEntry, typeof(ItemEntry).GetProperty("Name"));
            nameTranslation.Should().Be("Case");
            var tooltipTranlation = ModUtils.GetTranslationOfEntry(testEntry, typeof(ItemEntry).GetProperty("Tooltip"));
            tooltipTranlation.Should().Be("Case");
        }
    }
}
