using System;

namespace Localizer.Attributes
{
    public class TModLocalizeFieldAttribute : Attribute
    {
        public TModLocalizeFieldAttribute(string fieldName)
        {
            FieldName = fieldName;
        }

        public string FieldName { get; }
    }
}
