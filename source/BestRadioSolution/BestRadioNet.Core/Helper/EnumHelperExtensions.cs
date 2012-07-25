using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using BestRadioNet.Core.Attributes;

namespace BestRadioNet.Core.Helper
{
    public static class EnumHelperExtensions
    {
        public static T StringToEnum<T>(this string name)
        {
            return (T)Enum.Parse(typeof(T), name);
        }

        public static T GetAttribute<T>(this Enum @enum)
        {
            T t = default(T);

            if (@enum != null)
            {
                FieldInfo fi = @enum.GetType().GetField(@enum.ToString());

                if (fi != null)
                {
                    T[] attrs = fi.GetCustomAttributes(typeof(T), false) as T[];

                    if (attrs != null && attrs.Length > 0)
                    {
                        t = attrs[0];
                    }
                }
            }

            return t;
        }

        /// <summary>
        /// 取得Key And Value From Enum
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> EnumToKeyList(this Type type)
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();

            FieldInfo[] fields = type.GetFields();

            foreach (FieldInfo f in fields)
            {
                if (f.FieldType == type)
                {
                    string desc = f.Name;

                    object[] attrs = f.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);

                    foreach (EnumDescriptionAttribute att in attrs)
                    {
                        desc = att.Description;
                    }

                    list.Add(new KeyValuePair<string, string>(f.Name,desc));
                }
                

            }

            return list;
        }
    }
}
