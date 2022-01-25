using System;

namespace ChangeTracking.Core.Helpers
{
    public static class AttributeHelper
    {
        public static TAttribute GetAttributeName<TAttribute, TModel>(bool isRequired = false)
            where TAttribute : Attribute
        {
            var attribute = (TAttribute) Attribute.GetCustomAttribute(typeof(TModel), typeof(TAttribute))!;
            if (isRequired && attribute == null)
            {
                throw new Exception($"Attribute is required on Model {typeof(TModel).Name}");
            }

            return attribute;
        }
    }
}