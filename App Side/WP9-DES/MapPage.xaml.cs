using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Diagnostics; //---for Debug.WriteLine()---
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
using WP9_DES.ServiceReference1;
using System.Collections.ObjectModel;

namespace WP9_DES
{
    public partial class MapPage : PhoneApplicationPage
    {
        SchedulerServiceClient client = new SchedulerServiceClient();
        EventHandler<GetNearbyLocationsCompletedEventArgs> NearbyCompletedEventHandler;
      
        ObservableCollection<Location> listLocation;
        Pushpin pin1 = new Pushpin();
        App app = Application.Current as App;

        GeoCoordinateWatcher watcher;
        // Constructor
        public MapPage()
        {
            InitializeComponent();
             
            if (watcher == null)
            {
                //---get the highest accuracy---
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High)
                {
                    //---the minimum distance (in meters) to travel before the next 
                    // location update---
                    MovementThreshold = 10
                };

                //---event to fire when a new position is obtained---
                watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);

                //---event to fire when there is a status change in the location 
                // service API---
                watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
                watcher.Start();
                
                map1.ZoomBarVisibility = System.Windows.Visibility.Visible;
                map1.Mode = new Microsoft.Phone.Controls.Maps.RoadMode();

                map1.Center = new GeoCoordinate(47.676289396624654, -99.12096571922302);
                map1.ZoomLevel = 22;
 
//---create a new pushpin---
//Pushpin pin = new Pushpin();
 
//---set the location for the pushpin---
//pin.Location = new GeoCoordinate(47.676289396624654, -122.12096571922302);
//pin.Location = watcher.Position;
//map1.Center = new GeoCoordinate(
//e.Position.Location.Latitude, e.Position.Location.Longitude);
//pin.Location = map1.Center;

//---add the pushpin to the map---
//map1.Children.Add(pin);
                     
            }
            NearbyCompletedEventHandler = new EventHandler<GetNearbyLocationsCompletedEventArgs>(client_NearbyCompleted);
            client = new SchedulerServiceClient();
        }

        private void MapPage_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
        
        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    Debug.WriteLine("disabled");
                    break;
                case GeoPositionStatus.Initializing:
                    Debug.WriteLine("initializing");
                    break;
                case GeoPositionStatus.NoData:
                    Debug.WriteLine("nodata");
                    break;
                case GeoPositionStatus.Ready:
                    Debug.WriteLine("ready");
                    break;
            }
        }
        
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Debug.WriteLine("({0},{1})",
            e.Position.Location.Latitude, e.Position.Location.Longitude);

            map1.Center = new GeoCoordinate(
            e.Position.Location.Latitude, e.Position.Location.Longitude);

            
            pin1.Location = new GeoCoordinate(
            e.Position.Location.Latitude, e.Position.Location.Longitude);
            pin1.Content = "Current Location";
          // map1.Children.Add(pin1);
            map1.SetView(pin1.Location, 18);

             if (app.myLocType == "Grocery Store")
           {
           Pushpin pinx = new Pushpin();
           pinx.Location = new GeoCoordinate(44.9736, -93.2299);
           pinx.Content = "Harvard Market";
           map1.Children.Add(pinx);

           Pushpin piny = new Pushpin();
           piny.Location = new GeoCoordinate(44.9615, -93.1660);
           piny.Content = "Rainbow";
           map1.Children.Add(piny);

           Pushpin pinz = new Pushpin();
           pinz.Location = new GeoCoordinate(44.9550 ,-93.1609);
           pinz.Content = "Walmart";
           map1.Children.Add(pinz);

           Pushpin pinzx = new Pushpin();
           pinzx.Location = new GeoCoordinate(44.9527, -93.1763);
           pinzx.Content = "Cub";
           map1.Children.Add(pinzx);
           }

           if (app.myLocType == "Barbershop")
           {
               Pushpin pinzz = new Pushpin();
               pinzz.Location = new GeoCoordinate(44.9749, -93.2287);
               pinzz.Content = "Great Clips";
               map1.Children.Add(pinzz);
           }

           if (app.myLocType == "Bookstore")
           {
               Pushpin pinzxw = new Pushpin();
           pinzxw.Location = new GeoCoordinate(44.9728,-93.2354);
           pinzxw.Content = "UMN Bookstore";
           map1.Children.Add(pinzxw);

               Pushpin pinzx2 = new Pushpin();
           pinzx2.Location = new GeoCoordinate(44.8529,-93.2461);
           pinzx2.Content = "Barnes and Noble";
           map1.Children.Add(pinzx2);

           }
           if (app.myLocType == "Electronics Store")
           {
               Pushpin pinyy = new Pushpin();
               pinyy.Location = new GeoCoordinate(44.8533, -93.2392);
               pinyy.Content = "Radio Shack";
               map1.Children.Add(pinyy);

               Pushpin pinys = new Pushpin();
               pinys.Location = new GeoCoordinate(44.8554, -93.2435);
               pinys.Content = "BestBuy";
               map1.Children.Add(pinys);

           }
            /*
            Pushpin pin2 = new Pushpin();
            pin2.Location = new GeoCoordinate(
            e.Position.Location.Latitude + 0.01, e.Position.Location.Longitude - 0.01);
            pin2.Content = "Target";
            map1.Children.Add(pin2);

            //map1.SetView(pin2.Location, 14);

            Pushpin pin3 = new Pushpin();
            pin3.Location = new GeoCoordinate(
            e.Position.Location.Latitude - 0.014, e.Position.Location.Longitude + 0.017);
            pin3.Content = "Rainbow";
            map1.Children.Add(pin3);

            //map1.SetView(pin3.Location, 14);

            Pushpin pin4 = new Pushpin();
            pin4.Location = new GeoCoordinate(
            e.Position.Location.Latitude - 0.0156, e.Position.Location.Longitude - 0.0123);
            pin4.Content = "Walmart";
            map1.Children.Add(pin4);
            
            */
         
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
           
            client.GetNearbyLocationsCompleted += NearbyCompletedEventHandler;
            client.GetNearbyLocationsAsync(pin1.Location, 2);
        }

        void client_NearbyCompleted(object sender, GetNearbyLocationsCompletedEventArgs e)
        {
            listLocation = e.Result;
            PageTitle.Text = listLocation.Count.ToString() ;
            int number = listLocation.Count;

            Pushpin pin = new Pushpin();
            pin.Location = new GeoCoordinate(pin1.Location.Latitude, pin1.Location.Longitude);
            pin.Content = "Current Location";
            map1.Children.Add(pin);
            map1.SetView(pin.Location, 15);
            

            if (number != 0)
            {
                Pushpin pin2 = new Pushpin();
                pin2.Location = new GeoCoordinate(listLocation[0].Coordinates.Latitude, listLocation[0].Coordinates.Longitude);
                pin2.Content = listLocation[0].Name;
                map1.Children.Add(pin2);
                number--;
            }
            //map1.SetView(pin2.Location, 14);
            if (number != 0)
            {
                Pushpin pin3 = new Pushpin();
                pin3.Location = new GeoCoordinate(listLocation[1].Coordinates.Latitude, listLocation[1].Coordinates.Longitude);
                pin3.Content = listLocation[1].Name;
                map1.Children.Add(pin3);
                number--;
            }
            if (number != 0)
            {
                Pushpin pin4 = new Pushpin();
                pin4.Location = new GeoCoordinate(listLocation[2].Coordinates.Latitude, listLocation[2].Coordinates.Longitude);
                pin4.Content = listLocation[2].Name;
                map1.Children.Add(pin4);
                number--;
            }
            if (number != 0)
            {
                Pushpin pin5 = new Pushpin();
                pin5.Location = new GeoCoordinate(listLocation[3].Coordinates.Latitude, listLocation[3].Coordinates.Longitude);
                pin5.Content = listLocation[3].Name;
                map1.Children.Add(pin5);
                number--;
            }
           
            client.GetNearbyLocationsCompleted -= NearbyCompletedEventHandler;
        }

    

      
    }
}