using System;

namespace Localizer.DataModel.Default
{
    public class GitHubUpdateInfo : IUpdateInfo
    {
        public UpdateType Type { get; }

        public Version Version { get; }

        private string versionString;

        public GitHubUpdateInfo(string versionString)
        {
            this.versionString = versionString;
            Type = ParseType(versionString[0]);
            Version = Version.Parse(versionString.Substring(1, versionString.Length - 1));
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
