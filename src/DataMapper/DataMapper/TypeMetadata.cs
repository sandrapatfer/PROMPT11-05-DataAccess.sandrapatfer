using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.SqlClient;

namespace DataMapper
{
    public class TableMetadata
    {
        public string m_tableName;
        public List<ColumnMetadata> m_columns;

        private string m_selectStatement;
        public string SelectStatement
        {
            get
            {
                if (m_selectStatement == null)
                {
                    m_selectStatement = string.Format("SELECT {0} FROM {1}", m_columns.Select(c => c.Name).Aggregate((a, b) => a + ',' + b), m_tableName);
                }
                return m_selectStatement;
            }
        }

        public TableMetadata(Type t)
        {
            // TODO get from property on type
            m_tableName = t.Name;

            // only get the columns that the column metadata knows how to convert
            m_columns = new List<ColumnMetadata>();
            var props = t.GetProperties().GetEnumerator();
            while (props.MoveNext())
            {
                var col = ColumnMetadata.Create((PropertyInfo)props.Current);
                if (col != null)
                {
                    m_columns.Add(col);
                }
            }
        }

        public void ConvertAllColumns(object obj, SqlDataReader rows)
        {
            while (rows.Read())
            {
                int index = 0;
                foreach (var col in m_columns)
                {
                    // QUESTION matching indexes is ok?
                    col.SetValue(obj, rows[index]);
                    ++index;
                }
            }
        }
    }
}
