using BugDefender.Core.Game.Models.Maps;
using BugDefender.Tools;
using System.IO;
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
                var parsed = JsonSerializer.Deserialize<MapDefinition>(File.ReadAllText(targetMap.FullName));
                if (parsed == null)
                    throw new Exception("Could not parse map file!");
                _currentMap = parsed;
                _targetFile = targetMap;
                UpdateStatsBox();
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
            }
        }

        private void CompileMapButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AutoGenBlockingTilesButton_Click(object sender, RoutedEventArgs e)
        {
            var range = int.Parse(AutoGenBlockingTileRange.Text);
            _currentMap.BlockingTiles = MapPathBlockingTileGen.Program.AutoGenTiles(_currentMap.Paths, range);
            UpdateStatsBox();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void NewPathButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NewBlockingTileButton_Click(object sender, RoutedEventArgs e)
        {

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
    }
}