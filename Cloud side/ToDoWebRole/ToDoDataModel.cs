using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;

namespace ToDoWebRole
{
	public class ToDoDataModel : TableServiceEntity
	{

		public ToDoDataModel(Guid partitionKey, string rowKey)
			: base(partitionKey.ToString(), rowKey)
		{
			this.Id_new = partitionKey;
		}

		public ToDoDataModel()
			: this(Guid.NewGuid(), String.Empty)
		{
		}

		public ToDoDataModel(Task TheTask)
		{
			this.Id_new = TheTask.Id;
			this.PartitionKey = this.Id_new.ToString();
			this.RowKey = String.Empty;
			this.TaskName = TheTask.Name;
            this.Deadline = TheTask.Deadline;
            this.ReminderTime = TheTask.ReminderTime;
			this.LocationType = TheTask.LocationType;
            this.UserId = TheTask.UserID;
            this.GroupId = TheTask.GroupID;
            this.LocName = TheTask.LocName;
            this.LocLat = TheTask.LocLat;
            this.LocLong = TheTask.LocLong;
            ReminderTime = ReminderTime.AddHours(-5);
            Deadline = Deadline.AddHours(-5);
		}

		public Guid Id_new
		{
			get;
			set;
		}

		public string TaskName { get; set; }
        public System.DateTime Deadline { get; set; }
        public System.DateTime ReminderTime { get; set; }
		public string LocationType { get; set; }
        public string LocName { get; set; }
		public string UserId { get; set; }
		public string GroupId { get; set; }
        public double LocLat { get; set; }
        public double LocLong { get; set; }
		public readonly static Guid ROOT_ID = new Guid("11111111-1111-1111-1111-111111111111");
	}
}
