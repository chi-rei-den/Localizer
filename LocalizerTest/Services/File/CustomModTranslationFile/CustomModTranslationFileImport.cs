using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Localizer.DataModel.Default;
using Localizer.Services.File;
using Xunit;

namespace LocalizerTest.Services.File
{
    public class CustomModTranslationFileImportTest
    {
        [Fact]
        public void Merge_Correct()
        {
            var service = new CustomModTranslationFileImport();
            
            var main = new CustomModTranslationFile()
            {
                Translations = new Dictionary<string, BaseEntry>()
                {
                    { "Key1", new BaseEntry(){ Origin = "Origin1", Translation = "Translation1" } },
                    { "Key2", new BaseEntry(){ Origin = "Origin2", Translation = "Translation2" } },
                    { "Key3", new BaseEntry(){ Origin = "Origin3", Translation = "Translation3" } },
                }
            };
            
            var addition = new CustomModTranslationFile()
            {
                Translations = new Dictionary<string, BaseEntry>()
                {
                    { "Key1", new BaseEntry(){ Origin = "Origin1", Translation = "AnotherTranslation1" } },
                    { "Key4", new BaseEntry(){ Origin = "Origin4", Translation = "Translation4" } },
                }
            };

            var result = service.Merge(main, addition);

            result.Translations.Count.Should().Be(4);
            result.Translations["Key1"].Translation.Should().Be("Translation1");
            result.Translations["Key4"].Origin.Should().Be("Origin4");
            result.Translations["Key4"].Translation.Should().Be("Translation4");
        }
    }
}
