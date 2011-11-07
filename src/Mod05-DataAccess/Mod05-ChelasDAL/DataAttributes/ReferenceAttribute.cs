namespace Mod05_ChelasDAL.DataAttributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ReferenceAttribute : Attribute
    {
        public string ColumnName { get; private set; }

        public ReferenceAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
