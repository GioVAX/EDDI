using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

namespace IntegrationTests
{
    [TestClass]
    public class UnitTest1
    {
        private static WindowsDriver<WindowsElement> session;

        [ClassInitialize]
        public static void CreateSession(TestContext _)
        {
            // At this point "C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe"
            // should be already running!

            var appiumOptions = new AppiumOptions();
            appiumOptions.AddAdditionalCapability("app", @"C:\Program Files (x86)\VoiceAttack\Apps\EDDI\EDDI.exe");
            appiumOptions.AddAdditionalCapability("deviceName", "WindowsPC");
            appiumOptions.AddAdditionalCapability("platformName", "Windows");
            session = null;

            try
            {
                Console.WriteLine("Trying to Launch App");
                session = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appiumOptions);
            }
            catch
            {
                Console.WriteLine("Failed to attach to app session (expected).");
            }
        }

        [ClassCleanup]
        public static void CloseSession()
        {
            // Close the application and delete the session
            if (session != null)
            {
                session.Quit();
                session = null;
            }
        }

        [TestMethod]
        public void LaunchingEDDIApp_ShouldSucceed()
        {
            session.Should().NotBeNull();
        }

        [TestMethod]
        [Ignore]
        public void Eddi_ShouldHave1DockPanel()
        {
            var elems = session.FindElementsByTagName("DockPanel");
            elems.Should().HaveCount(1);
        }

        [TestMethod]
        public void Eddi_ShouldHave16TabItems()
        {
            var elems = session.FindElementsByTagName("TabItem");
            elems.Should().HaveCount(16);
        }

        [TestMethod]
        public void Eddi_ShouldHaveEDDIAsFirstTab()
        {
            var elems = session.FindElementsByTagName("TabItem");
            elems.First().Text.Should().Be("EDDI");
        }

        [TestMethod]
        public void Eddi_ShouldHaveAllTabsWithDistinctNames()
        {
            var elems = session.FindElementsByTagName("TabItem");
            var totElems = elems.Count();

            elems.GroupBy(t => t.Text)
                .Should().HaveCount(totElems);
        }

        [TestMethod]
        public void Eddi_ShouldHaveOneTabSelected()
        {
            var elems = session.FindElementsByTagName("TabItem");
            var totElems = elems.Count();

            elems.Where(t => t.Selected )
                .Should().HaveCount(1);
        }

        [TestMethod]
        public void Eddi_WHENSelectingTab_SHOULDChangeSelectedItem()
        {
            var elems = session.FindElementsByTagName("TabItem");
            var eddi = elems.Single(t => t.Text == "EDDI");
            var ship = elems.Single(t => t.Text == "Ship Monitor");

            //if (eddi.Selected)
            //    ship.
            //    ship.Selected = true;



        }
    }
}
