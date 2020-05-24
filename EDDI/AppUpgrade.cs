using EddiCore;
using EddiSpeechService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using System.Runtime.InteropServices;

namespace Eddi
{
	class AppUpgrade
	{
        // Upgrade information
        public bool UpgradeAvailable = false;
        public bool UpgradeRequired = false;
        public string UpgradeVersion;
        public string UpgradeLocation;
        public string Motd;
        public List<string> ProductionBuilds = new List<string>() { "r131487/r0" };

        /// <summary>
        /// Check to see if an upgrade is available and populate relevant variables
        /// </summary>
        public void CheckUpgrade()
        {
            // Clear the old values
            UpgradeRequired = false;
            UpgradeAvailable = false;
            UpgradeLocation = null;
            UpgradeVersion = null;
            Motd = null;

            try
            {
                ServerInfo updateServerInfo = ServerInfo.FromServer(Constants.EDDI_SERVER_URL);
                if (updateServerInfo == null)
                {
                    throw new Exception("Failed to contact update server");
                }
                else
                {
                    EDDIConfiguration configuration = EDDIConfiguration.FromFile();
                    InstanceInfo info = configuration.Beta ? updateServerInfo.beta : updateServerInfo.production;
                    string spokenVersion = info.version.Replace(".", $" {Properties.EddiResources.point} ");
                    Motd = info.motd;
                    if (updateServerInfo.productionbuilds != null)
                    {
                        ProductionBuilds = updateServerInfo.productionbuilds;
                    }
                    Utilities.Version minVersion = new Utilities.Version(info.minversion);
                    if (minVersion > Constants.EDDI_VERSION)
                    {
                        // There is a mandatory update available
                        if (!App.FromVA)
                        {
                            string message = String.Format(Properties.EddiResources.mandatory_upgrade, spokenVersion);
                            SpeechService.Instance.Say(null, message, 0);
                        }
                        UpgradeRequired = true;
                        UpgradeLocation = info.url;
                        UpgradeVersion = info.version;
                        return;
                    }

                    Utilities.Version latestVersion = new Utilities.Version(info.version);
                    if (latestVersion > Constants.EDDI_VERSION)
                    {
                        // There is an update available
                        if (!App.FromVA)
                        {
                            string message = String.Format(Properties.EddiResources.update_available, spokenVersion);
                            SpeechService.Instance.Say(null, message, 0);
                        }
                        UpgradeAvailable = true;
                        UpgradeLocation = info.url;
                        UpgradeVersion = info.version;
                    }
                }
            }
            catch (Exception ex)
            {
                SpeechService.Instance.Say(null, Properties.EddiResources.update_server_unreachable, 0);
                Logging.Warn("Failed to access " + Constants.EDDI_SERVER_URL, ex);
            }
        }

        internal static class NativeMethods
        {
            // Required to restart app after upgrade
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            internal static extern uint RegisterApplicationRestart(string pwzCommandLine, RestartFlags dwFlags);
        }

        // Flags for upgrade
        [Flags]
        internal enum RestartFlags
        {
            NONE = 0,
            RESTART_CYCLICAL = 1,
            RESTART_NOTIFY_SOLUTION = 2,
            RESTART_NOTIFY_FAULT = 4,
            RESTART_NO_CRASH = 8,
            RESTART_NO_HANG = 16,
            RESTART_NO_PATCH = 32,
            RESTART_NO_REBOOT = 64
        }


        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public void Upgrade()
        {
            try
            {
                if (UpgradeLocation != null)
                {
                    Logging.Info("Downloading upgrade from " + UpgradeLocation);
                    SpeechService.Instance.Say(null, Properties.EddiResources.downloading_upgrade, 0);
                    string updateFile = Net.DownloadFile(UpgradeLocation, @"EDDI-update.exe");
                    if (updateFile == null)
                    {
                        SpeechService.Instance.Say(null, Properties.EddiResources.download_failed, 0);
                    }
                    else
                    {
                        // Inno setup will attempt to restart this application so register it
                        NativeMethods.RegisterApplicationRestart(null, RestartFlags.NONE);

                        Logging.Info("Downloaded update to " + updateFile);
                        Logging.Info("Path is " + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                        File.SetAttributes(updateFile, FileAttributes.Normal);
                        SpeechService.Instance.Say(null, Properties.EddiResources.starting_upgrade, 0);
                        Logging.Info("Starting upgrade.");

                        Process.Start(updateFile, @"/closeapplications /restartapplications /silent /log /nocancel /noicon /dir=""" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"""");
                    }
                }
            }
            catch (Exception ex)
            {
                SpeechService.Instance.Say(null, Properties.EddiResources.upgrade_failed, 0);
                Logging.Error("Upgrade failed", ex);
            }
        }

    }
}
