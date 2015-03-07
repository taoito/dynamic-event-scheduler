using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Device.Location;

namespace ToDoWebRole
{
    [ServiceContract]
    public interface ISchedulerService
    {
        // Test function; not for production use
        //[OperationContract]
        //bool AddTask(String arg);

        // Test function; not for production use
        [OperationContract]
        string TestFun();

        // Returns a list of all Task objects associated with GroupId due within the next 24 hours.
        // GroupId: The group ID (possibly a single user) whose tasks we're interested in
        [OperationContract]
        List<Task> GetTasksDueSoon(List<string> GroupId);

        // Returns a list of all Task objects associated with GroupId
        // GroupId: The group ID (possibly a single user) whose tasks we're interested in
        [OperationContract]
        List<Task> GetAllTasks(List<string> GroupId);

        // Returns a list of all User groups associated with UserId
        // UserId: The User ID whose groups we want.
        [OperationContract]
        List<User> GetMyGroups(string UserId);

        [OperationContract]
        List<User> GetGroupMembers(string GroupId);

        //Returns a list of all Locations in the database within Dist miles from the given Coordinates.
        [OperationContract]
        List<Location> GetNearbyLocations(GeoCoordinate Coordinates, float Dist);

        // Adds a Task to the database
        // UserId: The unique ID of the phone
        // GroupId: The ID of the group to which this task belongs.  If UserId == GroupId, then the task is for that individual.
        // Returns true if successful, false otherwise
        [OperationContract]
        bool AddTask(Task TheTask);

        [OperationContract]
        bool AddUser(User TheUser);

        // Removes a task from the database.
        // Returns true if successful, false otherwise
        [OperationContract]
        bool RemoveTask(Guid TaskId);

        [OperationContract]
        bool RemoveUser(Guid U_Id);
    }

    [DataContract]
    public class User
    {
        [DataMember]
        public Guid Id;
        [DataMember]
        public String UserID;
        [DataMember]
        public String GroupID;
        [DataMember]
        public String Password;
        [DataMember]
        public String GroupPass;

        public User() { }
        public User(IdentityDataModel I)
        {
            Id = I.Id_new;
            UserID = I.UserId;
            GroupID = I.GroupId;
            Password = I.Password;
            GroupPass = I.GroupPass;
        }
    }

    [DataContract]
    public class Task
    {
        [DataMember]
        public Guid Id;
        [DataMember]
        public String Name;
        [DataMember]
        public DateTime Deadline;
        [DataMember]
        public DateTime ReminderTime;
        [DataMember]
        public String LocationType;
        [DataMember]
        public String UserID;
        [DataMember]
        public String GroupID;
        [DataMember]
        public String LocName;
        [DataMember]
        public Double LocLat;
        [DataMember]
        public Double LocLong;

        public Task() { }
        public Task(ToDoDataModel T)
        {
            Id = T.Id_new;
            Name = T.TaskName;
            Deadline = T.Deadline;
            LocationType = T.LocationType;
            UserID = T.UserId;
            GroupID = T.GroupId;
            ReminderTime = T.ReminderTime;
            LocName = T.LocName;
            LocLat = T.LocLat;
            LocLong = T.LocLong;
        }
    }

    [DataContract]
    public class Location {
        [DataMember]
        public String Name;
        [DataMember]
        public System.Device.Location.GeoCoordinate Coordinates;
        [DataMember]
        public String LocationType;
        [DataMember]
        public DateTime OpeningTime;
        [DataMember]
        public DateTime ClosingTime;

        public Location() { }
        public Location(LocationDataModel L)
        {
            Name = L.Place;
            Coordinates = new GeoCoordinate(L.Latitude, L.Longitude);
            LocationType = L.Type;
            OpeningTime = L.OpeningTime;
            ClosingTime = L.ClosingTime;
        }
    }

}
