using System.Collections.Generic;
using FluentAssertions;
using Localizer.DataModel.Default;
using Localizer.Package.Import;
using Xunit;

namespace LocalizerTest.Package.Import
{
    public class LdstrFileImportBaseTest
    {
        [Fact]
        public void MergeEntry_Correct()
        {
            var service = new HarmonyLdstrImporter();

            var main = new LdstrEntry()
            {
                Instructions = new List<BaseEntry>()
                {
                    new BaseEntry()
                    {
                        Origin = "Origin1",
                        Translation = "Translation1"
                    },
                    new BaseEntry()
                    {
                        Origin = "Origin2",
                        Translation = "Translation2"
                    },
                }
            };

            var addition = new LdstrEntry()
            {
                Instructions = new List<BaseEntry>()
                {
                    new BaseEntry()
                    {
                        Origin = "Origin1",
                        Translation = "AnotherTranslation1"
                    },
                    new BaseEntry()
                    {
                        Origin = "AnotherOrigin2",
                        Translation = "Translation2"
                    },
                    new BaseEntry()
                    {
                        Origin = "Origin3",
                        Translation = "Translation3"
                    },
                }
            };

            var result = service.Merge(main, addition);

            result.Instructions.Count.Should().Be(4);
            
            result.Instructions.Should().ContainSingle(i => i.Origin == "Origin1" && i.Translation == "Translation1");
            result.Instructions.Should().ContainSingle(i => i.Origin == "Origin2" && i.Translation == "Translation2");
            result.Instructions.Should().ContainSingle(i => i.Origin == "AnotherOrigin2" && i.Translation == "Translation2");
            result.Instructions.Should().ContainSingle(i => i.Origin == "Origin3" && i.Translation == "Translation3");
        }
        
        [Fact]
        public void MergeFile_Correct()
        {
            var importer = new HarmonyLdstrImporter();
            
            var main = new LdstrFile()
            {
                LdstrEntries = new Dictionary<string, LdstrEntry>()
                {
                    { "Key1", new LdstrEntry()
                        {
                            Instructions = new List<BaseEntry>()
                            {
                                new BaseEntry(){ Origin = "Origin1", Translation = "Translation1"},
                                new BaseEntry(){ Origin = "Origin2", Translation = "Translation2"},
                            }
                        }
                    },
                    { "Key2", new LdstrEntry()
                        {
                            Instructions = new List<BaseEntry>()
                            {
                                new BaseEntry(){ Origin = "Origin3", Translation = "Translation3"},
                                new BaseEntry(){ Origin = "Origin4", Translation = "Translation4"},
                            }
                        }
                    },
                }
            };
            
            var addition = new LdstrFile()
            {
                LdstrEntries = new Dictionary<string, LdstrEntry>()
                {
                    { "Key1", new LdstrEntry()
                        {
                            Instructions = new List<BaseEntry>()
                            {
                                new BaseEntry(){ Origin = "Origin1", Translation = "AnotherTranslation1"},
                                new BaseEntry(){ Origin = "AnotherOrigin1", Translation = "Translation1"},
                                new BaseEntry(){ Origin = "Origin5", Translation = "Translation5"},
                            }
                        }
                    },
                    { "Key3", new LdstrEntry()
                        {
                            Instructions = new List<BaseEntry>()
                            {
                                new BaseEntry(){ Origin = "Origin6", Translation = "Translation6"}
                            }
                        }
                    },
                }
            };

            var result = importer.MergeInternal(main, addition);

            result.LdstrEntries.Count.Should().Be(3);
            
            result.LdstrEntries["Key1"].Instructions.Should().ContainSingle(i => i.Origin == "Origin1" && i.Translation == "Translation1");
            result.LdstrEntries["Key1"].Instructions.Should().ContainSingle(i => i.Origin == "AnotherOrigin1" && i.Translation == "Translation1");
            result.LdstrEntries["Key1"].Instructions.Should().ContainSingle(i => i.Origin == "Origin5" && i.Translation == "Translation5");
            
            result.LdstrEntries["Key3"].Instructions.Count.Should().Be(1);
            result.LdstrEntries["Key3"].Instructions[0].Origin.Should().Be("Origin6");
            result.LdstrEntries["Key3"].Instructions[0].Translation.Should().Be("Translation6");
        }
    }
}
