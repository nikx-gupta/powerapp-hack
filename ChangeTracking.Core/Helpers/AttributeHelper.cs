using System;
using System.Linq;
using System.Reflection;
using ChangeTracking.Entities;
using Microsoft.Extensions.Configuration;

namespace ChangeTracking.Core.Helpers
{
    public static class AttributeHelper
    {
        public static TAttribute GetAttributeName<TAttribute, TModel>(bool isRequired = false)
            where TAttribute : Attribute
        {
            var attribute = typeof(TModel).GetCustomAttribute<TAttribute>();
            if (isRequired && attribute == null)
            {
                throw new Exception($"{typeof(TAttribute).Name} is required on Model {typeof(TModel).Name}");
            }

            return attribute;
        }

        public static string OnProperty<TAttribute, TModel>(bool isRequired = false)
            where TAttribute : ChangeTrackingAttribute
        {
            var props = typeof(TModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var prop = props.FirstOrDefault(x => Attribute.IsDefined(x, typeof(TAttribute)));
            var attribute = prop?.GetCustomAttribute<TAttribute>();
            if (isRequired && attribute == null)
            {
                throw new Exception($"{typeof(TAttribute).Name} is required on Model {typeof(TModel).Name}");
            }

            return string.IsNullOrEmpty(attribute?.Name) ? prop?.Name : attribute.Name;
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