using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    /// <summary>
    /// Summary description for WinUIAutomationTests
    /// </summary>
    [TestClass]
    public class WinUIAutomationTests
    {
        private static Process _eddi;
        private AutomationElement _mainForm;

        [ClassInitialize]
        public static void LaunchEDDI(TestContext _) =>
            _eddi = Process.Start(@"C:\Program Files (x86)\VoiceAttack\Apps\EDDI\EDDI.exe");

        [ClassCleanup]
        public static void ShutdownEDDI() =>
            _eddi?.CloseMainWindow();

        private AutomationElement MainForm => _mainForm ?? (_mainForm = GetMainForm());

        private AutomationElement GetMainForm()
        {
            // get reference to main Form control 
            var aeDesktop = AutomationElement.RootElement;
            // a better approach 
            AutomationElement aeForm = null;
            for (var numWaits = 0; aeForm == null && numWaits < 50; ++numWaits)
            {
                aeForm = aeDesktop.FindFirst(TreeScope.Children,
                    new PropertyCondition(AutomationElement.NameProperty, "EDDI v.3.7.0"));
                ++numWaits;
                Thread.Sleep(100);
            }

            if (aeForm != null)
                return aeForm;

            throw new Exception("Failed to find EDDI");
        }

        private void SelectTab(string tabName)
        {
            var tab = MainForm.FindFirst(TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem),
                    new PropertyCondition(AutomationElement.NameProperty, tabName))
                );

            var sel = (SelectionItemPattern)tab.GetCurrentPattern(SelectionItemPattern.Pattern);
            sel.Select();
        }

        private AutomationElement GetEditBox(string editName) =>
            MainForm.FindFirst(TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit),
                    new PropertyCondition(AutomationElement.AutomationIdProperty, editName))
                );

        [TestMethod]
        public void CanReadTheSquadronNameTextBox()
        {
            var mainForm = MainForm;
            SelectTab("Commander Details");
            var edit = GetEditBox("eddiSquadronNameText");

            var squadronName = (ValuePattern)edit.GetCurrentPattern(ValuePattern.Pattern);
            squadronName.Current.Value
                .Should().Be("Newton's Gambit");
        }
    }
}
