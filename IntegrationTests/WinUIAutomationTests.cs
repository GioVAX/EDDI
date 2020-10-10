using System.Windows.Automation;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    /// <summary>
    /// Summary description for WinUIAutomationTests
    /// </summary>
    [TestClass]
    public class WinUIAutomationTests : IntegrationBase
    {

        [ClassInitialize]
        public static void Setup(TestContext _) => LaunchEDDI();

        [ClassCleanup]
        public static void Cleanup() => ShutdownEDDI();

 
        [TestMethod]
        public void CanReadTheSquadronNameTextBox()
        {
            SelectTab("Commander Details");
            var edit = GetEditBox("eddiSquadronNameText");

            var squadronName = (ValuePattern)edit.GetCurrentPattern(ValuePattern.Pattern);
            squadronName.Current.Value
                .Should().Be("Newton's Gambit");
        }

        [TestMethod]
        public void CanReadTheCommanderNameTextBox()
        {
            SelectTab("Commander Details");
            var edit = GetEditBox("eddiCommanderPhoneticNameText");

            var squadronName = (ValuePattern)edit.GetCurrentPattern(ValuePattern.Pattern);
            squadronName.Current.Value
                .Should().Be("giovax");
        }

        [TestMethod]
        public void CanReadTheEDSMCommanderNameTextBox()
        {
            SelectTab("EDSM Responder");
            var edit = GetEditBox("edsmCommanderNameTextBox");

            var squadronName = (ValuePattern)edit.GetCurrentPattern(ValuePattern.Pattern);
            squadronName.Current.Value
                .Should().Be("GioVAX");
        }
    }
}
