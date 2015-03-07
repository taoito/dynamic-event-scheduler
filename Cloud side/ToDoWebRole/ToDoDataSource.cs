using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace ToDoWebRole
{
    //Azure Table Storage source
    public class ToDoDataSource
    {
        private ToDoDataServiceContext context = null;

        //Connect to storage. Create context. Lazy initialize table.
        public ToDoDataSource()
        {
            //Connect to storage
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
 configSetter(ConfigurationManager.AppSettings[configName]));
            var sa = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");//AzureConn Changed to DataConnection

            //Create context
            context = new ToDoDataServiceContext(sa.TableEndpoint.ToString(), sa.Credentials);

            //Ensure that table exists
            var cloudClient = sa.CreateCloudTableClient();
            //cloudClient.DeleteTable("ToDoTable");
            //cloudClient.DeleteTableIfExist(ToDoDataServiceContext.ToDoTableName); System.Threading.Thread.Sleep(10000);
            if (cloudClient.DoesTableExist(ToDoDataServiceContext.ToDoTableName) == false)
            {
                //Strictly speaking, this is a race condition -- table could come into existence from other source while this block executes. We can ignore it for our purposes. 
                cloudClient.CreateTableIfNotExist(ToDoDataServiceContext.ToDoTableName);

            }

            //Ensure that the _ROOT task exists. Ditto race condition possibility, ditto ignore.
            List<ToDoDataModel> TempList = context.ToDoTable.ToList<ToDoDataModel>();
            if (!TempList.Any(t => t.Id_new.Equals(ToDoDataModel.ROOT_ID)))
            {
                var root = new ToDoDataModel(ToDoDataModel.ROOT_ID, String.Empty);
                root.Id_new = ToDoDataModel.ROOT_ID;
                root.Deadline = DateTime.Now;
                root.ReminderTime = DateTime.Now;
                root.TaskName = "(Tasks)";
                Insert(root);
            }
        }

        //Generic "Select"
        public IEnumerable<ToDoDataModel> Select()
        {
            var rs = from todo in context.ToDoTable select todo;
            var query = rs.AsTableServiceQuery<ToDoDataModel>();
            return query.Execute();
        }

        public void Insert(ToDoDataModel todo)
        {
            context.AddObject(ToDoDataServiceContext.ToDoTableName, todo);
            context.SaveChanges();
        }

        public bool Delete(Guid id)
        {
            IQueryable<ToDoDataModel> q = from todo in context.ToDoTable where todo.Id_new.Equals(id) select todo;
            CloudTableQuery<ToDoDataModel> q2 = q.AsTableServiceQuery<ToDoDataModel>();
            IEnumerable<ToDoDataModel> ans = q2.Execute();
            if (ans.Count() == 0)
                return false;
            context.DeleteObject(ans.ElementAt(0));
            context.SaveChanges();
            return true;
        }

        public List<Task> GetTasksDueSoon(string GroupId)
        {
            IQueryable<ToDoDataModel> q = from todo in context.ToDoTable where todo.GroupId.Equals(GroupId) select todo;
            CloudTableQuery<ToDoDataModel> q2 = q.AsTableServiceQuery<ToDoDataModel>();
            IEnumerable<ToDoDataModel> ans = q2.Execute();
            List<ToDoDataModel> ansList = ans.ToList<ToDoDataModel>();
            List<Task> outList = new List<Task>();
            foreach (ToDoDataModel ToDo in ansList)
                if (ToDo.Deadline - new TimeSpan(24,0,0) <= DateTime.Now)
                    outList.Add(new Task(ToDo));
            return outList;
        }

        public List<Task> GetAllTasks(string GroupId)
        {
            IQueryable<ToDoDataModel> q = from todo in context.ToDoTable where todo.GroupId.Equals(GroupId) select todo;
            CloudTableQuery<ToDoDataModel> q2 = q.AsTableServiceQuery<ToDoDataModel>();
            IEnumerable<ToDoDataModel> ans = q2.Execute();
            List<ToDoDataModel> ansList = ans.ToList<ToDoDataModel>();
            List<Task> outList = new List<Task>();
            foreach (ToDoDataModel ToDo in ansList)
                outList.Add(new Task(ToDo));
            return outList;
        }
    }
}
