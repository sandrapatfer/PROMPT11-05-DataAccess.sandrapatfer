using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DataMapper
{
    public class ColumnMetadata
    {
        private PropertyInfo m_propertyInfo;
        public string Name { get { return m_propertyInfo.Name; } }

        public static ColumnMetadata Create(PropertyInfo pi)
        {
            // TODO read properties
            if (pi.PropertyType == typeof(string))
            {
                return new ColumnMetadata() { m_propertyInfo = pi };
            }
            return null;
        }

        void SetValue(object obj, object value)
        {
            // TODO check type
            // for now, only strings
            m_propertyInfo.SetValue(obj, value, null);
        }
    }
}
