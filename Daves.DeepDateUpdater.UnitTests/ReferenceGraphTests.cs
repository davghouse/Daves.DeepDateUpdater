using Daves.DeepDateUpdater.UnitTests.SampleCatalogs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Daves.DeepDateUpdater.UnitTests
{
    [TestClass]
    public class ReferenceGraphTests
    {
        [TestMethod]
        public void TableOrder_ForRootedWorld()
        {
            var referenceGraph = new ReferenceGraph(
                catalog: RootedWorld.Catalog,
                rootTable: RootedWorld.NationsTable);

            CollectionAssert.AreEqual(new[]
                {
                    RootedWorld.NationsTable,
                    RootedWorld.ProvincesTable
                }, referenceGraph.Tables.ToArray());
        }

        [TestMethod]
        public void TableOrder_WithExcludedColumns_ForRootedWorld()
        {
            var referenceGraph = new ReferenceGraph(
                catalog: RootedWorld.Catalog,
                rootTable: RootedWorld.NationsTable,
                excludedDateTypeColumns: new[]
                {
                    RootedWorld.ProvincesTable.FindColumn("FirstRevoltDate"),
                    RootedWorld.ProvincesTable.FindColumn("LastRevoltDate")
                });

            CollectionAssert.AreEqual(new[]
                {
                    RootedWorld.NationsTable,
                }, referenceGraph.Tables.ToArray());
        }
    }
}
