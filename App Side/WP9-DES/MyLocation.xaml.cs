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
using System.Device.Location;

namespace WP9_DES
{
    public partial class MyLocation : PhoneApplicationPage
    {
        App app = Application.Current as App;
        public MyLocation()
        {
            InitializeComponent();

            onButton.IsChecked = app.useGPS;

            OnCampus.IsChecked = app.atCampus;
            StPaul.IsChecked = app.atStPaul;
            MOA.IsChecked = app.atMOA;
            CustomRadio.IsChecked = app.atCustom;

            customLatText.Text = app.myCurrentLoc.Latitude.ToString();
            customLongText.Text = app.myCurrentLoc.Longitude.ToString();

            InputScope Keyboard4 = new InputScope();
            InputScope Keyboard5 = new InputScope();
            InputScopeName ScopeName4 = new InputScopeName();
            InputScopeName ScopeName5 = new InputScopeName();
            ScopeName4.NameValue = InputScopeNameValue.Number;
            ScopeName5.NameValue = InputScopeNameValue.Number;
            Keyboard4.Names.Add(ScopeName4);
            Keyboard5.Names.Add(ScopeName5);
            customLatText.InputScope = Keyboard4;
            customLongText.InputScope = Keyboard5;
        }

        private void onButton_Checked(object sender, RoutedEventArgs e)
        {
            app.useGPS = true;
            OnCampus.IsChecked = false;
            StPaul.IsChecked = false;
            MOA.IsChecked = false;
            CustomRadio.IsChecked = false;
            app.atCampus = false;
            app.atStPaul = false;
            app.atMOA = false;
            app.atCustom = false;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (app.atCustom)
            {
                app.myCurrentLoc = new GeoCoordinate(Convert.ToDouble(customLatText.Text), Convert.ToDouble(customLongText.Text));
            }

            this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }


        private void OnCampus_Checked(object sender, RoutedEventArgs e)
        {
            app.useGPS = false;
            onButton.IsChecked = app.useGPS;

            app.atCampus = true;
            app.atStPaul = false;
            app.atMOA = false;
            app.atCustom = false;

            app.myCurrentLoc = new GeoCoordinate(44.9744, -93.2324);
        }

        private void StPaul_Checked(object sender, RoutedEventArgs e)
        {
            app.useGPS = false;
            onButton.IsChecked = app.useGPS;

            app.atCampus = false;
            app.atStPaul = true;
            app.atMOA = false;
            app.atCustom = false;

            app.myCurrentLoc = new GeoCoordinate(44.9558, -93.167);
        }

        private void MOA_Checked(object sender, RoutedEventArgs e)
        {
            app.useGPS = false;
            onButton.IsChecked = app.useGPS;

            app.atCampus = false;
            app.atStPaul = false;
            app.atMOA = true;
            app.atCustom = false;

            app.myCurrentLoc = new GeoCoordinate(44.8583, -93.2471);
        }

        private void Custom_Checked(object sender, RoutedEventArgs e)
        {
            textBlock11.Visibility = Visibility.Visible;
            textBlock12.Visibility = Visibility.Visible;
            customLatText.Visibility = Visibility.Visible;
            customLongText.Visibility = Visibility.Visible;

            app.useGPS = false;
            onButton.IsChecked = app.useGPS;

            app.atCampus = false;
            app.atStPaul = false;
            app.atMOA = false;
            app.atCustom = true;
        }

        private void Custom_UnChecked(object sender, RoutedEventArgs e)
        {
            textBlock11.Visibility = Visibility.Collapsed;
            textBlock12.Visibility = Visibility.Collapsed;
            customLatText.Visibility = Visibility.Collapsed;
            customLongText.Visibility = Visibility.Collapsed;
        }
    }
}