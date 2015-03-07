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
using WP9_DES.ServiceReference1;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace WP9_DES
{
    public partial class AddEditTask : PhoneApplicationPage
    {
        SchedulerServiceClient client;

        EventHandler<GetMyGroupsCompletedEventArgs> GetMyGroupsCompletedEventHandler;
        ObservableCollection<User> listGroups = new ObservableCollection<User>();
        ObservableCollection<User> groupList = new ObservableCollection<User>();

        DispatcherTimer tmr = new DispatcherTimer();

        EventHandler<AddTaskCompletedEventArgs> AddTaskCompletedEventHandler;
        EventHandler<RemoveTaskCompletedEventArgs> RemoveTaskCompletedEventHandler;
        string locType = "Grocery Store";
        DateTime deadline = DateTime.Now;
        App app = Application.Current as App;
        string groupType = "thao";

        public AddEditTask()
        {
            InitializeComponent();
            client = new SchedulerServiceClient();

            groupType = app.myUserID;
            groupListBox.ItemsSource = groupList;
          
            if (app.addOrEdit == "Edit")
            {
                PageTitle.Text = "Edit Task";
                nameTextBox.Text = app.editTaskName;
                addEditButton.Content = "Edit";
                if (app.editTaskLocType == "Grocery Store")
                {
                    grocRadioButton.IsChecked = true;
                }
                else if (app.editTaskLocType == "Bookstore")
                {
                    bookstoreRadioButton.IsChecked = true;
                }
                else if (app.editTaskLocType == "Electronics Store")
                {
                    electronicsRadioButton.IsChecked = true;
                }
                else if (app.editTaskLocType == "Barbershop")
                {
                    barberRadioButton.IsChecked = true;
                }
                else
                {
                    customLocButton.IsChecked = true;
                    customNameText.Text = app.editTaskLocType;
                    customLatText.Text = app.editTaskLat;
                    customLongText.Text = app.editTaskLong;
                }
            }

            AddTaskCompletedEventHandler = new EventHandler<AddTaskCompletedEventArgs>(client_AddTaskCompleted);
            RemoveTaskCompletedEventHandler = new EventHandler<RemoveTaskCompletedEventArgs>(client_RemoveTaskCompleted);
            GetMyGroupsCompletedEventHandler = new EventHandler<GetMyGroupsCompletedEventArgs>(client_GetMyGroupsCompleted);

            tmr.Interval = TimeSpan.FromMilliseconds(2000);
            tmr.Tick += updateGroupList;

            client.GetMyGroupsCompleted += GetMyGroupsCompletedEventHandler;
            client.GetMyGroupsAsync(app.myUserID);
            this.updateGroupList(null, null);
            tmr.Start();

            InputScope Keyboard = new InputScope();
            InputScope Keyboard2 = new InputScope();
            InputScope Keyboard3 = new InputScope();
            InputScope Keyboard4 = new InputScope();
            InputScope Keyboard5 = new InputScope();
            InputScopeName ScopeName = new InputScopeName();
            InputScopeName ScopeName2 = new InputScopeName();
            InputScopeName ScopeName3 = new InputScopeName();
            InputScopeName ScopeName4 = new InputScopeName();
            InputScopeName ScopeName5 = new InputScopeName();
            ScopeName.NameValue = InputScopeNameValue.TelephoneNumber;
            ScopeName2.NameValue = InputScopeNameValue.TelephoneNumber;
            ScopeName3.NameValue = InputScopeNameValue.TelephoneNumber;
            ScopeName4.NameValue = InputScopeNameValue.Number;
            ScopeName5.NameValue = InputScopeNameValue.Number;
            Keyboard.Names.Add(ScopeName);
            Keyboard2.Names.Add(ScopeName2);
            Keyboard3.Names.Add(ScopeName3);
            Keyboard4.Names.Add(ScopeName4);
            Keyboard5.Names.Add(ScopeName5);
            reminderHour.InputScope = Keyboard;
            reminderMin.InputScope = Keyboard2;
            reminderDay.InputScope = Keyboard3;
            customLatText.InputScope = Keyboard4;
            customLongText.InputScope = Keyboard5;
        }

        void client_GetMyGroupsCompleted(object sender, GetMyGroupsCompletedEventArgs e)
        {
            listGroups = e.Result;
            client.GetMyGroupsCompleted -= GetMyGroupsCompletedEventHandler;
        }

        void client_RemoveTaskCompleted(object sender, RemoveTaskCompletedEventArgs e)
        {
            client.RemoveTaskCompleted -= RemoveTaskCompletedEventHandler;
        }

        void updateGroupList(object sender, EventArgs args)
        {
            tmr.Stop();
            groupList.Clear();
            for (int i = 0; i < listGroups.Count; i++)
            {
                if (listGroups[i].GroupID == app.myUserID)
                {
                    groupList.Add(new User() { Id = listGroups[i].Id, GroupID = "Personal"});
                }
                else
                {
                    groupList.Add(new User() { Id = listGroups[i].Id, GroupID = listGroups[i].GroupID});
                }
            }
            
        }

        void client_AddTaskCompleted(object sender, AddTaskCompletedEventArgs e)
        {
            client.AddTaskCompleted -= AddTaskCompletedEventHandler;
        }

        private void TaskClear_Click(object sender, RoutedEventArgs e)
        {
            nameTextBox.Text = "";
        }

        private void TaskAdd_Click(object sender, RoutedEventArgs e)
        {
            if (PageTitle.Text == "Edit Task")
            {
                client.RemoveTaskCompleted += RemoveTaskCompletedEventHandler;
                client.RemoveTaskAsync(app.editTaskID);
            }
            Task TheTask = new Task();
            TheTask.Id = Guid.NewGuid();
            TheTask.LocationType = locType;
            TheTask.Name = nameTextBox.Text;
            TheTask.Deadline = datePicker.Value.Value.Add(timePicker.Value.Value.TimeOfDay);
            TheTask.ReminderTime = TheTask.Deadline.Subtract(new TimeSpan(int.Parse(reminderDay.Text), int.Parse(reminderHour.Text), int.Parse(reminderMin.Text), 0));
            TheTask.UserID = app.myUserID;
            TheTask.GroupID = groupType;
            if (locType == "Custom")
            {
                TheTask.LocName = customNameText.Text;
                TheTask.LocLat = Convert.ToDouble(customLatText.Text);
                TheTask.LocLong = Convert.ToDouble(customLongText.Text);
            }

            client.AddTaskCompleted += AddTaskCompletedEventHandler;
            client.AddTaskAsync(TheTask);
            
            this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void TaskCancel_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void groceryChecked(object sender, RoutedEventArgs e)
        {
            locType = "Grocery Store";
        }

        private void bookstoreChecked(object sender, RoutedEventArgs e)
        {
            locType = "Bookstore";
        }

        private void electronicsChecked(object sender, RoutedEventArgs e)
        {
            locType = "Electronics Store";
        }

        private void barberChecked(object sender, RoutedEventArgs e)
        {
            locType = "Barbershop";
        }

        private void group_Checked(object sender, RoutedEventArgs e)
        {
            User listboxDataItem = (User)(sender as RadioButton).DataContext;
            groupType = listboxDataItem.GroupID;
            if (groupType == "Personal")
            {
                groupType = app.myUserID;
            }
        }

        private void customLoc_Checked(object sender, RoutedEventArgs e)
        {
            locType = "Custom";
            textBlock5.Visibility = Visibility.Visible;
            textBlock11.Visibility = Visibility.Visible;
            textBlock12.Visibility = Visibility.Visible;
            customNameText.Visibility = Visibility.Visible;
            customLatText.Visibility = Visibility.Visible;
            customLongText.Visibility = Visibility.Visible;

        }

        private void customLoc_UnChecked(object sender, RoutedEventArgs e)
        {
            textBlock5.Visibility = Visibility.Collapsed;
            textBlock11.Visibility = Visibility.Collapsed;
            textBlock12.Visibility = Visibility.Collapsed;
            customNameText.Visibility = Visibility.Collapsed;
            customLatText.Visibility = Visibility.Collapsed;
            customLongText.Visibility = Visibility.Collapsed;
        }

    }
}