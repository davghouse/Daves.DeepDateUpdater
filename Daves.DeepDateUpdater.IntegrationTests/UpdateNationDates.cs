using Microsoft.Data.Tools.Schema.Sql.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Daves.DeepDateUpdater.IntegrationTests
{
    [TestClass]
    public class UpdateNationDates : SqlDatabaseTestClass
    {
        private SqlDatabaseTestActions UpdateNationDates_UsingDefaultProcedureData;

        public UpdateNationDates()
            => InitializeComponent();

        [TestInitialize]
        public void TestInitialize()
            => InitializeTest();

        [TestCleanup]
        public void TestCleanup()
            => CleanupTest();

        [TestMethod]
        public void UpdateNationDates_UsingDefaultProcedure()
        {
            SqlDatabaseTestActions testActions = this.UpdateNationDates_UsingDefaultProcedureData;
            System.Diagnostics.Trace.WriteLineIf((testActions.PretestAction != null), "Executing pre-test script...");
            SqlExecutionResult[] pretestResults = TestService.Execute(this.PrivilegedContext, this.PrivilegedContext, testActions.PretestAction);
            System.Diagnostics.Trace.WriteLineIf((testActions.TestAction != null), "Executing test script...");
            SqlExecutionResult[] testResults = TestService.Execute(this.ExecutionContext, this.PrivilegedContext, testActions.TestAction);
            System.Diagnostics.Trace.WriteLineIf((testActions.PosttestAction != null), "Executing post-test script...");
            SqlExecutionResult[] posttestResults = TestService.Execute(this.PrivilegedContext, this.PrivilegedContext, testActions.PosttestAction);
        }

        #region Designer support code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction UpdateNationDates_UsingDefaultProcedure_TestAction;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateNationDates));
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ChecksumCondition checkSum;
            this.UpdateNationDates_UsingDefaultProcedureData = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestActions();
            UpdateNationDates_UsingDefaultProcedure_TestAction = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction();
            checkSum = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ChecksumCondition();
            // 
            // UpdateNationDates_UsingDefaultProcedure_TestAction
            // 
            UpdateNationDates_UsingDefaultProcedure_TestAction.Conditions.Add(checkSum);
            resources.ApplyResources(UpdateNationDates_UsingDefaultProcedure_TestAction, "UpdateNationDates_UsingDefaultProcedure_TestAction");
            // 
            // checkSum
            // 
            checkSum.Checksum = "-1609419249";
            checkSum.Enabled = true;
            checkSum.Name = "checkSum";
            // 
            // UpdateNationDates_UsingDefaultProcedureData
            // 
            this.UpdateNationDates_UsingDefaultProcedureData.PosttestAction = null;
            this.UpdateNationDates_UsingDefaultProcedureData.PretestAction = null;
            this.UpdateNationDates_UsingDefaultProcedureData.TestAction = UpdateNationDates_UsingDefaultProcedure_TestAction;
        }

        #endregion
    }
}
