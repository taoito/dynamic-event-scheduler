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
using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using WP9_DES.ServiceReference1;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Threading;

namespace WP9_DES
{
    public partial class MainPage : PhoneApplicationPage
    {

        SchedulerServiceClient client;

        EventHandler<GetAllTasksCompletedEventArgs> ListCompletedEventHandler;
        EventHandler<RemoveTaskCompletedEventArgs> RemoveTaskCompletedEventHandler;
        ObservableCollection<Task> listTasks = new ObservableCollection<Task>();
        ObservableCollection<string> groupIDs = new ObservableCollection<string>();

        EventHandler<GetNearbyLocationsCompletedEventArgs> NearbyCompletedEventHandler;
        ObservableCollection<Location> listLocations = new ObservableCollection<Location>();

        ObservableCollection<Location> nearLocList = new ObservableCollection<Location>();

        EventHandler<AddUserCompletedEventArgs> AddUserCompletedEventHandler;
        EventHandler<GetMyGroupsCompletedEventArgs> GetMyGroupsCompletedEventHandler;
        EventHandler<GetGroupMembersCompletedEventArgs> GetGroupMembersCompletedEventHandler;
        EventHandler<RemoveUserCompletedEventArgs> RemoveUserCompletedEventHandler;
        ObservableCollection<User> listGroups = new ObservableCollection<User>();
        ObservableCollection<User> listMembers = new ObservableCollection<User>();

        ObservableCollection<User> groupList = new ObservableCollection<User>();
        ObservableCollection<User> memberList = new ObservableCollection<User>();
        ObservableCollection<Notification> notifyList = new ObservableCollection<Notification>();
        
        App app = Application.Current as App;
        string myID;
        bool firstRefresh = false;
        bool validGroup = false;
        bool foundGroup = false;
        bool alreadyJoined = false;
        string refreshMessage = " ";
              
        GeoCoordinateWatcher watcher;
        GeoCoordinate tempLoc = new GeoCoordinate(44.9744, -93.2324);
        DispatcherTimer tmr3 = new DispatcherTimer();
        DispatcherTimer tmr4 = new DispatcherTimer();
        DispatcherTimer tmr5 = new DispatcherTimer();
        DispatcherTimer tmr6 = new DispatcherTimer();
        DispatcherTimer tmr7 = new DispatcherTimer();
        DispatcherTimer tmr8 = new DispatcherTimer();

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            client = new SchedulerServiceClient();
            myID = app.myUserID;

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            ListCompletedEventHandler = new EventHandler<GetAllTasksCompletedEventArgs>(client_ListCompleted);
            RemoveTaskCompletedEventHandler = new EventHandler<RemoveTaskCompletedEventArgs>(client_RemoveTaskCompleted);
            
            AddUserCompletedEventHandler = new EventHandler<AddUserCompletedEventArgs>(client_AddUserCompleted);
            GetMyGroupsCompletedEventHandler = new EventHandler<GetMyGroupsCompletedEventArgs>(client_GetMyGroupsCompleted);
            RemoveUserCompletedEventHandler = new EventHandler<RemoveUserCompletedEventArgs>(client_RemoveUserCompleted);
            GetGroupMembersCompletedEventHandler = new EventHandler<GetGroupMembersCompletedEventArgs>(client_GetGroupMembersCompleted);

            notifyListBox.ItemsSource = notifyList;
            groupListBox.ItemsSource = groupList;
            memberListBox.ItemsSource = memberList;

            usernameText.Text = "Username:  " + myID;

            DispatcherTimer tmr = new DispatcherTimer();
            tmr.Interval = TimeSpan.FromSeconds(45);
            tmr.Tick += OnTimerTick;
            tmr.Start();

            DispatcherTimer tmr2 = new DispatcherTimer();
            tmr2.Interval = TimeSpan.FromSeconds(5);
            tmr2.Tick += OnTimerTick2;
            tmr2.Start();

