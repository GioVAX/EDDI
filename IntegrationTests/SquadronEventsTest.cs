using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Windows.Automation;
using EddiCore;
using EddiJournalMonitor;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntegrationTests
{
    [TestClass]
    public class SquadronEventsTest: IntegrationBase
    {
        private static string _configPath;
        [ClassInitialize]
        public static void Setup(TestContext _)
        {
            _configPath = CopyConfiguration(@"..\\..\\TestConfigurationFiles\\BaseConfig");
            MakeSafe();
            LaunchEDDI($"ConfigRoot={_configPath}");

        }

        [ClassCleanup]
        public static void Cleanup()
        {
            ShutdownEDDI();
            DeleteConfiguration(_configPath);
        }

        [TestMethod]
        public void TestMethod1()
        {
            SelectTab("Commander Details");
            var original = GetEditBox("eddiSquadronNameText");

            var squadronName = (ValuePattern)original.GetCurrentPattern(ValuePattern.Pattern);
            var originalValue = squadronName.Current.Value;

            originalValue.Should().Be("Cicciobello");

            var eddi = new PrivateObject(EDDI.Instance);
            var line = @"{ ""timestamp"":""2018-10-17T16:17:55Z"", ""event"":""SquadronCreated"", ""SquadronName"":""TestSquadron"" }";

            var events = JournalMonitor.ParseJournalEntry(line);

            //eddi.Invoke("eventHandler", events[0]);
            
            
            //eddi.Invoke("enqueueEvent", events[0]);

            
            //Task.Run<>(async () => await eddi.Invoke("enqueueEvent", events[0]););


            //await Task.Run(async () =>
            //{
            //    eddi.Invoke("enqueueEvent", events[0]);
            //});


            var task = new Task<object>(() => eddi.Invoke("eventHandler", events[0]));
            task.RunSynchronously();


            //var responder = EDDI.Instance.responders.SingleOrDefault(r => r.ResponderName() == "MainWindow");
            //responder.Handle(events[0]);


            var final = squadronName.Current.Value;
            final.Should().Be("TestSquadron");
        }

        private async void WaitFor<T>(Action<T> f)
        {

        }
    }
}
