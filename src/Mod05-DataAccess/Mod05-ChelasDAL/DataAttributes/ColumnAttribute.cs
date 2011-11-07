namespace Mod05_ChelasDAL.DataAttributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ColumnAttribute : Attribute
    {
        public string ColumnName { get; private set; }

        public ColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}