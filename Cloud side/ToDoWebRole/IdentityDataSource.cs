using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;


namespace ToDoWebRole
{
    public class IdentityDataSource
    {
        //Azure Table Storage source
          private IdentityDataServiceContext context = null;

            //Connect to storage. Create context. Lazy initialize table.
            public IdentityDataSource()
            {
                //Connect to storage
                CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
                                                        configSetter(ConfigurationManager.AppSettings[configName]));
                var sa = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
                //Create context
                context = new IdentityDataServiceContext(sa.TableEndpoint.ToString(), sa.Credentials);
  
                //Ensure that table exists
                var cloudClient = sa.CreateCloudTableClient();
                //cloudClient.DeleteTable("IdentityTable");
                if (cloudClient.DoesTableExist(IdentityDataServiceContext.IdentityTableName) == false)
                {
                    //Strictly speaking, this is a race condition -- table could come into existence from other source while this block executes. We can ignore it for our purposes. 
                    cloudClient.CreateTableIfNotExist(IdentityDataServiceContext.IdentityTableName);
                }

                //Ensure that the _ROOT task exists. Ditto race condition possibility, ditto ignore.
                if (!context.IdentityTable.ToList<IdentityDataModel>().Any(t => t.Id_new.Equals(IdentityDataModel.ROOT_ID)))
                {
                    var root = new IdentityDataModel(IdentityDataModel.ROOT_ID, String.Empty);
                    root.Id_new = IdentityDataModel.ROOT_ID;
                    root.UserId = "_ROOT";
                    root.GroupId = "_ROOT";
                    root.Password = "***";
                    root.GroupPass = "***";
                    Insert(root);
                }
            }

            public bool Delete(Guid id)
            {
                IQueryable<IdentityDataModel> q = from ide in context.IdentityTable where ide.Id_new.Equals(id) select ide;
                CloudTableQuery<IdentityDataModel> q2 = q.AsTableServiceQuery<IdentityDataModel>();
                IEnumerable<IdentityDataModel> ans = q2.Execute();
                if (ans.Count() == 0)
                    return false;
                context.DeleteObject(ans.ElementAt(0));
                context.SaveChanges();
                return true;
            }

            //Generic "Select"
            public IEnumerable<IdentityDataModel> Select()
            {
                var rs = from todo in context.IdentityTable select todo;
                var query = rs.AsTableServiceQuery<IdentityDataModel>();
                return query.Execute();
            }

            public void Insert(IdentityDataModel ide)
            {
                context.AddObject(IdentityDataServiceContext.IdentityTableName, ide);
                context.SaveChanges();
            }

            public List<User> GetMyGroups(string UserID)
            {
                IQueryable<IdentityDataModel> q = from user in context.IdentityTable where user.UserId.Equals(UserID) select user;
                CloudTableQuery<IdentityDataModel> q2 = q.AsTableServiceQuery<IdentityDataModel>();
                IEnumerable<IdentityDataModel> ans = q2.Execute();
                List<IdentityDataModel> ansList = ans.ToList<IdentityDataModel>();
                List<User> outList = new List<User>();
                foreach (IdentityDataModel iUser in ansList)
                    outList.Add(new User(iUser));
                return outList;
            }

            public List<User> GetGroupMembers(string GroupID)
            {
                IQueryable<IdentityDataModel> q = from user in context.IdentityTable where user.GroupId.Equals(GroupID) select user;
                CloudTableQuery<IdentityDataModel> q2 = q.AsTableServiceQuery<IdentityDataModel>();
                IEnumerable<IdentityDataModel> ans = q2.Execute();
                List<IdentityDataModel> ansList = ans.ToList<IdentityDataModel>();
                List<User> outList = new List<User>();
                foreach (IdentityDataModel iUser in ansList)
                    outList.Add(new User(iUser));
                return outList;
            }
    }

}

       
 
