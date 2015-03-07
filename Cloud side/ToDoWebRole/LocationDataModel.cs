using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.StorageClient;
using System.Device.Location;

namespace ToDoWebRole
{
	public class LocationDataModel : TableServiceEntity
	{

		public LocationDataModel(Guid partitionKey, string rowKey)
			: base(partitionKey.ToString(), rowKey)
		{
			this.Id_new = partitionKey;
		}

		public LocationDataModel(string name)
			: this()
		{
			this.Place = name;
		}

		public LocationDataModel()
			: this(Guid.NewGuid(), String.Empty)
		{
		}

		public Guid Id_new
		{
			get;
			set;
		}

		public string Place { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
		public string Type { get; set; }
        public DateTime OpeningTime { get; set; }
        public DateTime ClosingTime { get; set; }

		public readonly static Guid ROOT_ID = new Guid("11111111-1111-1111-1111-111111111111");
		
	}
}