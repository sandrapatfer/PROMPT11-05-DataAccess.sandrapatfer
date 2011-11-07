namespace Mod05_ChelasDAL.DataAttributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class PrimaryKeyAttribute : Attribute
    {
        public string ColumnName { get; private set; }

        public PrimaryKeyAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}