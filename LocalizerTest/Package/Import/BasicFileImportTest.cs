using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Localizer.DataModel.Default;
using Localizer.Package.Import;
using Xunit;

namespace LocalizerTest.Package.Import
{
    public class BasicFileImportTest
    {
        [Fact]
        public void MergeFile_Correct()
        {
            var importer = new BasicImporter<BasicItemFile>();

            var main = new BasicItemFile()
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
                                Translation = ""
                            },
                        }
                    },
                    {
                        "Key3", new ItemEntry()
                        {
                            Name = new BaseEntry()
                            {
                                Origin = "NameOrigin3",
                                Translation = ""
                            },
                            Tooltip = new BaseEntry()
                            {
                                Origin = "TooltipOrigin3",
                                Translation = ""
                            },
                        }
                    },
                }
            };

            var addition = new BasicItemFile()
            {
                Items = new Dictionary<string, ItemEntry>()
                {
                    {
                        "Key1", new ItemEntry()
                        {
                            Name = new BaseEntry()
                            {
                                Origin = "NameOrigin1",
                                Translation = "AnotherNameTranslation1"
                            },
                            Tooltip = new BaseEntry()
                            {
                                Origin = "TooltipOrigin1",
                                Translation = "AnotherTooltipTranslation1"
                            },
                        }
                    },
                    {
                        "Key2", new ItemEntry()
                        {
                            Name = new BaseEntry()
                            {
                                Origin = "NameOrigin2",
                                Translation = ""
                            },
                            Tooltip = new BaseEntry()
                            {
                                Origin = "TooltipOrigin2",
                                Translation = "AnotherTooltipTranslation2"
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

            var result = importer.MergeInternal(main, addition);

            result.Items.Count.Should().Be(4);

            result.Items["Key1"].Name.Translation.Should().Be("NameTranslation1");
            result.Items["Key1"].Tooltip.Translation.Should().Be("TooltipTranslation1");

            result.Items["Key4"].Name.Origin.Should().Be("NameOrigin4");
            result.Items["Key4"].Name.Translation.Should().Be("NameTranslation4");
            result.Items["Key4"].Tooltip.Origin.Should().Be("TooltipOrigin4");
            result.Items["Key4"].Tooltip.Translation.Should().Be("TooltipTranslation4");

            result.Items["Key2"].Name.Translation.Should().Be("NameTranslation2");
            result.Items["Key2"].Tooltip.Translation.Should().Be("AnotherTooltipTranslation2");

            result.Items["Key3"].Name.Translation.Should().Be("AnotherNameTranslation3");
            result.Items["Key3"].Tooltip.Translation.Should().Be("AnotherTooltipTranslation3");
        }

        [Fact]
        public void MergeEntry_Correct()
        {
            var service = new BasicImporter<BasicItemFile>();

            var main1 = new ItemEntry()
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
            var addition1 = new ItemEntry()
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

            var result1 = service.Merge(main1, addition1) as ItemEntry;
            result1.Name.Translation.Should().Be("NameTranslation");
            result1.Tooltip.Translation.Should().Be("TooltipTranslation");

            var main2 = new ItemEntry()
            {
                Name = new BaseEntry()
                {
                    Origin = "NameOrigin",
                    Translation = ""
                },
                Tooltip = new BaseEntry()
                {
                    Origin = "TooltipOrigin",
                    Translation = ""
                },
            };
            var addition2 = new ItemEntry()
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

            var result2 = service.Merge(main2, addition2) as ItemEntry;
            result2.Name.Translation.Should().Be("AnotherNameTranslation");
            result2.Tooltip.Translation.Should().Be("AnotherTooltipTranslation");


            var main3 = new ItemEntry()
            {
                Name = new BaseEntry()
                {
                    Origin = "NameOrigin",
                    Translation = "NameTranslation"
                },
                Tooltip = new BaseEntry()
                {
                    Origin = "TooltipOrigin",
                    Translation = ""
                },
            };
            var addition3 = new ItemEntry()
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

            var result3 = service.Merge(main3, addition3) as ItemEntry;
            result3.Name.Translation.Should().Be("NameTranslation");
            result3.Tooltip.Translation.Should().Be("AnotherTooltipTranslation");
        }
    }
}
