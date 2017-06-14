using Daves.DeepDateUpdater.UnitTests.SampleCatalogs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Daves.DeepDateUpdater.UnitTests
{
    [TestClass]
    public class DateUpdateGeneratorTests
    {
        [TestMethod]
        public void GenerateProcedure_Default_ForRootedWorld()
        {
            string procedure = DateUpdateGenerator.GenerateProcedure(
                catalog: RootedWorld.Catalog,
                rootTable: RootedWorld.NationsTable);

            Assert.AreEqual(
@"CREATE PROCEDURE [dbo].[UpdateNationDates]
    @id INT,
    @dayDelta INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRAN;

    DECLARE @NationIDs TABLE (ID INT NOT NULL UNIQUE);

    INSERT INTO @NationIDs VALUES (@id);

    UPDATE [dbo].[Nations]
    SET [FoundedDate] = DATEADD(day, @dayDelta, [FoundedDate])
    WHERE [ID] IN (SELECT ID FROM @NationIDs);

    UPDATE [dbo].[Provinces]
    SET [FirstRevoltDate] = DATEADD(day, @dayDelta, [FirstRevoltDate]),
        [LastRevoltDate] = DATEADD(day, @dayDelta, [LastRevoltDate])
    WHERE [NationID] IN (SELECT ID FROM @NationIDs);

    COMMIT TRAN;
END;", procedure);
        }

        [TestMethod]
        public void GenerateProcedure_WithAnExcludedColumn_ForRootedWorld()
        {
            string procedure = DateUpdateGenerator.GenerateProcedure(
                catalog: RootedWorld.Catalog,
                rootTable: RootedWorld.NationsTable,
                excludedDateTypeColumns: new[] { RootedWorld.ProvincesTable.FindColumn("FirstRevoltDate") });

            Assert.AreEqual(
@"CREATE PROCEDURE [dbo].[UpdateNationDates]
    @id INT,
    @dayDelta INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRAN;

    DECLARE @NationIDs TABLE (ID INT NOT NULL UNIQUE);

    INSERT INTO @NationIDs VALUES (@id);

    UPDATE [dbo].[Nations]
    SET [FoundedDate] = DATEADD(day, @dayDelta, [FoundedDate])
    WHERE [ID] IN (SELECT ID FROM @NationIDs);

    UPDATE [dbo].[Provinces]
    SET [LastRevoltDate] = DATEADD(day, @dayDelta, [LastRevoltDate])
    WHERE [NationID] IN (SELECT ID FROM @NationIDs);

    COMMIT TRAN;
END;", procedure);
        }

        [TestMethod]
        public void GenerateProcedure_WithExcludedColumns_ForRootedWorld()
        {
            string procedure = DateUpdateGenerator.GenerateProcedure(
                catalog: RootedWorld.Catalog,
                rootTable: RootedWorld.NationsTable,
                excludedDateTypeColumns: new[]
                {
                    RootedWorld.ProvincesTable.FindColumn("FirstRevoltDate"),
                    RootedWorld.ProvincesTable.FindColumn("LastRevoltDate")
                });

            Assert.AreEqual(
@"CREATE PROCEDURE [dbo].[UpdateNationDates]
    @id INT,
    @dayDelta INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRAN;

    DECLARE @NationIDs TABLE (ID INT NOT NULL UNIQUE);

    INSERT INTO @NationIDs VALUES (@id);

    UPDATE [dbo].[Nations]
    SET [FoundedDate] = DATEADD(day, @dayDelta, [FoundedDate])
    WHERE [ID] IN (SELECT ID FROM @NationIDs);

    COMMIT TRAN;
END;", procedure);
        }

        [TestMethod]
        public void GenerateProcedure_Default_ForUnrootedWorld()
        {
            string procedure = DateUpdateGenerator.GenerateProcedure(
                catalog: UnrootedWorld.Catalog,
                rootTable: UnrootedWorld.NationsTable);

            Assert.AreEqual(
@"CREATE PROCEDURE [dbo].[UpdateNationDates]
    @id INT,
    @dayDelta INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRAN;

    DECLARE @NationIDs TABLE (ID INT NOT NULL UNIQUE);
    DECLARE @ProvinceIDs TABLE (ID INT NOT NULL UNIQUE);

    INSERT INTO @NationIDs VALUES (@id);

    INSERT INTO @ProvinceIDs
    SELECT [ID]
    FROM [dbo].[Provinces]
    WHERE [NationID] IN (SELECT ID FROM @NationIDs);

    UPDATE [dbo].[Nations]
    SET [FoundedDate] = DATEADD(day, @dayDelta, [FoundedDate])
    WHERE [ID] IN (SELECT ID FROM @NationIDs);

    UPDATE [dbo].[Provinces]
    SET [FirstRevoltDate] = DATEADD(day, @dayDelta, [FirstRevoltDate]),
        [LastRevoltDate] = DATEADD(day, @dayDelta, [LastRevoltDate])
    WHERE [ID] IN (SELECT ID FROM @ProvinceIDs);

    UPDATE [dbo].[Residents]
    SET [BirthDate] = DATEADD(day, @dayDelta, [BirthDate])
    WHERE [ProvinceID] IN (SELECT ID FROM @ProvinceIDs)
        OR [NationalityNationID] IN (SELECT ID FROM @NationIDs);

    COMMIT TRAN;
END;", procedure);
        }

        [TestMethod]
        public void GenerateProcedure_WithAnExcludedColumn_ForUnrootedWorld()
        {
            string procedure = DateUpdateGenerator.GenerateProcedure(
                catalog: UnrootedWorld.Catalog,
                rootTable: UnrootedWorld.NationsTable,
                excludedDateTypeColumns: new[] { UnrootedWorld.NationsTable.FindColumn("FoundedDate") });

            Assert.AreEqual(
@"CREATE PROCEDURE [dbo].[UpdateNationDates]
    @id INT,
    @dayDelta INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRAN;

    DECLARE @NationIDs TABLE (ID INT NOT NULL UNIQUE);
    DECLARE @ProvinceIDs TABLE (ID INT NOT NULL UNIQUE);

    INSERT INTO @NationIDs VALUES (@id);

    INSERT INTO @ProvinceIDs
    SELECT [ID]
    FROM [dbo].[Provinces]
    WHERE [NationID] IN (SELECT ID FROM @NationIDs);

    UPDATE [dbo].[Provinces]
    SET [FirstRevoltDate] = DATEADD(day, @dayDelta, [FirstRevoltDate]),
        [LastRevoltDate] = DATEADD(day, @dayDelta, [LastRevoltDate])
    WHERE [ID] IN (SELECT ID FROM @ProvinceIDs);

    UPDATE [dbo].[Residents]
    SET [BirthDate] = DATEADD(day, @dayDelta, [BirthDate])
    WHERE [ProvinceID] IN (SELECT ID FROM @ProvinceIDs)
        OR [NationalityNationID] IN (SELECT ID FROM @NationIDs);

    COMMIT TRAN;
END;", procedure);
        }
    }
}
