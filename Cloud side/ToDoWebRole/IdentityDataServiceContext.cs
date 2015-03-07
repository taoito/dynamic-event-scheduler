using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;
using System.Linq;
using ToDoWebRole;
using System.Data.Services;
using System;
using System.Data.Services.Providers;
using System.Collections.Generic;


public class IdentityDataServiceContext: TableServiceContext, IDataServiceUpdateProvider
{
    public const string IdentityTableName = "IdentityTable";
    public const string IdentityDataModelTypeName = "ToDoWebRole.IdentityDataModel";
    //private CloudStorageAccount _storageaccount;
    private string baseAddress;
    private StorageCredentials storageCredentials;

    public IdentityDataServiceContext() :
        this(CloudStorageAccount.FromConfigurationSetting("DataConnectionString").TableEndpoint.ToString(),
        CloudStorageAccount.FromConfigurationSetting("DataConnectionString").Credentials)
    {

    }
    
    public IdentityDataServiceContext(string baseAddress, StorageCredentials credentials)
        : base(baseAddress, credentials)
    {
        this.baseAddress = baseAddress;
        this.storageCredentials = credentials;
    }

    public IQueryable<IdentityDataModel> IdentityTable
    {
        get
        {
            return this.CreateQuery<IdentityDataModel>(IdentityTableName);
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
        if (containerName != IdentityTableName || fullTypeName != IdentityDataModelTypeName)
        {
            throw new NotImplementedException();
        }
        var tdm = new IdentityDataModel();
        AddObject(IdentityTableName, tdm);
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
        var resource = query.Cast<IdentityDataModel>().SingleOrDefault();
        if (fullTypeName != null && resource.GetType().FullName != fullTypeName)
        {
            throw new ArgumentException("Unexpected type for this resource");
        }
        return resource;
    }

    #endregion

}

    
