using Daves.DeepDateUpdater.Metadata;

namespace Daves.DeepDateUpdater.UnitTests.SampleCatalogs
{
    public static class UnrootedWorld
    {
        public static readonly Catalog Catalog;
        public static Table NationsTable => Catalog.FindTable("Nations");
        public static Table ProvincesTable => Catalog.FindTable("Provinces");
        public static Table ResidentsTable => Catalog.FindTable("Residents");

        static UnrootedWorld()
        {
            var schemas = new[]
            {
                new Schema(name: "dbo", id: 1),
                new Schema(name: "sys", id: 2)
            };
            var tables = new[]
            {
                // Create tables out of order to make sure proper ordering is happening.
                new Table(name: "Residents", id: 6, schemaId: 1),
                new Table(name: "Nations", id: 3, schemaId: 1),
                new Table(name: "Provinces", id: 4, schemaId: 1),
                new Table(name: "ProvinceLeafs", id: 100, schemaId: 1),
                new Table(name: "ProvinceLeafLeafs", id: 101, schemaId: 1)
            };
            var columns = new[]
            {
                new Column(tableId: 6, name: "ID", columnId: 1, isNullable: false, isIdentity: true),
                new Column(tableId: 6, name: "Name", columnId: 2, isNullable: false),
                new Column(tableId: 6, name: "ProvinceID", columnId: 3, isNullable: false),
                new Column(tableId: 6, name: "NationalityNationID", columnId: 4, isNullable: false),
                new Column(tableId: 6, name: "SpouseResidentID", columnId: 5, isNullable: true),
                new Column(tableId: 6, name: "FavoriteProvinceID", columnId: 6, isNullable: true),
                new Column(tableId: 6, name: "BirthDate", columnId: 7, isNullable: true, isDateType: true),
                new Column(tableId: 3, name: "ID", columnId: 1, isNullable: false, isIdentity: true),
                new Column(tableId: 3, name: "Name", columnId: 2, isNullable: false),
                new Column(tableId: 3, name: "FoundedDate", columnId: 3, isNullable: false, isDateType: true),
                new Column(tableId: 4, name: "ID", columnId: 1, isNullable: false, isIdentity: true),
                new Column(tableId: 4, name: "NationID", columnId: 2, isNullable: false),
                new Column(tableId: 4, name: "Name", columnId: 3, isNullable: false),
                new Column(tableId: 4, name: "Motto", columnId: 4, isNullable: false),
                new Column(tableId: 4, name: "LeaderResidentID", columnId: 5, isNullable: true),
                new Column(tableId: 4, name: "FirstRevoltDate", columnId: 6, isNullable: true, isDateType: true),
                new Column(tableId: 4, name: "LastRevoltDate", columnId: 7, isNullable: true, isDateType: true),
                new Column(tableId: 4, name: "ComputedRevoltDate", columnId: 8, isNullable: true, isDateType: true, isComputed: true),
                new Column(tableId: 100, name: "ID", columnId: 1, isNullable: false, isIdentity: true),
                new Column(tableId: 100, name: "Name", columnId: 2, isNullable: false),
                new Column(tableId: 100, name: "ProvinceID", columnId: 3, isNullable: false),
                new Column(tableId: 101, name: "ID", columnId: 1, isNullable: false, isIdentity: true),
                new Column(tableId: 101, name: "Name", columnId: 2, isNullable: false),
                new Column(tableId: 101, name: "ProvinceLeafID", columnId: 3, isNullable: false)
            };
            var primaryKeys = new[]
            {
                new PrimaryKey(tableId: 6, name: "PK_Residents"),
                new PrimaryKey(tableId: 3, name: "PK_Nations"),
                new PrimaryKey(tableId: 4, name: "PK_Provinces"),
                new PrimaryKey(tableId: 100, name: "PK_ProvinceLeafs"),
                new PrimaryKey(tableId: 101, name: "PK_ProvinceLeafLeafs"),
            };
            var primaryKeyColumns = new[]
            {
                new PrimaryKeyColumn(tableId: 3, columnId: 1),
                new PrimaryKeyColumn(tableId: 4, columnId: 1),
                new PrimaryKeyColumn(tableId: 6, columnId: 1),
                new PrimaryKeyColumn(tableId: 100, columnId: 1),
                new PrimaryKeyColumn(tableId: 101, columnId: 1)
            };
            var foreignKeys = new[]
            {
                new ForeignKey(name: "FK_Provinces_NationID_Nations_ID", id: 5, parentTableId: 4, referencedTableId: 3),
                new ForeignKey(name: "FK_Residents_ProvinceID_Provinces_ID", id: 7, parentTableId: 6, referencedTableId: 4),
                new ForeignKey(name: "FK_Residents_NationalityNationID_Nations_ID", id: 8, parentTableId: 6, referencedTableId: 3),
                new ForeignKey(name: "FK_Residents_SpouseResidentID_Residents_ID", id: 9, parentTableId: 6, referencedTableId: 6),
                new ForeignKey(name: "FK_Provinces_LeaderResidentID_Residents_ID", id: 10, parentTableId: 4, referencedTableId: 6),
                new ForeignKey(name: "FK_Residents_FavoriteProvinceID_Provinces_ID", id: 11, parentTableId: 6, referencedTableId: 4),
                new ForeignKey(name: "FK_ProvinceLeafs_ProvinceID_Provinces_ID", id: 12, parentTableId: 100, referencedTableId: 4),
                new ForeignKey(name: "FK_ProvinceLeafLeafs_ProvinceLeafID_ProvinceLeafs_ID", id: 13, parentTableId: 101, referencedTableId: 100)
            };
            var foreignKeyColumns = new[]
            {
                new ForeignKeyColumn(foreignKeyId: 7, parentTableId: 6, parentColumnId: 3, referencedTableId: 4, referencedColumnId: 1),
                new ForeignKeyColumn(foreignKeyId: 5, parentTableId: 4, parentColumnId: 2, referencedTableId: 3, referencedColumnId: 1),
                new ForeignKeyColumn(foreignKeyId: 8, parentTableId: 6, parentColumnId: 4, referencedTableId: 3, referencedColumnId: 1),
                new ForeignKeyColumn(foreignKeyId: 9, parentTableId: 6, parentColumnId: 5, referencedTableId: 6, referencedColumnId: 1),
                new ForeignKeyColumn(foreignKeyId: 10, parentTableId: 4, parentColumnId: 5, referencedTableId: 6, referencedColumnId: 1),
                new ForeignKeyColumn(foreignKeyId: 11, parentTableId: 6, parentColumnId: 6, referencedTableId: 4, referencedColumnId: 1),
                new ForeignKeyColumn(foreignKeyId: 12, parentTableId: 100, parentColumnId: 3, referencedTableId: 4, referencedColumnId: 1),
                new ForeignKeyColumn(foreignKeyId: 13, parentTableId: 101, parentColumnId: 3, referencedTableId: 100, referencedColumnId: 1)
            };
            var checkConstraints = new CheckConstraint[0];

            Catalog = new Catalog(schemas, tables, columns, primaryKeys, primaryKeyColumns, foreignKeys, foreignKeyColumns, checkConstraints);
        }
    }
}
