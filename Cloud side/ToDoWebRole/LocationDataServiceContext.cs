using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;
using System.Linq;
using ToDoWebRole;
using System.Data.Services;
using System;
using System.Data.Services.Providers;
using System.Collections.Generic;


namespace ToDoWebRole
{
    public class LocationDataServiceContext : TableServiceContext, IDataServiceUpdateProvider
    {
       
    public const string LocationTableName = "LocationTable";
    public const string LocationDataModelTypeName = "ToDoWebRole.LocationDataModel";
    private CloudStorageAccount _storageaccount;
    private string p;
    private StorageCredentials storageCredentials;

    public LocationDataServiceContext() :
        this(CloudStorageAccount.FromConfigurationSetting("DataConnectionString").TableEndpoint.ToString(),//AzureConnection changed to data
        CloudStorageAccount.FromConfigurationSetting("DataConnectionString").Credentials)
    {

    }
    

  /*  public LocationDataServiceContext(string p, Microsoft.WindowsAzure.StorageCredentials storageCredentials)
    {
        // TODO: Complete member initialization
        this.p = p;
        this.StorageCredentials = storageCredentials;
    }*/



    public LocationDataServiceContext(string baseAddress, StorageCredentials credentials)
        : base(baseAddress, credentials)
    {
        this.p = baseAddress;
        this.storageCredentials = credentials;
    }

    public IQueryable<LocationDataModel> LocationTable
    {
        get
        {
            return this.CreateQuery<LocationDataModel>(LocationTableName);
        }
    }

    #region IDataServiceUpdateProvider Members

    public void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region IUpdatable Members

    public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
    {
        throw new NotImplementedException();
    }

    public void ClearChanges()
    {
        throw new NotImplementedException();
    }

    public object GetValue(object targetResource, string propertyName)
    {
        throw new NotImplementedException();
    }

    public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
    {
        throw new NotImplementedException();
    }

    public object ResetResource(object resource)
    {
        throw new NotImplementedException();
    }

    public void SetReference(object targetResource, string propertyName, object propertyValue)
    {
        throw new NotImplementedException();
    }

    //N.B. Note explicit interface implementation
    //was: public void new SaveChanges()
    void IUpdatable.SaveChanges()
    {
        try
        {
            //N.B. that this is actually forwarding from IUpdatable.SaveChanges() to DataServiceContext.SaveChanges()
            base.SaveChanges();
        }
        catch (Exception x)
        {
            //BUG: Why is it trying to duplicate creation of existing resources when a subtask is added?
            //TODO: Figure out this bug
            Console.WriteLine(x);
            //Swallow it for now
        }
    }

    public object CreateResource(string containerName, string fullTypeName)
    {
        //Assert(containerName == "ToDoTable" && fullTypeName == "ToDoWebRole.ToDoDataModel")
        if (containerName != LocationTableName || fullTypeName != LocationDataModelTypeName)
        {
            throw new NotImplementedException();
        }
        var tdm = new ToDoDataModel();
        AddObject(LocationTableName, tdm);
        return tdm;
    }

    public void SetValue(object targetResource, string propertyName, object propertyValue)
    {
        var propInfo = targetResource.GetType().GetProperty(propertyName);
        propInfo.SetValue(targetResource, propertyValue, null);
        UpdateObject(targetResource);
    }

    //Return the instance created by CreateResource()
    public object ResolveResource(object resource)
    {
        return resource;
    }

    public void DeleteResource(object targetResource)
    {
        DeleteObject(targetResource);
    }

    public object GetResource(IQueryable query, string fullTypeName)
    {
        var resource = query.Cast<ToDoDataModel>().SingleOrDefault();
        if (fullTypeName != null && resource.GetType().FullName != fullTypeName)
        {
            throw new ArgumentException("Unexpected type for this resource");
        }
        return resource;
    }

    #endregion

}
          
       
    
}