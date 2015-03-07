using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Device.Location;

namespace ToDoWebRole
{
    public class LocationDataSource
    {
        /***/
        //Azure Table Storage source
          private LocationDataServiceContext context = null;

            //Connect to storage. Create context. Lazy initialize table.
            public LocationDataSource()
            {
                //Connect to storage
                CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
     configSetter(ConfigurationManager.AppSettings[configName]));
                //Aditi commented this var sa = CloudStorageAccount.FromConfigurationSetting("AzureConnectionString");
                var sa = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
                //Create context
                context = new LocationDataServiceContext(sa.TableEndpoint.ToString(), sa.Credentials);

                //Ensure that table exists
                var cloudClient = sa.CreateCloudTableClient();
                if (cloudClient.DoesTableExist(LocationDataServiceContext.LocationTableName) == false)
                {
                    //Strictly speaking, this is a race condition -- table could come into existence from other source while this block executes. We can ignore it for our purposes. 
                    cloudClient.CreateTableIfNotExist(LocationDataServiceContext.LocationTableName);

                }

                List<LocationDataModel> TempList = context.LocationTable.ToList<LocationDataModel>();

                // Delete all entries from the database.  Used during development, must be commented out for production.
                //foreach (LocationDataModel Loc in TempList)
                //    context.DeleteObject(Loc);
                //context.SaveChanges();

                //Ensure that the _ROOT task exists. Ditto race condition possibility, ditto ignore.
                if (!TempList.Any(t => t.Id_new.Equals(LocationDataModel.ROOT_ID)))
                {
                    var root = new LocationDataModel(LocationDataModel.ROOT_ID, String.Empty);
                    root.Id_new = LocationDataModel.ROOT_ID;
                    root.Place = "ROOT - Don't use";
                    root.Latitude = 0;
                    root.Longitude = 0;
                    root.OpeningTime = DateTime.Now;
                    root.ClosingTime = DateTime.Now;
                    Insert(root);

                    LocationDataModel Place1 = new LocationDataModel();
                    Place1.Place = "Harvard Market";
                    Place1.Latitude = 44.9736;
                    Place1.Longitude = -93.2299;
                    Place1.Type = "Grocery Store";
                    Place1.OpeningTime = new DateTime(2010, 1, 1, 9, 0, 0);
                    Place1.ClosingTime = new DateTime(2010, 1, 1, 22, 0, 0);

                    LocationDataModel Place2 = new LocationDataModel();
                    Place2.Place = "Great Clips";
                    Place2.Latitude = 44.9749;
                    Place2.Longitude = -93.2287;
                    Place2.Type = "Barbershop";
                    Place2.OpeningTime = new DateTime(2010, 1, 1, 1, 0, 0);
                    Place2.ClosingTime = new DateTime(2010, 1, 1, 23, 0, 0);

                    LocationDataModel Place3 = new LocationDataModel();
                    Place3.Place = "UMN Bookstore";
                    Place3.Latitude = 44.9728;
                    Place3.Longitude = -93.2354;
                    Place3.Type = "Bookstore";
                    Place3.OpeningTime = new DateTime(2010, 1, 1, 10, 0, 0);
                    Place3.ClosingTime = new DateTime(2010, 1, 1, 20, 0, 0);

                    LocationDataModel Place4 = new LocationDataModel();
                    Place4.Place = "Walmart";
                    Place4.Latitude = 44.9550;
                    Place4.Longitude = -93.1609;
                    Place4.Type = "Grocery Store";
                    Place4.OpeningTime = new DateTime(2010, 1, 1, 10, 0, 0);
                    Place4.ClosingTime = new DateTime(2010, 1, 1, 22, 0, 0);

                    LocationDataModel Place5 = new LocationDataModel();
                    Place5.Place = "Cub Foods";
                    Place5.Latitude = 44.9527;
                    Place5.Longitude = -93.1763;
                    Place5.Type = "Grocery Store";
                    Place5.OpeningTime = new DateTime(2010, 1, 1, 10, 0, 0);
                    Place5.ClosingTime = new DateTime(2010, 1, 1, 22, 0, 0);

                    LocationDataModel Place6 = new LocationDataModel();
                    Place6.Place = "Rainbow Foods";
                    Place6.Latitude = 44.9615;
                    Place6.Longitude = -93.1660;
                    Place6.Type = "Grocery Store";
                    Place6.OpeningTime = new DateTime(2010, 1, 1, 10, 0, 0);
                    Place6.ClosingTime = new DateTime(2010, 1, 1, 22, 0, 0);

                    LocationDataModel Place7 = new LocationDataModel();
                    Place7.Place = "BestBuy";
                    Place7.Latitude = 44.8554;
                    Place7.Longitude = -93.2435;
                    Place7.Type = "Electronics Store";
                    Place7.OpeningTime = new DateTime(2010, 1, 1, 10, 0, 0);
                    Place7.ClosingTime = new DateTime(2010, 1, 1, 22, 0, 0);

                    LocationDataModel Place8 = new LocationDataModel();
                    Place8.Place = "RadioShack";
                    Place8.Latitude = 44.8533;
                    Place8.Longitude = -93.2392;
                    Place8.Type = "Electronics Store";
                    Place8.OpeningTime = new DateTime(2010, 1, 1, 10, 0, 0);
                    Place8.ClosingTime = new DateTime(2010, 1, 1, 22, 0, 0);

                    LocationDataModel Place9 = new LocationDataModel();
                    Place9.Place = "Barnes & Noble";
                    Place9.Latitude = 44.8529;
                    Place9.Longitude = -93.2461;
                    Place9.Type = "Bookstore";
                    Place9.OpeningTime = new DateTime(2010, 1, 1, 10, 0, 0);
                    Place9.ClosingTime = new DateTime(2010, 1, 1, 22, 0, 0);

                    Insert(Place1);
                    Insert(Place2);
                    Insert(Place3);
                    Insert(Place4);
                    Insert(Place5);
                    Insert(Place6);
                    Insert(Place7);
                    Insert(Place8);
                    Insert(Place9);
                }
            }

            //Generic "Select"
            public IEnumerable<LocationDataModel> Select()
            {
                var rs = from todo in context.LocationTable select todo;
                var query = rs.AsTableServiceQuery<LocationDataModel>();
                return query.Execute();
            }

            private void Insert(LocationDataModel loc)
            {
                context.AddObject(LocationDataServiceContext.LocationTableName, loc);
                context.SaveChanges();
            }

        // Dist in miles
        public List<Location> GetNearbyLocations(GeoCoordinate Point, float Dist)
        {
            float DistInMeters = Dist * 1609.344F;
            IQueryable<LocationDataModel> q = from loc in context.LocationTable select loc;
            CloudTableQuery<LocationDataModel> q2 = q.AsTableServiceQuery<LocationDataModel>();
            IEnumerable<LocationDataModel> ans = q2.Execute();
            List<LocationDataModel> ansList = ans.ToList<LocationDataModel>();
            List<Location> outList = new List<Location>();
            foreach (LocationDataModel Loc in ansList)
                if (Loc.Id_new != LocationDataModel.ROOT_ID) {
                    double Space = Point.GetDistanceTo(new GeoCoordinate(Loc.Latitude, Loc.Longitude));
                    if (Space < DistInMeters)
                        outList.Add(new Location(Loc));
                }
            return outList;
        }
    }
}