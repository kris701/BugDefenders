using BugDefender.Tools;
using System.Windows;
using System.Windows.Controls;

namespace MapDesigner.UserControls
{
    public partial class PathsControl : UserControl
    {
        public List<FloatPoint> Waypoints
        {
            get
            {
                var retList = new List<FloatPoint>();
                foreach (var child in SubPathsStackPanel.Children)
                    if (child is PathControl path)
                        retList.Add(new FloatPoint(path.WaypointX, path.WaypointY));
                return retList;
            }
        }

        private readonly UIElementCollection _from;
        private readonly Action _update;
        public PathsControl(UIElementCollection from, Action update)
        {
            _from = from;
            _update = update;
            InitializeComponent();
        }

        public PathsControl(UIElementCollection from, Action update, List<FloatPoint> waypoints) : this(from, update)
        {
            foreach (var point in waypoints)
                SubPathsStackPanel.Children.Add(new PathControl(SubPathsStackPanel.Children, _update, point.X, point.Y));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            _from.Remove(this);
        }

        private void AddSubPathButton_Click(object sender, RoutedEventArgs e)
        {
            SubPathsStackPanel.Children.Add(new PathControl(SubPathsStackPanel.Children, _update));
        }
    }
}
