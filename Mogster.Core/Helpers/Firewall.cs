using NetFwTypeLib;
using System;
using System.Diagnostics;
using WindowsFirewallHelper;

namespace Thaliak.Network.Utilities
{
    public class FirewallRegister
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        //control.exe /name Microsoft.WindowsFirewall /page pageConfigureApps
        public static void RegisterToFirewall()
        {
            try
            {
                var exePath = Process.GetCurrentProcess().MainModule?.FileName ?? throw new InvalidOperationException("Current process has no main module");

                var rule = FirewallManager.Instance.CreateApplicationRule(FirewallProfiles.Private | FirewallProfiles.Public | FirewallProfiles.Domain, "Mogster", FirewallAction.Allow, exePath);
                rule.IsEnable = true;
                rule.Direction = FirewallDirection.Inbound;
                FirewallManager.Instance.Rules.Add(rule);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        public static void RegisterToFirewall2()
        {
            try
            {
                var exePath = Process.GetCurrentProcess().MainModule?.FileName ?? throw new InvalidOperationException("Current process has no main module");



                var netFwMgr = GetInstance<INetFwMgr>("HNetCfg.FwMgr");

                if (!netFwMgr.LocalPolicy.CurrentProfile.FirewallEnabled) return;

                var netAuthApps = netFwMgr.LocalPolicy.CurrentProfile.AuthorizedApplications;

                var isExists = false;
                foreach (var netAuthAppObject in netAuthApps)
                {
                    if (netAuthAppObject is INetFwAuthorizedApplication netAuthApp && netAuthApp.ProcessImageFileName == exePath && netAuthApp.Enabled)
                    {
                        isExists = true;
                    }
                }

                if (!isExists)
                {
                    var netAuthApp = GetInstance<INetFwAuthorizedApplication>("HNetCfg.FwAuthorizedApplication");

                    netAuthApp.Enabled = true;
                    netAuthApp.Name = "Mogster";
                    netAuthApp.ProcessImageFileName = exePath;
                    netAuthApp.Scope = NET_FW_SCOPE_.NET_FW_SCOPE_MAX;

                    netAuthApps.Add(netAuthApp);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            T GetInstance<T>(string typeName)
            {
                return (T)Activator.CreateInstance(Type.GetTypeFromProgID(typeName));
            }
        }
    }
}
