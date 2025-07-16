using System.Reflection;
using System;
using System.ComponentModel;

namespace VARLab.RespiratoryTherapy
{
    public static class NameFormatter
    {
        /// <summary>
        /// Takes in any enum and returns its description attribute
        /// </summary>
        /// <param name="value">Generic value (Expected to be an enum)</param>
        /// <returns>if it exists, The description attribute of the enum value</returns>
        public static string ToDescription<T>(this T value)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type!");
            }

            FieldInfo field = value.GetType().GetField(value.ToString());
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute != null ? attribute.Description : value.ToString();
        }
    }
}