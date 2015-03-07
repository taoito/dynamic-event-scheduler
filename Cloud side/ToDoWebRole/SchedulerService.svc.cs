using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.Device.Location;

namespace ToDoWebRole
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SchedulerService : ISchedulerService
    {
        /*
        public bool AddTask(String arg)
        {
            return true;
        }
        */

        public string TestFun()
        {
            try
            {
                ToDoDataSource tdds = new ToDoDataSource();
                IEnumerable<ToDoDataModel> ans = tdds.Select();
                return ans.Count().ToString();
                
            } catch (Exception e) {
                return e.Message;
            }
            /*
            foreach (ToDoDataModel tddm in ans)
                return tddm.ID + "X" + tddm.Deadline.ToString();
            return "Not found";
             */
        }

        // Returns a list of all Task objects due within the next 24 hours.
        // GroupId: The group ID (possibly a single user) whose tasks we're interested in
        public List<Task> GetTasksDueSoon(List<string> GroupId)
        {
            try
            {
                List<Task> OutList = new List<Task>();
                foreach (string gid in GroupId)
                    OutList.AddRange(Global.ToDoTable.GetTasksDueSoon(gid));
                return OutList;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Returns a list of all Task objects associated with GroupId
        // GroupId: The group ID (possibly a single user) whose tasks we're interested in
        public List<Task> GetAllTasks(List<string> GroupId)
        {
            try
            {
                List<Task> OutList = new List<Task>();
                foreach (string gid in GroupId)
                    OutList.AddRange(Global.ToDoTable.GetAllTasks(gid));
                return OutList;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Returns a list of all User groups associated with UserId
        // UserId: The User ID whose groups we want.
        public List<User> GetMyGroups(string UserId)
        {
            try
            {
                return Global.IdentityTable.GetMyGroups(UserId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<User> GetGroupMembers(string GroupId)
        {
            try
            {
                return Global.IdentityTable.GetGroupMembers(GroupId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Returns a list of all Locations in the database within Dist miles from the given Coordinates.
        public List<Location> GetNearbyLocations(GeoCoordinate Coordinates, float Dist)
        {
            try
            {
                return Global.LocationTable.GetNearbyLocations(Coordinates, Dist);
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Adds a Task to the database
        // UserId: The unique ID of the phone
        // GroupId: The ID of the group to which this task belongs.  If UserId == GroupId, then the task is for that individual.
        // Returns true if successful, false otherwise
        public bool AddTask(Task TheTask)
        {
            ToDoDataModel NewToDo = new ToDoDataModel(TheTask);
            if (NewToDo.Id_new.ToString().Equals("00000000-0000-0000-0000-000000000000"))
                return false;

            try
            {
                Global.ToDoTable.Insert(NewToDo);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

          // Add new user to the database
        public bool AddUser(User TheUser) 
        {
            IdentityDataModel NewIdentity = new IdentityDataModel(TheUser);
            //ToDoDataModel NewIdentity = new ToDoDataModel(TheTask);
            try
            {
                Global.IdentityTable.Insert(NewIdentity);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }   

        // Removes a task from the database.
        // Returns true if successful, false otherwise
        public bool RemoveTask(Guid TaskId)
        {
            try
            {
                return Global.ToDoTable.Delete(TaskId);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemoveUser(Guid U_Id)
        {
            try
            {
                return Global.IdentityTable.Delete(U_Id);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
