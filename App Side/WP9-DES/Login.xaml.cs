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
    public partial class Login : PhoneApplicationPage
    {
        App app = Application.Current as App;
        SchedulerServiceClient client;
        EventHandler<AddUserCompletedEventArgs> AddUserCompletedEventHandler;
        EventHandler<GetMyGroupsCompletedEventArgs> GetMyGroupsCompletedEventHandler;
        EventHandler<RemoveUserCompletedEventArgs> RemoveUserCompletedEventHandler;
        ObservableCollection<User> listUsers = new ObservableCollection<User>();
        bool foundID = false;
        bool validID = false;
        int validIDindex = 0;
        DispatcherTimer tmr = new DispatcherTimer();
        DispatcherTimer tmr2 = new DispatcherTimer();

        public Login()
        {
            InitializeComponent();
            AddUserCompletedEventHandler = new EventHandler<AddUserCompletedEventArgs>(client_AddUserCompleted);
            GetMyGroupsCompletedEventHandler = new EventHandler<GetMyGroupsCompletedEventArgs>(client_GetMyGroupsCompleted);
            RemoveUserCompletedEventHandler = new EventHandler<RemoveUserCompletedEventArgs>(client_RemoveUserCompleted);

            client = new SchedulerServiceClient();
            
            tmr.Interval = TimeSpan.FromMilliseconds(2000);
            tmr.Tick += CheckAndCreateUser;
            tmr2.Interval = TimeSpan.FromMilliseconds(3000);
            tmr2.Tick += LogInUser;
        }

        void client_RemoveUserCompleted(object sender, RemoveUserCompletedEventArgs e)
        {
            client.RemoveUserCompleted -= RemoveUserCompletedEventHandler;
        }

        void client_GetMyGroupsCompleted(object sender, GetMyGroupsCompletedEventArgs e)
        {
            listUsers = e.Result;
            client.GetMyGroupsCompleted -= GetMyGroupsCompletedEventHandler;
            for (int i = 0; i < listUsers.Count; i++)
            {
                if (listUsers[i].UserID == usernameTextBox.Text)
                {
                    foundID = true;
                    validID = true;
                    validIDindex = i;
                }
            }
        }

        void client_AddUserCompleted(object sender, AddUserCompletedEventArgs e)
        {
            client.AddUserCompleted -= AddUserCompletedEventHandler;
            if (e.Result)
            {
                messageText.Text = "Username <" + usernameTextBox.Text + "> created, please log in.";
                logInButton.Visibility = Visibility.Visible;
                textBlock3.Visibility = Visibility.Collapsed;
                passwordTextBox2.Visibility = Visibility.Collapsed;
                newUsernameButton.Visibility = Visibility.Visible;
                createButton.Visibility = Visibility.Collapsed;
                backButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                messageText.Text = "Unable to create new username, please try again.";
            }
        }

        private void NewUsername_Click(object sender, RoutedEventArgs e)
        {
            logInButton.Visibility = Visibility.Collapsed;
            textBlock3.Visibility = Visibility.Visible;
            passwordTextBox2.Visibility = Visibility.Visible;
            newUsernameButton.Visibility = Visibility.Collapsed;
            createButton.Visibility = Visibility.Visible;
            backButton.Visibility = Visibility.Visible;
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            messageText.Text = "Creating User Profile...";
            if (passwordTextBox.Text != passwordTextBox2.Text)
            {
                messageText.Text = "Error: Passwords do not match";
            }
            else if (usernameTextBox.Text == "")
            {
                messageText.Text = "Please Enter a Username";
            }
            else
            {
                foundID = false;
                client.GetMyGroupsCompleted += GetMyGroupsCompletedEventHandler;
                client.GetMyGroupsAsync(usernameTextBox.Text);
                
                tmr.Start();
            }
            
        }

        void CheckAndCreateUser(object sender, EventArgs args)
        {
            tmr.Stop();
            if (foundID)
            {
                messageText.Text = "Error: Username is already taken";
            }
            else
            {
                User TheUser = new User();
                TheUser.Id = Guid.NewGuid();
                TheUser.UserID = usernameTextBox.Text;
                TheUser.GroupID = usernameTextBox.Text;
                TheUser.Password = passwordTextBox.Text;
                TheUser.GroupPass = passwordTextBox.Text;

                client.AddUserCompleted += AddUserCompletedEventHandler;
                client.AddUserAsync(TheUser);
            }
        }

        private void logInButton_Click(object sender, RoutedEventArgs e)
        {
            messageText.Text = "Verifying User information ...";
            if (usernameTextBox.Text == "")
            {
                messageText.Text = "Please Enter a Username";
            }
            else
            {
                validID = false;
                client.GetMyGroupsCompleted += GetMyGroupsCompletedEventHandler;
                client.GetMyGroupsAsync(usernameTextBox.Text);

                tmr2.Start();
            }
        }

        void LogInUser(object sender, EventArgs args)
        {
            tmr2.Stop();
            if (!validID)
            {
                messageText.Text = "Error: Username doesn't exist. Please create one or try again.";
            }
            else if (passwordTextBox.Text != listUsers[validIDindex].Password)
            {
                messageText.Text = "Access Denied. Invalid Password. Please try again.";
            }
            else
            {
                app.myUserID = usernameTextBox.Text;
                app.myPass = passwordTextBox.Text;
                messageText.Text = "Welcome! Logging in...";
                this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            logInButton.Visibility = Visibility.Visible;
            textBlock3.Visibility = Visibility.Collapsed;
            passwordTextBox2.Visibility = Visibility.Collapsed;
            newUsernameButton.Visibility = Visibility.Visible;
            createButton.Visibility = Visibility.Collapsed;
            backButton.Visibility = Visibility.Collapsed;
        }
        
    }
}