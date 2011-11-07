namespace Mod05_ChelasDAL.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// This class holds all metadata to map an entity to a SQL table.
    /// </summary>
    public class TableInfo : MetaData
    {
        private readonly Dictionary<string, ColumnInfo> columns = new Dictionary<string, ColumnInfo>();

        private readonly Dictionary<string, ReferenceInfo> references = new Dictionary<string, ReferenceInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TableInfo"/> class.
        /// </summary>
        /// <param name="store">The <see cref="MetaDataStore"/>.</param>
        /// <param name="name">The table name.</param>
        /// <param name="entityType">Type of the entity.</param>
        public TableInfo(MetaDataStore store, string name, Type entityType)
            : base(store)
        {
            Name = name;
            EntityType = entityType;
        }

        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        public Type EntityType { get; private set; }

        /// <summary>
        /// Gets or sets the primary key <see cref="ColumnInfo"/>.
        /// </summary>
        /// <value>
        /// The primary key.
        /// </value>
        public ColumnInfo PrimaryKey { get; set; }


        /// <summary>
        /// Gets all entity references.
        /// </summary>
        public IEnumerable<ReferenceInfo> References
        {
            get { return references.Values; }
        }

        /// <summary>
        /// Gets all entity columns <see cref="ColumnInfo"/>.
        /// </summary>
        public IEnumerable<ColumnInfo> Columns
        {
            get { return columns.Values; }
        }


        /// <summary>
        /// Adds a column to the entity.
        /// </summary>
        /// <param name="column">The column to add.</param>
        public void AddColumn(ColumnInfo column)
        {
            if (columns.ContainsKey(column.Name))
            {
                throw new InvalidOperationException(
                    string.Format("An item with key {0} has already been added", column.Name));
            }

            columns.Add(column.Name, column);
        }

        /// <summary>
        /// Adds a reference to the entity.
        /// </summary>
        /// <param name="reference">The reference to add.</param>
        public void AddReference(ReferenceInfo reference)
        {
            if (references.ContainsKey(reference.Name))
            {
                throw new InvalidOperationException(
                    string.Format("An item with key {0} has already been added", reference.Name));
            }

            references.Add(reference.Name, reference);
        }

        /// <summary>
        /// Gets a <see cref="ColumnInfo"/> given its <paramref name="columnName"/>.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The corresponding <see cref="ColumnInfo"/></returns>
        public ColumnInfo GetColumn(string columnName)
        {
            if (!columns.ContainsKey(columnName))
            {
                throw new InvalidOperationException(
                    string.Format("The table '{0}' does not have a '{1}' column", Name, columnName));
            }

            return columns[columnName];
        }


        #region SQL commands construction
        public StringBuilder GetSelectStatementForAllFields()
        {
            StringBuilder builder = new StringBuilder("SELECT " + Escape(PrimaryKey.Name) + ", ");

            AddReferenceColumnNames(builder);
            AddRegularColumnNames(builder);
            RemoveLastCommaAndSpaceIfThereAreAnyColumns(builder);
            builder.Append(" FROM " + Escape(Name));

            return builder;
        }


        public string GetInsertStatement()
        {
            StringBuilder builder = new StringBuilder("INSERT INTO " + Escape(Name) + " (");

            AddReferenceColumnNames(builder);
            AddRegularColumnNames(builder);
            RemoveLastCommaAndSpaceIfThereAreAnyColumns(builder);
            builder.Append(") VALUES (");
            AddReferenceColumnParameterNames(builder);
            AddRegularColumnParameterNames(builder);
            RemoveLastCommaAndSpaceIfThereAreAnyColumns(builder);
            builder.Append("); SELECT SCOPE_IDENTITY();");

            return builder.ToString();
        }

        public string GetUpdateStatement()
        {
            StringBuilder builder = new StringBuilder("UPDATE " + Escape(Name) + " SET ");

            AddReferenceColumnsNameWithParameterName(builder);
            AddRegularColumnsNameWithParameterName(builder);
            RemoveLastCommaAndSpaceIfThereAreAnyColumns(builder);
            AddWhereByIdClause(builder);
            builder.Append(";");

            return builder.ToString();
        }

        public string GetDeleteStatement()
        {
            StringBuilder builder = new StringBuilder("DELETE FROM " + Escape(Name) + " ");

            AddWhereByIdClause(builder);
            builder.Append(";");

            return builder.ToString();
        }



        private void AddReferenceColumnNames(StringBuilder builder)
        {
            foreach (var referenceInfo in References)
            {
                builder.Append(Escape(referenceInfo.Name) + ", ");
            }
        }

        private void AddReferenceColumnParameterNames(StringBuilder builder)
        {
            foreach (var referenceInfo in References)
            {
                builder.Append("@" + referenceInfo.Name + ", ");
            }
        }

        private void AddReferenceColumnsNameWithParameterName(StringBuilder builder)
        {
            foreach (var referenceInfo in References)
            {
                builder.Append(Escape(referenceInfo.Name) + " = @" + referenceInfo.Name + ", ");
            }
        }

        private void AddRegularColumnNames(StringBuilder builder)
        {
            foreach (var columnInfo in Columns)
            {
                builder.Append(Escape(columnInfo.Name) + ", ");
            }
        }

        private void AddRegularColumnParameterNames(StringBuilder builder)
        {
            foreach (var columnInfo in Columns)
            {
                builder.Append("@" + columnInfo.Name + ", ");
            }
        }

        private void AddRegularColumnsNameWithParameterName(StringBuilder builder)
        {
            foreach (var columnInfo in Columns)
            {
                builder.Append(Escape(columnInfo.Name) + " = @" + columnInfo.Name + ", ");
            }
        }

        
        public StringBuilder AddWhereByIdClause(StringBuilder query)
        {
            query.Append(" WHERE " + Escape(PrimaryKey.Name) + " = " + GetPrimaryKeyParameterName());
            return query;
        }

        public string GetPrimaryKeyParameterName()
        {
            return "@" + PrimaryKey.Name;
        }

        public object GetPrimaryKeyParameterValue(object entity)
        {
            return PrimaryKey.PropertyInfo.GetValue(entity, null);
        }

        public IEnumerable<AdoParameterInfo> GetParametersForInsert(object entity)
        {
            return GetParametersForAllReferenceAndRegularColumns(entity);
        }

        public IEnumerable<AdoParameterInfo> GetParametersForUpdate(object entity)
        {
            var parameters = GetParametersForAllReferenceAndRegularColumns(entity);
            parameters.Add(
                new AdoParameterInfo(PrimaryKey.Name, PrimaryKey.DbType, PrimaryKey.PropertyInfo.GetValue(entity, null)));
            return parameters;
        }

        private List<AdoParameterInfo> GetParametersForAllReferenceAndRegularColumns(object entity)
        {
            var parameters = new List<AdoParameterInfo>();

            foreach (var referenceInfo in References)
            {
                var referencedEntity = referenceInfo.PropertyInfo.GetValue(entity, null);
                var referencePrimaryKeyProperty =
                    MetaDataStore.GetTableInfoFor(referenceInfo.ReferenceType).PrimaryKey.PropertyInfo;

                if (referencedEntity == null)
                {
                    parameters.Add(new AdoParameterInfo(referenceInfo.Name, referenceInfo.DbType, null));
                }
                else
                {
                    parameters.Add(
                        new AdoParameterInfo(
                            referenceInfo.Name,
                            referenceInfo.DbType,
                            referencePrimaryKeyProperty.GetValue(referencedEntity, null)));
                }
            }

            foreach (var columnInfo in Columns)
            {
                parameters.Add(
                    new AdoParameterInfo(
                        columnInfo.Name, columnInfo.DbType, columnInfo.PropertyInfo.GetValue(entity, null)));
            }

            return parameters;
        }

        private void RemoveLastCommaAndSpaceIfThereAreAnyColumns(StringBuilder builder)
        {
            if ((References.Count() + Columns.Count()) > 0)
            {
                RemoveLastCharacters(builder, 2);
            }
        }

        private string Escape(string name)
        {
            return "[" + name + "]";
        }

        private void RemoveLastCharacters(StringBuilder stringBuilder, int numberOfCharacters)
        {
            stringBuilder.Remove(stringBuilder.Length - numberOfCharacters, numberOfCharacters);
        }

        #endregion SQL commands construction
    }


}