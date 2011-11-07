namespace Mod05_ChelasDAL.DataAttributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class TableAttribute : Attribute
    {
        public string TableName { get; private set; }

        public TableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}