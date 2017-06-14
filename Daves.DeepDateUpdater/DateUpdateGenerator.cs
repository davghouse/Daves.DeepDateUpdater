using Daves.DeepDateUpdater.Helpers;
using Daves.DeepDateUpdater.Metadata;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Daves.DeepDateUpdater
{
    public class DateUpdateGenerator
    {
        protected DateUpdateGenerator(
            Catalog catalog,
            Table rootTable,
            string primaryKeyParameterName,
            string datePart = "day",
            IReadOnlyList<Column> excludedDateTypeColumns = null,
            IReadOnlyList<Table> excludedTables = null,
            ReferenceGraph referenceGraph = null)
        {
            Catalog = catalog;
            RootTable = rootTable;
            PrimaryKeyParameterName = Parameter.ValidateName(primaryKeyParameterName ?? RootTable.DefaultPrimaryKeyParameterName);
            DatePart = datePart;
            ExcludedDateTypeColumns = excludedDateTypeColumns ?? new Column[0];
            ReferenceGraph = referenceGraph ?? new ReferenceGraph(catalog, rootTable, excludedTables, excludedDateTypeColumns);
            DependentlyReferencedVertices = ReferenceGraph.Vertices
                .Where(v => v.IsDependentlyReferenced())
                .ToArray();
            AreTableNamesDistinct = DependentlyReferencedVertices
                .Select(v => v.Table.Name)
                .Distinct()
                .Count() == DependentlyReferencedVertices.Count;

            ProcedureBody.AppendLine();
            GenerateTableVariable(RootTable);
            foreach (var vertex in DependentlyReferencedVertices.Skip(1))
            {
                GenerateTableVariable(vertex.Table);
            }
            GenerateRootTableInsert();
            foreach (var vertex in DependentlyReferencedVertices.Skip(1))
            {
                GenerateDependentlyReferencedTableInsert(vertex);
            }
            foreach (var vertex in ReferenceGraph
                .Where(v => v.Table.HasDateTypeColumns(ExcludedDateTypeColumns)))
            {
                GenerateDateUpdates(vertex);
            }
        }

        protected virtual void GenerateTableVariable(Table table)
        {
            string tableVariableName = $"@{(AreTableNamesDistinct ? "" : table.Schema.SpacelessName)}{table.SingularSpacelessName}IDs";
            TableVariableNames.Add(table, tableVariableName);

            ProcedureBody.AppendLine($"    DECLARE {tableVariableName} TABLE (ID INT NOT NULL UNIQUE);");
        }

        protected virtual void GenerateRootTableInsert()
            => ProcedureBody.AppendLine($@"
    INSERT INTO {TableVariableNames[RootTable]} VALUES ({PrimaryKeyParameterName});");

        protected virtual void GenerateDependentlyReferencedTableInsert(ReferenceGraph.Vertex vertex)
        {
            var table = vertex.Table;
            var whereClauses = vertex.DependentReferences
                .Select(r => $"[{r.ParentColumn.Name}] IN (SELECT ID FROM {TableVariableNames[r.ReferencedTable]})");
            ProcedureBody.AppendLine($@"
    INSERT INTO {TableVariableNames[table]}
    SELECT [{table.PrimaryKey.Column.Name}]
    FROM [{table.Schema}].[{table.Name}]
    WHERE {string.Join($"{Separators.Nlw8}OR ", whereClauses)};");
        }

        protected virtual void GenerateDateUpdates(ReferenceGraph.Vertex vertex)
        {
            var table = vertex.Table;
            var setClauses = table.GetDateTypeColumns(ExcludedDateTypeColumns)
                .Select(c => $"[{c.Name}] = DATEADD({DatePart}, @{DatePart}Delta, [{c.Name}])");
            if (TableVariableNames.TryGetValue(table, out string tableVariableName))
            {
                ProcedureBody.AppendLine($@"
    UPDATE [{table.Schema}].[{table.Name}]
    SET {string.Join(Separators.Cnlw8, setClauses)}
    WHERE [{table.PrimaryKey.Column.Name}] IN (SELECT ID FROM {tableVariableName});");
            }
            else
            {
                var whereClauses = vertex.DependentReferences
                    .Select(r => $"[{r.ParentColumn.Name}] IN (SELECT ID FROM {TableVariableNames[r.ReferencedTable]})");
                ProcedureBody.AppendLine($@"
    UPDATE [{table.Schema}].[{table.Name}]
    SET {string.Join(Separators.Cnlw8, setClauses)}
    WHERE {string.Join($"{Separators.Nlw8}OR ", whereClauses)};");
            }
        }

        protected Catalog Catalog { get; }
        protected Table RootTable { get; }
        protected string PrimaryKeyParameterName { get; }
        protected string DatePart { get; }
        protected IReadOnlyList<Column> ExcludedDateTypeColumns { get; }
        protected ReferenceGraph ReferenceGraph { get; }
        protected StringBuilder ProcedureBody { get; } = new StringBuilder();
        protected IReadOnlyList<ReferenceGraph.Vertex> DependentlyReferencedVertices { get; }
        protected bool AreTableNamesDistinct { get; }
        protected IDictionary<Table, string> TableVariableNames { get; } = new Dictionary<Table, string>();

        public static string GenerateProcedure(IDbConnection connection, string rootTableName,
            string procedureName = null,
            string primaryKeyParameterName = null,
            string rootTableSchemaName = null,
            string datePart = "day")
        {
            var catalog = new Catalog(connection);
            var rootTable = catalog.FindTable(rootTableName, rootTableSchemaName);

            return GenerateProcedure(catalog, rootTable, procedureName, primaryKeyParameterName, datePart);
        }

        public static string GenerateProcedure(IDbConnection connection, IDbTransaction transaction, string rootTableName,
            string procedureName = null,
            string primaryKeyParameterName = null,
            string rootTableSchemaName = null,
            string datePart = "day")
        {
            var catalog = new Catalog(connection, transaction);
            var rootTable = catalog.FindTable(rootTableName, rootTableSchemaName);

            return GenerateProcedure(catalog, rootTable, procedureName, primaryKeyParameterName, datePart);
        }

        public static string GenerateProcedure(Catalog catalog, Table rootTable,
            string procedureName = null,
            string primaryKeyParameterName = null,
            string datePart = "day",
            IReadOnlyList<Column> excludedDateTypeColumns = null,
            IReadOnlyList<Table> excludedTables = null,
            ReferenceGraph referenceGraph = null)
        {
            procedureName = procedureName ?? $"Update{rootTable.SingularSpacelessName}Dates";
            primaryKeyParameterName = Parameter.ValidateName(primaryKeyParameterName ?? rootTable.DefaultPrimaryKeyParameterName);

            return
$@"CREATE PROCEDURE [{rootTable.Schema.Name}].[{procedureName}]
    {primaryKeyParameterName} INT,
    @{datePart}Delta INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRAN;
{GenerateProcedureBody(catalog, rootTable, primaryKeyParameterName, datePart, excludedDateTypeColumns, excludedTables, referenceGraph)}
    COMMIT TRAN;
END;";
        }

        public static string GenerateProcedureBody(IDbConnection connection, string rootTableName, string primaryKeyParameterName,
            string rootTableSchemaName = null,
            string datePart = "day")
        {
            var catalog = new Catalog(connection);
            var rootTable = catalog.FindTable(rootTableName, rootTableSchemaName);

            return GenerateProcedureBody(catalog, rootTable, primaryKeyParameterName, datePart);
        }

        public static string GenerateProcedureBody(IDbConnection connection, IDbTransaction transaction, string rootTableName, string primaryKeyParameterName,
            string rootTableSchemaName = null,
            string datePart = "day")
        {
            var catalog = new Catalog(connection, transaction);
            var rootTable = catalog.FindTable(rootTableName, rootTableSchemaName);

            return GenerateProcedureBody(catalog, rootTable, primaryKeyParameterName, datePart);
        }

        public static string GenerateProcedureBody(Catalog catalog, Table rootTable, string primaryKeyParameterName,
            string datePart = "day",
            IReadOnlyList<Column> excludedDateTypeColumns = null,
            IReadOnlyList<Table> excludedTables = null,
            ReferenceGraph referenceGraph = null)
            => new DateUpdateGenerator(catalog, rootTable, primaryKeyParameterName, datePart, excludedDateTypeColumns, excludedTables, referenceGraph)
            .ProcedureBody
            .ToString();
    }
}
