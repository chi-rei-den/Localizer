using System.Collections.Generic;
using FluentAssertions;
using Localizer.DataModel.Default;
using Localizer.Package.Update;
using LocalizerTest.NonTest;
using Xunit;

namespace LocalizerTest.Package.Update
{
    public class LdstrFileUpdateTest
    {
        [Fact]
        public void Update_Correct()
        {
            var service = new LdstrFileUpdater();

            var logger = new UpdateLogger();
            
            var oldFile = new LdstrFile()
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
            
            var newFile = new LdstrFile()
            {
                LdstrEntries = new Dictionary<string, LdstrEntry>()
                {
                    { "Key1", new LdstrEntry()
                        {
                            Instructions = new List<BaseEntry>()
                            {
                                new BaseEntry(){ Origin = "Origin1", Translation = ""},
                                new BaseEntry(){ Origin = "AnotherOrigin1", Translation = ""},
                                new BaseEntry(){ Origin = "Origin5", Translation = ""},
                            }
                        }
                    },
                    { "Key3", new LdstrEntry()
                        {
                            Instructions = new List<BaseEntry>()
                            {
                                new BaseEntry(){ Origin = "Origin6", Translation = ""}
                            }
                        }
                    },
                }
            };

            service.Update(oldFile, newFile, logger);

            logger.Added.Count.Should().Be(1);
            logger.Changed.Count.Should().Be(2);
            logger.Added.Count.Should().Be(1);

            oldFile.LdstrEntries.Count.Should().Be(3);

            oldFile.LdstrEntries["Key1"].Instructions.Count.Should().Be(4);
            oldFile.LdstrEntries["Key1"].Instructions.Should().ContainSingle(i => i.Origin == "Origin1" && i.Translation == "Translation1");
            oldFile.LdstrEntries["Key1"].Instructions.Should().ContainSingle(i => i.Origin == "Origin2" && i.Translation == "Translation2");
            oldFile.LdstrEntries["Key1"].Instructions.Should().ContainSingle(i => i.Origin == "AnotherOrigin1" && i.Translation == "");
            oldFile.LdstrEntries["Key1"].Instructions.Should().ContainSingle(i => i.Origin == "Origin5" && i.Translation == "");
            
            oldFile.LdstrEntries["Key2"].Instructions.Count.Should().Be(2);
            oldFile.LdstrEntries["Key2"].Instructions.Should().ContainSingle(i => i.Origin == "Origin3" && i.Translation == "Translation3");
            oldFile.LdstrEntries["Key2"].Instructions.Should().ContainSingle(i => i.Origin == "Origin4" && i.Translation == "Translation4");
            
            oldFile.LdstrEntries["Key3"].Instructions.Count.Should().Be(1);
            oldFile.LdstrEntries["Key3"].Instructions.Should().ContainSingle(i => i.Origin == "Origin6" && i.Translation == "");
        }
    }
}
