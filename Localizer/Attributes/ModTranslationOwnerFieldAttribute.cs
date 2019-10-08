using System;

namespace Localizer.Attributes
{
    /// <summary>
    /// Indicates the field contains objects those have ModTranslations need to be localized.
    /// </summary>
    public class ModTranslationOwnerFieldAttribute : Attribute
    {
        public ModTranslationOwnerFieldAttribute(string fieldName)
        {
            FieldName = fieldName;
        }

        public string FieldName { get; }
    }
}
