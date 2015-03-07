using System.Data.Services;
using System.Data.Services.Common;

namespace ToDoWebRole
{    [System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class IdentityDataService: DataService<IdentityDataServiceContext>
    {

            // This method is called only once to initialize service-wide policies.
            public static void InitializeService(DataServiceConfiguration config)
            {
                config.SetEntitySetAccessRule("*", EntitySetRights.All);
                config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                config.UseVerboseErrors = true;
                config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
            }

            protected override void OnStartProcessingRequest(ProcessRequestArgs args)
            {
                base.OnStartProcessingRequest(args);
            }

            protected override void HandleException(HandleExceptionArgs args)
            {
                base.HandleException(args);
            }
        

        /***/


    }
}
