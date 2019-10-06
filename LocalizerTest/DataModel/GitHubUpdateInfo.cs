using System;
using FluentAssertions;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Xunit;

namespace LocalizerTest.DataModel
{
    public class GitHubUpdateInfoTest
    {
        [Fact]
        public void Parse_Correct()
        {
            var info = new GitHubUpdateInfo("a1.2.3.4");

            info.Type.Should().Be(UpdateType.Minor);
            info.Version.Should().BeEquivalentTo(Version.Parse("1.2.3.4"));
        }
    }
}
