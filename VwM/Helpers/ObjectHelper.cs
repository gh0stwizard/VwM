using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace VwM.Helpers
{
    public static class ObjectHelper
    {
        private const BindingFlags defaultBindingFlags =
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.Static;


        public static PropertyInfo GetNestedProperty<T>(string propertyName, bool IgnoreCase = true)
        {
            var partType = typeof(T);
            PropertyInfo info = null;
            var flags = IgnoreCase ? defaultBindingFlags | BindingFlags.IgnoreCase : defaultBindingFlags;

            foreach (var partName in propertyName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries))
            {
                info = partType.GetProperty(partName, flags);

                if (info == null)
                    return null;

                partType = info.PropertyType;
            }

            return (info == null) ? null : info;
        }


        public static FieldInfo GetNestedField<T>(string fieldName)
        {
            var partType = typeof(T);
            FieldInfo info = null;

            foreach (var partName in fieldName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries))
            {
                info = partType.GetField(partName);

                if (info == null)
                    return null;

                partType = info.FieldType;
            }

            return (info == null) ? null : info;
        }


        public static Type GetNestedPropertyType<T>(string propertyName, bool IgnoreCase = true)
        {
            var partType = typeof(T);
            PropertyInfo info = null;
            var flags = IgnoreCase ? defaultBindingFlags | BindingFlags.IgnoreCase : defaultBindingFlags;

            foreach (var partName in propertyName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries))
            {
                info = partType.GetProperty(partName, flags);

                if (info == null)
                    return null;

                partType = info.PropertyType;
            }

            return (info == null) ? null : info.PropertyType;
        }


        public static Type GetNestedFieldType<T>(string fieldName)
        {
            var partType = typeof(T);
            FieldInfo info = null;

            foreach (var partName in fieldName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries))
            {
                info = partType.GetField(partName);

                if (info == null)
                    return null;

                partType = info.FieldType;
            }

            return (info == null) ? null : info.FieldType;
        }



        public static Type GetNestedPropertyType(Type type, string propertyName, bool IgnoreCase = true)
        {
            var partType = type;
            PropertyInfo info = null;
            var flags = IgnoreCase ? defaultBindingFlags | BindingFlags.IgnoreCase : defaultBindingFlags;

            foreach (var partName in propertyName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries))
            {
                info = partType.GetProperty(partName, flags);

                if (info == null)
                    return null;

                partType = info.PropertyType;
            }

            return (info == null) ? null : info.PropertyType;
        }


        public static Type GetNestedFieldType(Type type, string fieldName)
        {
            var partType = type;
            FieldInfo info = null;

            foreach (var partName in fieldName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries))
            {
                info = partType.GetField(partName);

                if (info == null)
                    return null;

                partType = info.FieldType;
            }

            return (info == null) ? null : info.FieldType;
        }
    }
}
