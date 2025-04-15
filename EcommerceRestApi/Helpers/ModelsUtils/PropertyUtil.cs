using EcommerceRestApi.Models;
using Inventory_Management_Sustem.Models.Dtos;
using System.Reflection;

namespace EcommerceRestApi.Helpers.ModelsUtils
{
    public static class PropertyUtil
    {
        /// <summary>
        /// Extension method for copying property values from one object to another. 
        /// Property must have the same type and name in order to be copied.
        /// </summary>
        /// <typeparam name="TargetType">Target type to which values will be copied.</typeparam>
        /// <typeparam name="SourceType">Source type from which values will be copied.</typeparam>
        /// <param name="targetObject">Target object to which values will be copied.</param>
        /// <param name="sourceObject">Source object from which values will be copied.</param>
        /// <returns>Target object.</returns>
        public static T CopyProperties<T, T2>(this T targetObject, T2 sourceObject)
        {
            targetObject.GetTypeProperties().Where(p => p.CanWrite).ToList()
                .ForEach(property => FindAndReplaceProperty(targetObject, sourceObject, property));
            return targetObject;
        }
        private static void FindAndReplaceProperty<T, T2>(T targetObject, T2 sourceObject, PropertyInfo property)
        {
            if (IsNavigationProperty(property))
                return;

            if (sourceObject.GetTypeProperties().Any(prop => CheckIfPropertyExistInSource(prop, property)))
            {
                property.SetValue(targetObject, sourceObject.GetPropertyValue(property.Name), default);
            }
        }

        private static IEnumerable<PropertyInfo> GetTypeProperties<T2>(this T2 sourceObject)
            => sourceObject.GetType().GetProperties();

        private static bool CheckIfPropertyExistInSource(PropertyInfo prop, PropertyInfo property)
        {
            return string.Equals(property.Name, prop.Name, StringComparison.InvariantCultureIgnoreCase)
                && prop.PropertyType.Equals(property.PropertyType)
                && !IsNavigationProperty(property);
        }

        private static bool IsNavigationProperty(PropertyInfo property)
        {
            // Skip properties that are classes and not string
            return property.PropertyType.IsClass
                && property.PropertyType != typeof(string);
        }


        private static object GetPropertyValue<T>(this T source, string propertyName)
            => source.GetType().GetProperty(propertyName).GetValue(source, null);

        public static ProductDto ToDto(Product product)
        {
            var dto = new ProductDto().CopyProperties(product);
            dto.SubcategoryCode = product.Subcategory?.Code ?? string.Empty;
            dto.CategoryCode = product.ProductCategories.FirstOrDefault()?.Category?.Code ?? string.Empty;
            dto.Reviews = product.Reviews?.Select(ReviewDto.FromEntity).ToList() ?? new();
            return dto;
        }
    }
}
