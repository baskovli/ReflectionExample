using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using static RefelectionExample.AttributeExtensions;

namespace RefelectionExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var data = new List<ReflectionClass>
            {
                new ReflectionClass { Id  = 1, Name = "avb afafsa 3223 sdfs", Price = 61.23345 },
                new ReflectionClass { Id  = 2, Price = 3.32 },
                new ReflectionClass { Id  = 3, Name = "rew", Price = 49.57553 },
            };

            var classType = new ReflectionClass();

            Type myType = classType.GetType();

            var props = new List<PropertyInfo>(myType.GetProperties());

            List<PropertyInfoModel> propertyListInuse = new();

            props.ForEach((item) =>
            {
                var existsHeaderAttribute = item.GetCustomAttributes(false).OfType<UseHeaderAttribute>().Any();
                if (existsHeaderAttribute)
                {
                    propertyListInuse.Add(GetAttributeByType(item));
                }
            });

            StringBuilder str = new StringBuilder();
            string seperator = ";";
            foreach (var item in data)
            {
                string val = string.Empty;

                foreach (var prop in propertyListInuse)
                {
                    var value = GetValueByTypeAndAttribute(prop, prop.PropertyInfo.GetValue(item, null));
                    val += $"{value}{seperator}";
                }
                val = val.Remove(val.Length - seperator.Length);

                str.AppendLine(val);
            }

            Console.WriteLine(str);
        }

        public static PropertyInfoModel GetAttributeByType(PropertyInfo property)
        {
            var model = new PropertyInfoModel
            {
                PropertyInfo = property,
            };

            switch (property.PropertyType.Name)
            {
                case nameof(String):
                    var maxLengthAttr = (MaxLengthAttribute)property.GetCustomAttribute(typeof(MaxLengthAttribute));
                    model.FirstNum = maxLengthAttr?.Length;
                    break;
                case nameof(Int64):
                case nameof(Int32):
                case nameof(Int16):
                    var rangeAttr = (RangeAttribute)property.GetCustomAttribute(typeof(RangeAttribute));
                    if (rangeAttr != null)
                    {
                        model.FirstNum = (int)rangeAttr.Minimum;
                        model.SecondNum = (int)rangeAttr.Maximum;
                    }
                    break;
                case nameof(Decimal):
                case nameof(Double):
                    var doubleAttr = (DoubleLengthAttribute)property.GetCustomAttribute(typeof(DoubleLengthAttribute));
                    if (doubleAttr != null)
                    {
                        model.FirstNum = doubleAttr.Length;
                        model.SecondNum = doubleAttr.AfteDot;
                    }
                    break;
                default:
                    break;
            }
            return model;
        }

        public static string GetValueByTypeAndAttribute(PropertyInfoModel propertyData, object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            switch (propertyData.PropertyInfo.PropertyType.Name)
            {
                case nameof(String):
                    var strValue = value.ToString();
                    if (propertyData.FirstNum.HasValue)
                    {
                        if (strValue.Length > propertyData.FirstNum)
                        {
                            var result = strValue.Substring(0, propertyData.FirstNum.Value);
                            strValue = Regex.Replace(result, @"\r\n?|\n", "");
                        }
                    }
                    return strValue;
                case nameof(Int64):
                case nameof(Int32):
                case nameof(Int16):
                    return value.ToString();// Have to think about. We cannot change int values...
                case nameof(Decimal):
                case nameof(Double):
                    var valueDouble = (double)value;
                    if (valueDouble == 0)
                    {
                        return string.Empty;
                    }
                    if (propertyData.FirstNum.HasValue && propertyData.SecondNum.HasValue)
                    {
                        var aa = valueDouble.ToString($"F{propertyData.SecondNum}", CultureInfo.InvariantCulture);
                        return valueDouble.ToString($"F{propertyData.SecondNum}", CultureInfo.InvariantCulture).Replace(",", ".");

                    }
                    return valueDouble.ToString("F", CultureInfo.InvariantCulture).Replace(",", ".");
                case nameof(DateTime):
                    return ((DateTime)value).ToString("yyyyMMdd");
                default:
                    return string.Empty;
            }
        }
    }

    public class ReflectionClass
    {
        [UseHeader]
        [Range(1, 100)]
        public int Id { get; set; }
        [UseHeader, DoubleLength(5, 2)]
        public double Price { get; set; }
        [UseHeader]
        [MaxLength(6)]
        public string Name { get; set; }
    }

    public class PropertyInfoModel
    {
        public PropertyInfo PropertyInfo { get; set; }
        public int? FirstNum { get; set; }
        public int? SecondNum { get; set; }
    }
}
