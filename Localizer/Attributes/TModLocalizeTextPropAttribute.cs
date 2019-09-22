using System;

namespace Localizer.Attributes
{
    public class TModLocalizeTextPropAttribute : Attribute
    {
        public TModLocalizeTextPropAttribute(string propName)
        {
            PropName = propName;
        }

        public string PropName { get; }
    }
}