            tmr3.Interval = TimeSpan.FromMilliseconds(1000);
            tmr3.Tick += updateGroupList;
            tmr4.Interval = TimeSpan.FromMilliseconds(2000);
            tmr4.Tick += joinGroup;
            tmr5.Interval = TimeSpan.FromMilliseconds(2000);
            tmr5.Tick += createGroup;
            tmr6.Interval = TimeSpan.FromMilliseconds(2000);
            tmr6.Tick += updateTaskList;
            tmr7.Interval = TimeSpan.FromMilliseconds(2000);
            tmr7.Tick += viewMembers;
            tmr8.Interval = TimeSpan.FromMilliseconds(2000);
            tmr8.Tick += updateTaskList2;

            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High)
                {
                    MovementThreshold = 10
                };
                watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
                watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
                watcher.Start();
            }
            NearbyCompletedEventHandler = new EventHandler<GetNearbyLocationsCompletedEventArgs>(client_NearbyCompleted);

            firstRefresh = true;
            this.RefreshButton_Click(null, null);

            currentX.Text = app.myCurrentLoc.Latitude.ToString();
            currentY.Text = app.myCurrentLoc.Longitude.ToString();
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            tempLoc.Latitude = e.Position.Location.Latitude;
            tempLoc.Longitude = e.Position.Location.Longitude;

            if (app.useGPS)
            {
                currentX.Text = tempLoc.Latitude.ToString();
                currentY.Text = tempLoc.Longitude.ToString();
                app.myCurrentLoc.Latitude = tempLoc.Latitude;
                app.myCurrentLoc.Longitude = tempLoc.Longitude;
            }
        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        { }

        void OnTimerTick(object sender, EventArgs args)
        {
            client.GetAllTasksCompleted += ListCompletedEventHandler;
            client.GetAllTasksAsync(groupIDs);
            tmr8.Start();
        }

        void OnTimerTick2(object sender, EventArgs args)
        {
            
            client.GetNearbyLocationsCompleted += NearbyCompletedEventHandler;
            client.GetNearbyLocationsAsync(app.myCurrentLoc, 1);

            notifyList.Clear();
            for (int i = 0; i < listTasks.Count; i++)
            {
                string notifyLocType = listTasks[i].LocationType;
                if (notifyLocType == "Custom")
                {
                    notifyLocType = listTasks[i].LocName;
                }

                if (listTasks[i].Deadline.CompareTo(DateTime.Now) < 0)
                {
                    double timeDiff = DateTime.Now.Subtract(listTasks[i].Deadline).TotalHours;
                    string timeDiffStr = string.Format("{0:0.00}", timeDiff);
                    notifyList.Add(new Notification() { ID = listTasks[i].Id, Name = listTasks[i].Name, Deadline = listTasks[i].Deadline.DayOfWeek.ToString() + "  " + listTasks[i].Deadline.ToString(), LocType = notifyLocType, Reason = "Past deadline! " + timeDiffStr + " hours Late" });
                }
                else if (listTasks[i].ReminderTime.CompareTo(DateTime.Now) < 0)
                {
                    double timeDiff = listTasks[i].Deadline.Subtract(DateTime.Now).TotalHours;
                    string timeDiffStr = string.Format("{0:0.00}", timeDiff);
                    notifyList.Add(new Notification() { ID = listTasks[i].Id, Name = listTasks[i].Name, Deadline = listTasks[i].Deadline.DayOfWeek.ToString() + "  " + listTasks[i].Deadline.ToString(), LocType = notifyLocType, Reason = "It's Time! Due Soon in " + timeDiffStr + " hours" });
                }
            }
        }

        void client_RemoveUserCompleted(object sender, RemoveUserCompletedEventArgs e)
        {
            client.RemoveUserCompleted -= RemoveUserCompletedEventHandler;                       
        }

        void client_GetMyGroupsCompleted(object sender, GetMyGroupsCompletedEventArgs e)
        {
            listGroups = e.Result;
            client.GetMyGroupsCompleted -= GetMyGroupsCompletedEventHandler;
        }

        void client_GetGroupMembersCompleted(object sender, GetGroupMembersCompletedEventArgs e)
        {
            listMembers = e.Result;
            client.GetGroupMembersCompleted -= GetGroupMembersCompletedEventHandler;
            if (listMembers.Count > 0)
            {
                validGroup = true;
                foundGroup = true;
            }
            for (int i = 0; i < listMembers.Count; i++)
            {
                if (listMembers[i].UserID == myID)
                {
                    alreadyJoined = true;
                }
            }
        }
        
        void client_AddUserCompleted(object sender, AddUserCompletedEventArgs e)
        {
            client.AddUserCompleted -= AddUserCompletedEventHandler;
            if (e.Result)
            {
                if (joinCreateButton.Content.ToString() == "Join")
                {
                    joinCreateStatus.Text = "Joined: " + joinCreateGroupID.Text;
                }
                else if (joinCreateButton.Content.ToString() == "Create")
                {
                    joinCreateStatus.Text = "Created: " + joinCreateGroupID.Text;
                }
            }
        }

        void client_NearbyCompleted(object sender, GetNearbyLocationsCompletedEventArgs e)
        {
            listLocations = e.Result;
            client.GetNearbyLocationsCompleted -= NearbyCompletedEventHandler;

            //mainPivot.Title = listLocations.Count.ToString() + " nearby locations";
            for (int i = 0; i < listTasks.Count; i++)
            {
                nearLocList.Clear();

                double currentDistance;
                string notifyLocType = listTasks[i].LocationType;
                if (notifyLocType == "Custom")
                {
                    notifyLocType = listTasks[i].LocName;
                    currentDistance = app.myCurrentLoc.GetDistanceTo(new GeoCoordinate(listTasks[i].LocLat, listTasks[i].LocLong));
                    if (currentDistance < 1609.344)
                    {
                        nearLocList.Add(new Location() { Name = listTasks[i].LocName, Coordinates = new GeoCoordinate(listTasks[i].LocLat, listTasks[i].LocLong) });
                    }
                }
                
                for (int j = 0; j < listLocations.Count; j++)
                {
                    if (listTasks[i].LocationType == listLocations[j].LocationType)
                    {
                        nearLocList.Add(new Location() { Name = listLocations[j].Name, Coordinates = listLocations[j].Coordinates });
                    }
                }
                if (nearLocList.Count > 0)
                {
                    string nearReason = "It's Near! Check It Out at ";
                    for (int jj = 0; jj < nearLocList.Count; jj++)
                    {
                        string nearLat = string.Format("{0:0.00}", nearLocList[jj].Coordinates.Latitude);
                        string nearLong = string.Format("{0:0.00}", nearLocList[jj].Coordinates.Longitude);
                        double nearDist = app.myCurrentLoc.GetDistanceTo(new GeoCoordinate(nearLocList[jj].Coordinates.Latitude, nearLocList[jj].Coordinates.Longitude)) / 1609.344;
                        string nearDistStr = string.Format("{0:0.00}",nearDist);
                        nearReason = nearReason + nearLocList[jj].Name + "(" + nearLat + "," + nearLong + ") - Distance: " + nearDistStr + " miles";
                        
                        if ((jj + 1) < nearLocList.Count)
                        {
                            nearReason = nearReason + " and ";
                        }
                    }

                    bool foundNotification = false;
                    for (int ii = 0; ii < notifyList.Count; ii++)
                    {
                        if (listTasks[i].Id.Equals(notifyList[ii].ID))
                        {
                            string firstReason = notifyList[ii].Reason;
                            foundNotification = true;
                            notifyList.RemoveAt(ii);
                            notifyList.Add(new Notification() { ID = listTasks[i].Id, Name = listTasks[i].Name, Deadline = listTasks[i].Deadline.DayOfWeek.ToString() + "  " + listTasks[i].Deadline.ToString(), LocType = notifyLocType, Reason = firstReason, Reason2 = nearReason });
                            break;
                        }
                    }
                    if (!foundNotification)
                    {
                        notifyList.Add(new Notification() { ID = listTasks[i].Id, Name = listTasks[i].Name, Deadline = listTasks[i].Deadline.DayOfWeek.ToString() + "  " + listTasks[i].Deadline.ToString(), LocType = notifyLocType, Reason = nearReason });
                    }
                }
            }
        }

        void client_RemoveTaskCompleted(object sender, RemoveTaskCompletedEventArgs e)
        {
            client.RemoveTaskCompleted -= RemoveTaskCompletedEventHandler;
        }

        string imgSrc;
        int numList;

        void client_ListCompleted(object sender, GetAllTasksCompletedEventArgs e)
        {
            listTasks = e.Result;
            client.GetAllTasksCompleted -= ListCompletedEventHandler;
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            currentX.Text = app.myCurrentLoc.Latitude.ToString();
            currentY.Text = app.myCurrentLoc.Longitude.ToString();

            if (refreshMessage == "refreshTasksOnly")
            {
                refreshMessage = " ";
                client.GetAllTasksCompleted += ListCompletedEventHandler;
                client.GetAllTasksAsync(groupIDs);
                tmr6.Start();
            }
            else
            {
                client.GetMyGroupsCompleted += GetMyGroupsCompletedEventHandler;
                client.GetMyGroupsAsync(myID);
                tmr3.Start();
            }
        }

        private void AddTaskButton_Click(object sender, EventArgs e)
        {
            app.addOrEdit = "Add";
            this.NavigationService.Navigate(new Uri("/AddEditTask.xaml", UriKind.Relative));
        }

        private void EditMenuItem_Click(object sender, EventArgs e)
        {
            app.addOrEdit = "Edit";
            ItemViewModel listboxDataItem = (ItemViewModel)(sender as MenuItem).DataContext;
            app.editTaskName = listboxDataItem.LineOne;
            app.editTaskLocType = listboxDataItem.LineThree;
            app.editTaskID = listboxDataItem.TaskID;
            app.editTaskLat = listboxDataItem.LocLat;
            app.editTaskLong = listboxDataItem.LocLong;
            this.NavigationService.Navigate(new Uri("/AddEditTask.xaml", UriKind.Relative));
        }

        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            ItemViewModel listboxDataItem = (ItemViewModel)(sender as MenuItem).DataContext;
            client.RemoveTaskCompleted += RemoveTaskCompletedEventHandler;
            client.RemoveTaskAsync(listboxDataItem.TaskID);
            firstRefresh = true;
            tmr6.Start();
        }

        private void ShowMapMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Notification listboxDataItem = (Notification)(sender as MenuItem).DataContext;
            //app.myReason = listboxDataItem.Reason;
            app.myLocType = listboxDataItem.LocType;
            this.NavigationService.Navigate(new Uri("/MapPage.xaml", UriKind.Relative));
        }

        private void logOut_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
        }

        private void changeLoc_Click(object sender, RoutedEventArgs e)
        {
            //app.manualCurrentLoc = true;
            //app.myCurrentLoc = new GeoCoordinate(44.8615, -93.2475);
            ////currentX.Text = "44.8615";
            ////currentY.Text = "-93.2475";
            this.NavigationService.Navigate(new Uri("/MyLocation.xaml", UriKind.Relative));
        }

        private void joinGroup_Click(object sender, RoutedEventArgs e)
        {
            joinButton.FontSize = 21;
            createButton.FontSize = 17;
            textBlock9.Visibility = Visibility.Collapsed;
            joinCreateGroupPass2.Visibility = Visibility.Collapsed;
            joinCreateButton.Content = "Join";
            textBlock6.Text = "Enter Group ID:";
            textBlock7.Text = "Enter Password:";
        }

        private void createGroup_Click(object sender, RoutedEventArgs e)
        {
            createButton.FontSize = 21;
            joinButton.FontSize = 17;
            textBlock9.Visibility = Visibility.Visible;
            joinCreateGroupPass2.Visibility = Visibility.Visible;
            joinCreateButton.Content = "Create";
            textBlock6.Text = "New Group ID:";
            textBlock7.Text = "New Password:";
        }

        private void joinCreateGroup_Click(object sender, RoutedEventArgs e)
        {
            if (joinCreateButton.Content.ToString() == "Join")
            {
                if (joinCreateGroupID.Text == "") 
                {
                    joinCreateStatus.Text = "Please Enter GroupID";
                }
                else
                {
                    validGroup = false;
                    alreadyJoined = false;
                    client.GetGroupMembersCompleted += GetGroupMembersCompletedEventHandler;
                    client.GetGroupMembersAsync(joinCreateGroupID.Text);

                    tmr4.Start();
                }
            }
            else if (joinCreateButton.Content.ToString() == "Create")
            {
                if (joinCreateGroupPass.Text != joinCreateGroupPass2.Text)
                {
                    joinCreateStatus.Text = "Error: Passwords do not match";
                }
                else if (joinCreateGroupID.Text == "")
                {
                    joinCreateStatus.Text = "Please Enter a Group ID";
                }
                else
                {
                    foundGroup = false;
                    client.GetGroupMembersCompleted += GetGroupMembersCompletedEventHandler;
                    client.GetGroupMembersAsync(joinCreateGroupID.Text);

                    tmr5.Start();
                }
            }       
        }

        private void ViewMemberMenu_Click(object sender, RoutedEventArgs e)
        {
            User listboxDataItem = (User)(sender as MenuItem).DataContext;
            memberText.Text = listboxDataItem.GroupID + " members: ";

            client.GetGroupMembersCompleted += GetGroupMembersCompletedEventHandler;
            client.GetGroupMembersAsync(listboxDataItem.GroupID);
            tmr7.Start();         
        }

        void viewMembers(object sender, EventArgs args)
        {
            tmr7.Stop();
            memberList.Clear();
            for (int i = 0; i < listMembers.Count; i++)
            {
                memberList.Add(new User() { UserID = listMembers[i].UserID });
            }
            memberText.Text = memberText.Text + listMembers.Count.ToString();
        }

        private void LeaveGroupMenu_Click(object sender, RoutedEventArgs e)
        {
            User listboxDataItem = (User)(sender as MenuItem).DataContext;

            if (listboxDataItem.GroupID == myID)
            {
                joinCreateStatus.Text = "Can't leave your personal group";
            }
            else
            {
                client.RemoveUserCompleted += RemoveUserCompletedEventHandler;
                client.RemoveUserAsync(listboxDataItem.Id);

                client.GetMyGroupsCompleted += GetMyGroupsCompletedEventHandler;
                client.GetMyGroupsAsync(myID);
                tmr3.Start();
                firstRefresh = true;
                myGroupsText.Text = "My Groups:  updating ..";
                joinCreateStatus.Text = "Left group: " + listboxDataItem.GroupID;
            }
        }

        void updateGroupList(object sender, EventArgs args)
        {
            mainPivot.SelectedIndex = 2;
            tmr3.Stop();
            groupList.Clear();
            if (firstRefresh)
            {
                myGroupsText.Text = "My Groups:  updating ....";
            }
            else
            {
                myGroupsText.Text = "My Groups:  " + listGroups.Count.ToString();
            }

            groupIDs.Clear();
            for (int i = 0; i < listGroups.Count; i++)
            {
                groupList.Add(new User() { Id = listGroups[i].Id, UserID = listGroups[i].UserID, GroupID = listGroups[i].GroupID, Password = listGroups[i].Password, GroupPass = listGroups[i].GroupPass });
                groupIDs.Add(listGroups[i].GroupID);
            }

            if (firstRefresh)
            {
                firstRefresh = false;
                refreshMessage = " ";
                this.RefreshButton_Click(null, null);
            }
            else
            {
                client.GetAllTasksCompleted += ListCompletedEventHandler;
                client.GetAllTasksAsync(groupIDs);
                firstRefresh = true;
                tmr6.Start();
            }
        }

        void joinGroup(object sender, EventArgs args)
        {
            tmr4.Stop();
            if (!validGroup)
            {
                joinCreateStatus.Text = "Error: Group doesn't exist.";
            }
            else if (alreadyJoined)
            {
                joinCreateStatus.Text = "Error: Already in this group.";
            }
            else if (joinCreateGroupPass.Text != listMembers[0].GroupPass)
            {
                joinCreateStatus.Text = "Access Denied. Invalid Group Password";
            }
            else
            {
                User TheUser = new User();
                TheUser.Id = Guid.NewGuid();
                TheUser.UserID = myID;
                TheUser.GroupID = joinCreateGroupID.Text;
                TheUser.Password = app.myPass;
                TheUser.GroupPass = listMembers[0].GroupPass;

                client.AddUserCompleted += AddUserCompletedEventHandler;
                client.AddUserAsync(TheUser);

                client.GetMyGroupsCompleted += GetMyGroupsCompletedEventHandler;
                client.GetMyGroupsAsync(myID);
                tmr3.Start();
                firstRefresh = true;
                myGroupsText.Text = "My Groups:  updating ..";
            }
        }

        void createGroup(object sender, EventArgs args)
        {
            tmr5.Stop();
            if (foundGroup)
            {
                joinCreateStatus.Text = "Error: Group already existed.";
            }
            else
            {
                User TheUser = new User();
                TheUser.Id = Guid.NewGuid();
                TheUser.UserID = myID;
                TheUser.GroupID = joinCreateGroupID.Text;
                TheUser.Password = app.myPass;
                TheUser.GroupPass = joinCreateGroupPass.Text;

                client.AddUserCompleted += AddUserCompletedEventHandler;
                client.AddUserAsync(TheUser);

                client.GetMyGroupsCompleted += GetMyGroupsCompletedEventHandler;
                client.GetMyGroupsAsync(myID);
                tmr3.Start();
                firstRefresh = true;
                myGroupsText.Text = "My Groups:  updating ..";
            }
        }

        void updateTaskList(object sender, EventArgs args)
        {
            mainPivot.SelectedIndex = 1;
            tmr6.Stop();

            App.ViewModel.Items.Clear();
            numList = 1;
            for (int i = 0; i < listTasks.Count; i++)
            {
                if (listTasks[i].LocationType == "Bookstore")
                {
                    imgSrc = "/WP9-DES;component/Pic/bookstore.jpg";
                }
                else if (listTasks[i].LocationType == "Grocery Store")
                {
                    imgSrc = "/WP9-DES;component/Pic/grocery.jpg";
                }
                else if (listTasks[i].LocationType == "Electronics Store")
                {
                    imgSrc = "/WP9-DES;component/Pic/electronics.jpg";
                }
                else if (listTasks[i].LocationType == "Barbershop")
                {
                    imgSrc = "/WP9-DES;component/Pic/barbershop.jpg";
                }
                else
                {
                    imgSrc = "/WP9-DES;component/Pic/any.jpg";
                }

                if (listTasks[i].GroupID == myID)
                {
                    if (listTasks[i].LocationType == "Custom")
                    {
                        App.ViewModel.Items.Add(new ItemViewModel() { LineOne = listTasks[i].Name, LineTwo = listTasks[i].Deadline.DayOfWeek.ToString() + "  " + listTasks[i].Deadline.ToString(), LineThree = listTasks[i].LocName, ImageSource = imgSrc, NumIndex = numList, TaskID = listTasks[i].Id, UserID = "By '" + listTasks[i].UserID + "'", GroupID = " Personal group", LocLat = listTasks[i].LocLat.ToString(), LocLong = listTasks[i].LocLong.ToString() });
                    }
                    else
                    {
                        App.ViewModel.Items.Add(new ItemViewModel() { LineOne = listTasks[i].Name, LineTwo = listTasks[i].Deadline.DayOfWeek.ToString() + "  " + listTasks[i].Deadline.ToString(), LineThree = listTasks[i].LocationType, ImageSource = imgSrc, NumIndex = numList, TaskID = listTasks[i].Id, UserID = "By '" + listTasks[i].UserID + "'", GroupID = " Personal group", LocLat = listTasks[i].LocLat.ToString(), LocLong = listTasks[i].LocLong.ToString() });
                    }
                }
                else
                {
                    if (listTasks[i].LocationType == "Custom")
                    {
                        App.ViewModel.Items.Add(new ItemViewModel() { LineOne = listTasks[i].Name, LineTwo = listTasks[i].Deadline.DayOfWeek.ToString() + "  " + listTasks[i].Deadline.ToString(), LineThree = listTasks[i].LocName, ImageSource = imgSrc, NumIndex = numList, TaskID = listTasks[i].Id, UserID = "By '" + listTasks[i].UserID + "'", GroupID = " " + listTasks[i].GroupID + " group", LocLat = listTasks[i].LocLat.ToString(), LocLong = listTasks[i].LocLong.ToString() });
                    }
                    else
                    {
                        App.ViewModel.Items.Add(new ItemViewModel() { LineOne = listTasks[i].Name, LineTwo = listTasks[i].Deadline.DayOfWeek.ToString() + "  " + listTasks[i].Deadline.ToString(), LineThree = listTasks[i].LocationType, ImageSource = imgSrc, NumIndex = numList, TaskID = listTasks[i].Id, UserID = "By '" + listTasks[i].UserID + "'", GroupID = " " + listTasks[i].GroupID + " group", LocLat = listTasks[i].LocLat.ToString(), LocLong = listTasks[i].LocLong.ToString() });
                    }
                }
                numList++;
            }
            if (firstRefresh)
            {
                firstRefresh = false;
                refreshMessage = "refreshTasksOnly";
                this.RefreshButton_Click(null, null);
            }
        }


        void updateTaskList2(object sender, EventArgs args)
        {
            tmr8.Stop();
            App.ViewModel.Items.Clear();
            numList = 1;
            for (int i = 0; i < listTasks.Count; i++)
            {
                if (listTasks[i].LocationType == "Bookstore")
                {
                    imgSrc = "/WP9-DES;component/Pic/bookstore.jpg";
                }
                else if (listTasks[i].LocationType == "Grocery Store")
                {
                    imgSrc = "/WP9-DES;component/Pic/grocery.jpg";
                }
                else if (listTasks[i].LocationType == "Electronics Store")
                {
                    imgSrc = "/WP9-DES;component/Pic/electronics.jpg";
                }
                else if (listTasks[i].LocationType == "Barbershop")
                {
                    imgSrc = "/WP9-DES;component/Pic/barbershop.jpg";
                }
                else
                {
                    imgSrc = "/WP9-DES;component/Pic/any.jpg";
                }

                if (listTasks[i].GroupID == myID)
                {
                    if (listTasks[i].LocationType == "Custom")
                    {
                        App.ViewModel.Items.Add(new ItemViewModel() { LineOne = listTasks[i].Name, LineTwo = listTasks[i].Deadline.DayOfWeek.ToString() + "  " + listTasks[i].Deadline.ToString(), LineThree = listTasks[i].LocName, ImageSource = imgSrc, NumIndex = numList, TaskID = listTasks[i].Id, UserID = "By '" + listTasks[i].UserID + "'", GroupID = " Personal group", LocLat = listTasks[i].LocLat.ToString(), LocLong = listTasks[i].LocLong.ToString() });
                    }
                    else
                    {
                        App.ViewModel.Items.Add(new ItemViewModel() { LineOne = listTasks[i].Name, LineTwo = listTasks[i].Deadline.DayOfWeek.ToString() + "  " + listTasks[i].Deadline.ToString(), LineThree = listTasks[i].LocationType, ImageSource = imgSrc, NumIndex = numList, TaskID = listTasks[i].Id, UserID = "By '" + listTasks[i].UserID + "'", GroupID = " Personal group", LocLat = listTasks[i].LocLat.ToString(), LocLong = listTasks[i].LocLong.ToString() });
                    }
                }
                else
                {
                    if (listTasks[i].LocationType == "Custom")
                    {
                        App.ViewModel.Items.Add(new ItemViewModel() { LineOne = listTasks[i].Name, LineTwo = listTasks[i].Deadline.DayOfWeek.ToString() + "  " + listTasks[i].Deadline.ToString(), LineThree = listTasks[i].LocName, ImageSource = imgSrc, NumIndex = numList, TaskID = listTasks[i].Id, UserID = "By '" + listTasks[i].UserID + "'", GroupID = " " + listTasks[i].GroupID + " group", LocLat = listTasks[i].LocLat.ToString(), LocLong = listTasks[i].LocLong.ToString() });
                    }
                    else
                    {
                        App.ViewModel.Items.Add(new ItemViewModel() { LineOne = listTasks[i].Name, LineTwo = listTasks[i].Deadline.DayOfWeek.ToString() + "  " + listTasks[i].Deadline.ToString(), LineThree = listTasks[i].LocationType, ImageSource = imgSrc, NumIndex = numList, TaskID = listTasks[i].Id, UserID = "By '" + listTasks[i].UserID + "'", GroupID = " " + listTasks[i].GroupID + " group", LocLat = listTasks[i].LocLat.ToString(), LocLong = listTasks[i].LocLong.ToString() });
                    }
                }
                numList++;
            }
        }
    }
}