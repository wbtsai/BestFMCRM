using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace BestRadioNet.Core.Logic
{
    public class ReflectionLogic
    {
        /// <summary>
        /// Get PropertyList
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        public List<PropertyInfo> GetPropertyList(Type objType)
        {
            return RetrievePropertyInfo(objType);
        }

        /// <summary>
        /// Get EntityT PropertyList
        /// </summary>
        /// <typeparam name="EntityT"></typeparam>
        /// <returns></returns>
        public List<PropertyInfo> GetPropertyList<EntityT>()
        {
            return RetrievePropertyInfo(typeof(EntityT));
        }


        private List<PropertyInfo> RetrievePropertyInfo(Type objType)
        {
            List<PropertyInfo> propList = new List<PropertyInfo>(objType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

            return propList;
        }

        public List<T> RetrieveAttribute<T>(Type type, AttributeTargets target)
        {
            return RetrieveAttribute<T>(type, target, "");
        }

        public List<ConstructorInfo> RetriveConstructorInfo(Type objType)
        {
            List<ConstructorInfo> constructorList = new List<ConstructorInfo>(objType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

            return constructorList;
        }

        /// <summary>
        /// 取得AttrT 的資料值
        /// </summary>
        /// <typeparam name="T">Attribute T</typeparam>
        /// <param name="type">需要擷取的Type</param>
        /// <param name="target">class,field,constructor</param>
        /// <param name="name">針對那一個名稱</param>
        /// <returns></returns>
        public List<AttrT> RetrieveAttribute<AttrT>(Type type, AttributeTargets target, string name)
        {
            List<AttrT> list = new List<AttrT>();

            object[] attrs = null;

            switch (target)
            {
                case AttributeTargets.Class:
                    attrs = type.GetCustomAttributes(typeof(AttrT), false);
                    break;
                case AttributeTargets.Property:
                    attrs = type.GetProperty(name).GetCustomAttributes(typeof(AttrT), false);
                    break;
                case AttributeTargets.Method:
                    attrs = type.GetMethod(name).GetCustomAttributes(typeof(AttrT), false);
                    break;
                default:
                    throw new Exception("尚無實作此類型的做法");
            }

            foreach (object t in attrs)
            {
                list.Add((AttrT)t);
            }

            return list;
        }
    }
}
