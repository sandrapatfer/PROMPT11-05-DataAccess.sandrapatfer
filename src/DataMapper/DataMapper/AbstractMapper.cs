using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Reflection;

namespace DataMapper
{
    public class AbstractMapper<T>
        where T:new()
    {
        private static TableMetadata m_typeMap;
        private static Dictionary<int, T> m_map = new Dictionary<int,T>();
        private Type m_type;
        private string m_selectStatement;
        private SqlConnection m_sqlConnection;
        private SqlConnection SqlConnection
        {
            get
            {
                if (m_sqlConnection == null)
                { }
                return m_sqlConnection;
            }
        }

        static AbstractMapper()
        {
            m_typeMap = new TableMetadata(typeof(T));
        }

        public AbstractMapper()
        {
        }

        public IQueryable<T> GetAll()
        {
            var select = new SqlCommand(m_typeMap.SelectStatement, SqlConnection);
            var rows = select.ExecuteReader();
            List<object> objects = new List<object>();
            while (rows.Read())
            {
                object obj = new T();
                m_typeMap.ConvertAllColumns(obj, rows);
                objects.Add(obj);
            }
            return objects.AsQueryable();
        }

        public T Get(int id)
        {
            if (m_map.ContainsKey(id))
            {
                return m_map[id];
            }
            else
            {
                m_selectStatement = string.Format("SELECT {0} FROM {1} WHERE id = {2}", m_type.GetProperties());
            }
        }

        public void Insert(T t)
        {

        }
    }
}
