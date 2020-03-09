using System.Collections.Generic;
using FluentAssertions;
using Localizer.DataModel.Default;
using Localizer.Package.Update;
using LocalizerTest.NonTest;
using Xunit;

namespace LocalizerTest.Package.Update
{
    public class CustomModTranslationFileUpdateTest
    {
        [Fact]
        public void Update_Correct()
        {
            var service = new CustomModTranslationUpdater();
            var logger = new UpdateLogger();

            var oldFile = new CustomModTranslationFile()
            {
                Translations = new Dictionary<string, BaseEntry>()
                {
                    { "Key1", new BaseEntry(){ Origin = "Origin1", Translation = "Translation1" } },
                    { "Key2", new BaseEntry(){ Origin = "Origin2", Translation = "Translation2" } },
                    { "Key3", new BaseEntry(){ Origin = "Origin3", Translation = "Translation3" } },
                }
            };

            var newFile = new CustomModTranslationFile()
            {
                Translations = new Dictionary<string, BaseEntry>()
                {
                    { "Key1", new BaseEntry(){ Origin = "AnotherOrigin1", Translation = "AnotherTranslation1" } },
                    { "Key4", new BaseEntry(){ Origin = "Origin4", Translation = "Translation4" } },
                    { "Key5", new BaseEntry(){ Origin = "Origin5", Translation = "Translation5" } },
                }
            };

            service.Update(oldFile, newFile, logger);

            logger.Added.Count.Should().Be(2);
            logger.Removed.Count.Should().Be(2);
            logger.Changed.Count.Should().Be(1);

            oldFile.Translations.Count.Should().Be(5);
            oldFile.Translations["Key1"].Origin.Should().Be("AnotherOrigin1");
            oldFile.Translations["Key1"].Translation.Should().Be("AnotherTranslation1");
            oldFile.Translations["Key2"].Origin.Should().Be("Origin2");
            oldFile.Translations["Key2"].Translation.Should().Be("Translation2");
            oldFile.Translations["Key3"].Origin.Should().Be("Origin3");
            oldFile.Translations["Key3"].Translation.Should().Be("Translation3");
            oldFile.Translations["Key4"].Origin.Should().Be("Origin4");
            oldFile.Translations["Key4"].Translation.Should().Be("Translation4");
            oldFile.Translations["Key5"].Origin.Should().Be("Origin5");
            oldFile.Translations["Key5"].Translation.Should().Be("Translation5");
        }
    }
}
