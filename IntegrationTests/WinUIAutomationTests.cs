using System.Windows.Automation;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    [TestClass]
    public class WinUIAutomationFunctions : IntegrationBase
    {
        [ClassInitialize]
        public static void Setup(TestContext _) => LaunchEDDI(@"ConfigRoot=..\..\TestConfigurationFiles\BaseConfig");

        [ClassCleanup]
        public static void Cleanup() => ShutdownEDDI();

 
        [TestMethod]
        public void CanReadTheSquadronNameTextBox()
        {
            SelectTab("Commander Details");
            var edit = GetEditBox("eddiSquadronNameText");

            var squadronName = (ValuePattern)edit.GetCurrentPattern(ValuePattern.Pattern);
            squadronName.Current.Value
                .Should().Be("Cicciobello");
        }

         [TestMethod]
        public void CanReadTheEDSMCommanderNameTextBox()
        {
            SelectTab("EDSM Responder");
            var edit = GetEditBox("edsmCommanderNameTextBox");

            var edsmCmndrName = (ValuePattern)edit.GetCurrentPattern(ValuePattern.Pattern);
            edsmCmndrName.Current.Value
                .Should().Be("GioVAX");
        }

        [TestMethod]
        public void ControlsFromUnselectedTabs_SHOULDNotBeAccessible()
        {
            SelectTab("EDSM Responder");
            var edit = GetEditBox("eddiCommanderPhoneticNameText"); // This is in the "Commander Details" tab

            edit.Should().BeNull();
        }
    }
}
