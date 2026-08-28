using System.ComponentModel;

namespace FundManager.Common.Helper
{
    public static class EnumHepler
    {
        public static T GetValueFromDescription<T>(string description)
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null)!;
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null)!;
                }
            }
            return default!;
            //throw new Exception("Not found.", nameof(description));
        }

        public static string GetDescriptionFromEnum<T>(this T? enumValue)
        {
            if (enumValue == null)
                return string.Empty;

            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : enumValue.ToString();
        }

        public static List<string> GetAllDescriptions<T>()
        {
            var descriptions = new List<string>();
            var fields = typeof(T).GetFields();
            foreach (var field in fields)
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    descriptions.Add(attribute.Description);
                }
                else
                {
                    descriptions.Add(field.Name);
                }
            }
            return descriptions;
        }
    }
}