﻿namespace Mod05_ChelasDAL.Metadata
{
    using System;
    using System.Reflection;

    public class ReferenceInfo : ColumnInfo
    {
        public Type ReferenceType { get; private set; }

        public ReferenceInfo(MetaDataStore store, string name, Type referenceType, PropertyInfo propertyInfo)
            : base(store, name, store.GetTableInfoFor(referenceType).PrimaryKey.DotNetType,
                    store.GetTableInfoFor(referenceType).PrimaryKey.DbType, propertyInfo)
        {
            ReferenceType = referenceType;
        }
    }
}