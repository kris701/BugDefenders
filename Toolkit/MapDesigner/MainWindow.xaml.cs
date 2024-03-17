using BugDefender.Core.Game.Models.Maps;
using BugDefender.Tools;
using CommandLine;
using MapDesigner.UserControls;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MapDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MapDefinition _currentMap;
        private FileInfo _targetFile;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadMapButton_Click(object sender, RoutedEventArgs e)
        {
            var targetMap = LoadFile(".json", "Json Files (.json)|*.json");
            if (targetMap != null)
            {
                var parsed = JsonSerializer.Deserialize<MapDefinition>(File.ReadAllText(targetMap.FullName), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                if (parsed == null)
                    throw new Exception("Could not parse map file!");
                _currentMap = parsed;
                _targetFile = targetMap;
                BlockingTilesStackPanel.Children.Clear();
                foreach (var block in parsed.BlockingTiles)
                    BlockingTilesStackPanel.Children.Add(new BlockingTileControl(BlockingTilesStackPanel.Children, UpdateView, block.X, block.Y, block.Width, block.Height));
                PathsStackPanel.Children.Clear();
                foreach (var path in parsed.Paths)
                    PathsStackPanel.Children.Add(new PathsControl(PathsStackPanel.Children, UpdateView, path));

                UpdateView();
                ControlsGrid.IsEnabled = true;
            }
        }

        private void NewMapButton_Click(object sender, RoutedEventArgs e)
        {
            var targetFile = SaveFile(".json", "Json Files (.json)|*.json");
            if (targetFile != null)
            {
                _currentMap = new MapDefinition(
                    Guid.NewGuid(),
                    targetFile.Name.Replace(targetFile.Extension, ""),
                    "",
                    new List<List<FloatPoint>>(),
                    new List<BlockedTile>(),
                    950,
                    950,
                    new List<string>()
                    );
                _targetFile = targetFile;
                UpdateStatsBox();
                ControlsGrid.IsEnabled = true;
            }
        }

        private void CompileMapButton_Click(object sender, RoutedEventArgs e)
        {
            var targetFile = SaveFile(".json", "Json Files (.json)|*.json");
            if (targetFile != null)
            {
                var text = JsonSerializer.Serialize(_currentMap);
                File.WriteAllText(targetFile.FullName, text);
            }
        }

        private void AutoGenBlockingTilesButton_Click(object sender, RoutedEventArgs e)
        {
            var range = int.Parse(AutoGenBlockingTileRange.Text);
            _currentMap.BlockingTiles = MapPathBlockingTileGen.Program.AutoGenTiles(_currentMap.Paths, range);
            BlockingTilesStackPanel.Children.Clear();
            foreach (var block in _currentMap.BlockingTiles)
                BlockingTilesStackPanel.Children.Add(new BlockingTileControl(BlockingTilesStackPanel.Children, UpdateView, block.X, block.Y, block.Width, block.Height));
            UpdateView();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void NewPathButton_Click(object sender, RoutedEventArgs e)
        {
            PathsStackPanel.Children.Add(new PathsControl(PathsStackPanel.Children, UpdateView));
            UpdateView();
        }

        private void NewBlockingTileButton_Click(object sender, RoutedEventArgs e)
        {
            BlockingTilesStackPanel.Children.Add(new BlockingTileControl(BlockingTilesStackPanel.Children, UpdateView));
            UpdateView();
        }

        public void UpdateView()
        {
            MapCanvas.Children.Clear();
            _currentMap.BlockingTiles.Clear();
            foreach (var child in BlockingTilesStackPanel.Children)
            {
                if (child is BlockingTileControl block)
                {
                    var newRec = new Canvas();
                    newRec.Margin = new Thickness(block.BlockX, block.BlockY, 0, 0);
                    newRec.Width = block.BlockWidth;
                    newRec.Width = block.BlockWidth;
                    newRec.Height = block.BlockHeight;
                    newRec.Background = Brushes.Red;
                    newRec.Opacity = 0.5;

                    MapCanvas.Children.Add(newRec);
                    _currentMap.BlockingTiles.Add(new BlockedTile(block.BlockX, block.BlockY, block.BlockWidth, block.BlockHeight));
                }
            }
            _currentMap.Paths.Clear();
            foreach (var child in PathsStackPanel.Children)
            {
                if (child is PathsControl path)
                {
                    if (path.Waypoints.Count < 2)
                        break;
                    var from = path.Waypoints[0];
                    var brush = PickBrush();
                    foreach (var point in path.Waypoints.Skip(1)) 
                    {
                        var newLine = new Line();
                        newLine.X1 = from.X;
                        newLine.Y1 = from.Y;
                        newLine.X2 = point.X;
                        newLine.Y2 = point.Y;
                        newLine.StrokeThickness = 3;
                        newLine.Stroke = brush;
                        from = point;

                        MapCanvas.Children.Add(newLine);
                    }
                    _currentMap.Paths.Add(path.Waypoints);
                }
            }
            UpdateStatsBox();
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            var targetFile = LoadFile(".png", "PNG Files (.png)|*.png");
            if (targetFile != null)
            {
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(targetFile.FullName, UriKind.Absolute));
                MapCanvas.Background = brush;
            }
        }

        // https://learn.microsoft.com/en-us/dotnet/desktop/wpf/windows/how-to-open-common-system-dialog-box?view=netdesktop-8.0#open-file-dialog-box
        private FileInfo? LoadFile(string extension, string filter)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = extension;
            dialog.Filter = filter;

            bool? result = dialog.ShowDialog();

            if (result == true)
                return new FileInfo(dialog.FileName);
            return null;
        }

        // https://learn.microsoft.com/en-us/dotnet/desktop/wpf/windows/how-to-open-common-system-dialog-box?view=netdesktop-8.0#save-file-dialog-box
        private FileInfo? SaveFile(string extension, string filter)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.DefaultExt = extension;
            dialog.Filter = filter;

            bool? result = dialog.ShowDialog();

            if (result == true)
                return new FileInfo(dialog.FileName);
            return null;
        }

        private void UpdateStatsBox()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"ID: {_currentMap.ID}");
            sb.AppendLine($"Name: {_currentMap.Name}");
            sb.AppendLine($"Description: {_currentMap.Description}");
            sb.AppendLine($"Size: {_currentMap.Width}x{_currentMap.Height}");
            sb.AppendLine($"Paths: {_currentMap.Paths.Count}");
            sb.AppendLine($"Subpaths: {_currentMap.Paths.Sum(x => x.Count)}");
            sb.AppendLine($"Blocking Tiles: {_currentMap.BlockingTiles.Count}");

            StatsTextBlock.Text = sb.ToString();
        }

        private Brush PickBrush()
        {
            Brush result = Brushes.Transparent;

            Random rnd = new Random();

            Type brushesType = typeof(Brushes);

            PropertyInfo[] properties = brushesType.GetProperties();

            int random = rnd.Next(properties.Length);
            result = (Brush)properties[random].GetValue(null, null);

            return result;
        }
    }
}