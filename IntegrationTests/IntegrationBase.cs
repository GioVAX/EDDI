using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;
using EddiCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rollbar;
using Utilities;

namespace IntegrationTests
{
    public class IntegrationBase
    {
        /// <summary>
        /// Static code used during tests Setup and Cleanup
        /// </summary>
        private static Process _eddi;
        protected static void LaunchEDDI(string exeParams="") =>
            // Relative path to the local built executable
            _eddi = Process.Start(@".\EDDI.exe", exeParams);

        protected static void ShutdownEDDI() =>
            _eddi?.CloseMainWindow();

        private AutomationElement _mainForm;

        private AutomationElement MainForm => _mainForm ?? (_mainForm = GetMainForm());

        private AutomationElement GetMainForm()
        {
            // get reference to main Form control 
            // a better approach 
            AutomationElement aeForm = null;
            for (var numWaits = 0; aeForm == null && numWaits < 50; ++numWaits)
            {
                var title = "EDDI v." + Constants.EDDI_VERSION;

                aeForm = AutomationElement.RootElement.FindFirst(TreeScope.Children,
                    new PropertyCondition(AutomationElement.NameProperty, title));
                ++numWaits;
                Thread.Sleep(100);
            }

            if (aeForm != null)
                return aeForm;

            throw new Exception("Failed to find EDDI");
        }

        protected void SelectTab(string tabName)
        {
            var tab = MainForm.FindFirst(TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem),
                    new PropertyCondition(AutomationElement.NameProperty, tabName))
            );

            var sel = (SelectionItemPattern)tab.GetCurrentPattern(SelectionItemPattern.Pattern);
            sel.Select();
        }

        protected AutomationElement GetEditBox(string editName) =>
            MainForm.FindFirst(TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit),
                    new PropertyCondition(AutomationElement.AutomationIdProperty, editName))
            );


        protected void MakeSafe()
        {
            // Prevent telemetry data from being reported based on test results
            RollbarLocator.RollbarInstance.Config.Enabled = false;

            // Don't write to permanent storage (do this before we initialize our EDDI instance)
            Utilities.Files.unitTesting = true;

            // Set ourselves as in a beta game session to stop automatic sending of data to remote systems
            PrivateObject privateObject = new PrivateObject(EDDI.Instance);
            privateObject.SetFieldOrProperty("gameIsBeta", true);
        }
    }
}
