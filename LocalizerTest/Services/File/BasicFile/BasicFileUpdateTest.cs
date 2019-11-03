using System.Collections.Generic;
using FluentAssertions;
using Localizer.DataModel.Default;
using Localizer.Services.File;
using LocalizerTest.NonTest;
using Xunit;

namespace LocalizerTest.Services.File
{
    public class BasicFileUpdateTest
    {
        [Fact]
        public void UpdateFile_Correct()
        {
            var service = new BasicFileUpdate<BasicItemFile>();
            
            var logger = new UpdateLogger();
            
            var oldFile = new BasicItemFile()
            {
                Items = new Dictionary<string, ItemEntry>()
                {
                    { 
                        "Key1", new ItemEntry()
                        {
                            Name = new BaseEntry()
                            {
                                Origin = "NameOrigin1",
                                Translation = "NameTranslation1"
                            },
                            Tooltip = new BaseEntry()
                            {
                                Origin = "TooltipOrigin1",
                                Translation = "TooltipTranslation1"
                            },
                        }
                    },
                    { 
                        "Key2", new ItemEntry()
                        {
                            Name = new BaseEntry()
                            {
                                Origin = "NameOrigin2",
                                Translation = "NameTranslation2"
                            },
                            Tooltip = new BaseEntry()
                            {
                                Origin = "TooltipOrigin2",
                                Translation = "TooltipTranslation2"
                            },
                        }
                    },
                    { 
                        "Key3", new ItemEntry()
                        {
                            Name = new BaseEntry()
                            {
                                Origin = "NameOrigin3",
                                Translation = "NameTranslation3"
                            },
                            Tooltip = new BaseEntry()
                            {
                                Origin = "TooltipOrigin3",
                                Translation = "TooltipTranslation3"
                            },
                        }
                    },
                }
            };

            var newFile = new BasicItemFile()
            {
                Items = new Dictionary<string, ItemEntry>()
                {
                    {
                        "Key1", new ItemEntry()
                        {
                            Name = new BaseEntry()
                            {
                                Origin = "AnotherNameOrigin1",
                                Translation = "AnotherNameTranslation1"
                            },
                            Tooltip = new BaseEntry()
                            {
                                Origin = "AnotherTooltipOrigin1",
                                Translation = "AnotherTooltipTranslation1"
                            },
                        }
                    },
                    {
                        "Key3", new ItemEntry()
                        {
                            Name = new BaseEntry()
                            {
                                Origin = "NameOrigin3",
                                Translation = "AnotherNameTranslation3"
                            },
                            Tooltip = new BaseEntry()
                            {
                                Origin = "TooltipOrigin3",
                                Translation = "AnotherTooltipTranslation3"
                            },
                        }
                    },
                    {
                        "Key4", new ItemEntry()
                        {
                            Name = new BaseEntry()
                            {
                                Origin = "NameOrigin4",
                                Translation = "NameTranslation4"
                            },
                            Tooltip = new BaseEntry()
                            {
                                Origin = "TooltipOrigin4",
                                Translation = "TooltipTranslation4"
                            },
                        }
                    },
                }
            };

            service.Update(oldFile, newFile, logger);

            logger.Added.Count.Should().Be(1);
            logger.Changed.Count.Should().Be(2);
            logger.Removed.Count.Should().Be(1);

            oldFile.Items.Count.Should().Be(4);
            oldFile.Items["Key1"].Name.Origin.Should().Be("AnotherNameOrigin1");
            oldFile.Items["Key1"].Name.Translation.Should().Be("NameTranslation1");
            oldFile.Items["Key1"].Tooltip.Origin.Should().Be("AnotherTooltipOrigin1");
            oldFile.Items["Key1"].Tooltip.Translation.Should().Be("TooltipTranslation1");
            oldFile.Items["Key3"].Name.Origin.Should().Be("NameOrigin3");
            oldFile.Items["Key3"].Name.Translation.Should().Be("NameTranslation3");
            oldFile.Items["Key3"].Tooltip.Origin.Should().Be("TooltipOrigin3");
            oldFile.Items["Key3"].Tooltip.Translation.Should().Be("TooltipTranslation3");
            oldFile.Items["Key4"].Name.Origin.Should().Be("NameOrigin4");
            oldFile.Items["Key4"].Name.Translation.Should().Be("NameTranslation4");
            oldFile.Items["Key4"].Tooltip.Origin.Should().Be("TooltipOrigin4");
            oldFile.Items["Key4"].Tooltip.Translation.Should().Be("TooltipTranslation4");
        }
        
        [Fact]
        public void UpdateEntry_Correct()
        {
            var service = new BasicFileUpdate<BasicItemFile>();
            var logger1 = new UpdateLogger();

            var oldEntry1 = new ItemEntry()
            {
                Name = new BaseEntry()
                {
                    Origin = "NameOrigin",
                    Translation = "NameTranslation"
                },
                Tooltip = new BaseEntry()
                {
                    Origin = "TooltipOrigin",
                    Translation = "TooltipTranslation"
                },
            };
            
            var newEntry1 = new ItemEntry()
            {
                Name = new BaseEntry()
                {
                    Origin = "AnotherNameOrigin",
                    Translation = "AnotherNameTranslation"
                },
                Tooltip = new BaseEntry()
                {
                    Origin = "AnotherTooltipOrigin",
                    Translation = "AnotherTooltipTranslation"
                },
            };

            service.UpdateEntry("Key", oldEntry1, newEntry1, logger1);
            
            logger1.Changed.Count.Should().Be(2);
            oldEntry1.Name.Origin.Should().Be("AnotherNameOrigin");
            oldEntry1.Name.Translation.Should().Be("NameTranslation");
            oldEntry1.Tooltip.Origin.Should().Be("AnotherTooltipOrigin");
            oldEntry1.Tooltip.Translation.Should().Be("TooltipTranslation");
            
            
            var logger2 = new UpdateLogger();

            var oldEntry2 = new ItemEntry()
            {
                Name = new BaseEntry()
                {
                    Origin = "NameOrigin",
                    Translation = "NameTranslation"
                },
                Tooltip = new BaseEntry()
                {
                    Origin = "TooltipOrigin",
                    Translation = "TooltipTranslation"
                },
            };
            
            var newEntry2 = new ItemEntry()
            {
                Name = new BaseEntry()
                {
                    Origin = "NameOrigin",
                    Translation = "AnotherNameTranslation"
                },
                Tooltip = new BaseEntry()
                {
                    Origin = "TooltipOrigin",
                    Translation = "AnotherTooltipTranslation"
                },
            };
            
            service.UpdateEntry("Key", oldEntry2, newEntry2, logger2);
            
            logger2.Changed.Count.Should().Be(0);
            oldEntry2.Name.Origin.Should().Be("NameOrigin");
            oldEntry2.Name.Translation.Should().Be("NameTranslation");
            oldEntry2.Tooltip.Origin.Should().Be("TooltipOrigin");
            oldEntry2.Tooltip.Translation.Should().Be("TooltipTranslation");
        }
    }
}
