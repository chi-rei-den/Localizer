using System;

namespace Localizer.Attributes
{
    /// <summary>
    /// Indicates the name of ModTranslation property.
    /// </summary>
    public class ModTranslationPropAttribute : Attribute
    {
        public ModTranslationPropAttribute(string propName)
        {
            PropName = propName;
        }

        public string PropName { get; }
    }
}
