using System;

namespace Localizer.DataModel.Default
{
    public class GitHubUpdateInfo : IUpdateInfo
    {
        public UpdateType Type => type;

        public Version Version => version;

        private UpdateType type;
        private Version version;

        private string versionString;

        public GitHubUpdateInfo(string versionString)
        {
            this.versionString = versionString;
            
            Parse();
        }

        internal void Parse()
        {
            type = ParseType(versionString[0]);

            version = Version.Parse(versionString.Substring(1, versionString.Length - 1));
        }

        internal UpdateType ParseType(char t)
        {
            switch (t)
            {
                case 'a':
                    return UpdateType.Minor;
                case 'b':
                    return UpdateType.Major;
                case 'c':
                    return UpdateType.Critical;
                default:
                    return UpdateType.None;
            }
        }
    }
}
