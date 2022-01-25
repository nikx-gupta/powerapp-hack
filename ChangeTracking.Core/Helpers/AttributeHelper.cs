using System;
using Microsoft.Extensions.Configuration;

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
                throw new Exception($"{typeof(TAttribute).Name} is required on Model {typeof(TModel).Name}");
            }

            return attribute;
        }
    }

    public static class ConfigurationHelper
    {
        public static TModel GetSettings<TModel>(this IConfiguration config)
        {
            var objInst = Activator.CreateInstance<TModel>();
            config.GetSection(typeof(TModel).Name).Bind(objInst);
            return objInst;
        }
    }
}