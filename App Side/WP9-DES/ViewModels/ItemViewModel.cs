using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WP9_DES
{
    public class ItemViewModel : INotifyPropertyChanged
    {
        private string _lineOne;
        public string LineOne
        {
            get
            {
                return _lineOne;
            }
            set
            {
                if (value != _lineOne)
                {
                    _lineOne = value;
                    NotifyPropertyChanged("LineOne");
                }
            }
        }

        private string _lineTwo;
        public string LineTwo
        {
            get
            {
                return _lineTwo;
            }
            set
            {
                if (value != _lineTwo)
                {
                    _lineTwo = value;
                    NotifyPropertyChanged("LineTwo");
                }
            }
        }

        private string _lineThree;
        public string LineThree
        {
            get
            {
                return _lineThree;
            }
            set
            {
                if (value != _lineThree)
                {
                    _lineThree = value;
                    NotifyPropertyChanged("LineThree");
                }
            }
        }

        private string _imageSource;
        public string ImageSource
        {
            get
            {
                return _imageSource;
            }
            set
            {
                if (value != _imageSource)
                {
                    _imageSource = value;
                    NotifyPropertyChanged("ImageSource");
                }
            }
        }

        private int _numIndex;
        public int NumIndex
        {
            get
            {
                return _numIndex;
            }
            set
            {
                if (value != _numIndex)
                {
                    _numIndex = value;
                    NotifyPropertyChanged("NumIndex");
                }
            }
        }

        private Guid _taskID;
        public Guid TaskID
        {
            get
            {
                return _taskID;
            }
            set
            {
                if (value != _taskID)
                {
                    _taskID = value;
                    NotifyPropertyChanged("TaskID");
                }
            }
        }

        private string _userID;
        public string UserID
        {
            get
            {
                return _userID;
            }
            set
            {
                if (value != _userID)
                {
                    _userID = value;
                    NotifyPropertyChanged("UserID");
                }
            }
        }

        private string _groupID;
        public string GroupID
        {
            get
            {
                return _groupID;
            }
            set
            {
                if (value != _groupID)
                {
                    _groupID = value;
                    NotifyPropertyChanged("GroupID");
                }
            }
        }

        private string _locLat;
        public string LocLat
        {
            get
            {
                return _locLat;
            }
            set
            {
                if (value != _locLat)
                {
                    _locLat = value;
                    NotifyPropertyChanged("LocLat");
                }
            }
        }

        private string _locLong;
        public string LocLong
        {
            get
            {
                return _locLong;
            }
            set
            {
                if (value != _locLong)
                {
                    _locLong = value;
                    NotifyPropertyChanged("LocLong");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}