using System;
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
    public class Notification
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Deadline { get; set; }
        public string LocType { get; set; }
        public string Reason { get; set; }
        public string Reason2 { get; set; }
    }
}
