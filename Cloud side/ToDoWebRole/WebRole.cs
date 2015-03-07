using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace ToDoWebRole
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        { 
                DiagnosticMonitor.Start("DiagnosticsConnectionString");
            /**
                // For information on handling configuration changes
                // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
                RoleEnvironment.Changing += RoleEnvironmentChanging;
                return base.OnStart();
            **/
                // For information on handling configuration changes// see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.RoleEnvironment.Changing += RoleEnvironmentChanging;
                CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
                {
                    configSetter(RoleEnvironment.GetConfigurationSettingValue(configName));
                    RoleEnvironment.Changed += (anotherSender, arg) =>
                    {
                        if (arg.Changes.OfType<RoleEnvironmentConfigurationSettingChange>().Any((change) => (change.ConfigurationSettingName == configName)))
                        {
                            if (!configSetter(RoleEnvironment.GetConfigurationSettingValue(configName)))
                            {
                                RoleEnvironment.RequestRecycle();
                            }
                        }
                    };
                });

                var account = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
                CloudTableClient.CreateTablesFromModel(typeof(ToDoDataServiceContext), account.TableEndpoint.AbsoluteUri, account.Credentials);
                //CloudBlobClient blobStorage = account.CreateCloudBlobClient();
   
                return base.OnStart();

          
        }

        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            // If a configuration setting is changing
            if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange))
            {
                // Set e.Cancel to true to restart this role instance
                e.Cancel = true;
            }
        }
    }
}
