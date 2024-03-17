using BugDefender.Core.Campaign.Models;
using CampaignDesigner.UserControls;
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

namespace CampaignDesigner
{
    public partial class MainWindow : Window
    {
        private CampaignDefinition _currentCampaign;
        private FileInfo _targetFile;
        private bool _isLoaded;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void NewCampaignButton_Click(object sender, RoutedEventArgs e)
        {
            var targetFile = SaveFile(".json", "Json Files (.json)|*.json");
            if (targetFile != null)
            {
                _currentCampaign = new CampaignDefinition(
                    Guid.NewGuid(),
                    targetFile.Name.Replace(targetFile.Extension, ""),
                    "",
                    new Dictionary<Guid, string>(),
                    new List<ChapterDefinition>(),
                    new CutsceneDefinition(Guid.NewGuid(), new List<ConversationDefinition>()),
                    0);
                _targetFile = targetFile;
                InitialPanel.Visibility = Visibility.Hidden;
                DesignGrid.Visibility = Visibility.Visible;
                UpdateView();
            }
        }

        private void LoadCampaignButton_Click(object sender, RoutedEventArgs e)
        {
            var targetFile = LoadFile(".json", "Json Files (.json)|*.json");
            if (targetFile != null)
            {
                var parsed = JsonSerializer.Deserialize<CampaignDefinition>(File.ReadAllText(targetFile.FullName), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                if (parsed == null)
                    throw new Exception("Could not parse campaign file!");
                _currentCampaign = parsed;
                _targetFile = targetFile;

                InitialPanel.Visibility = Visibility.Hidden;
                DesignGrid.Visibility = Visibility.Visible;
                UpdateView();
            }
        }

        public void UpdateView()
        {
            CampaignNameTextbox.Text = _currentCampaign.Name;
            CampaignDescriptionTextbox.Text = _currentCampaign.Description;
            CampaignIDTextBlock.Text = _currentCampaign.ID.ToString();
            CampaignRewardTextbox.Text = $"{_currentCampaign.Reward}";
            ChaptersItem.Items.Clear();
            foreach(var chapter in _currentCampaign.Chapters)
            {
                var newChapterItem = new TreeViewItem()
                {
                    Header = chapter.Name,
                    ContextMenu = new ContextMenu()
                };
                var deleteItem = new MenuItem()
                {
                    Header = "Delete Chapter",
                    Tag = chapter
                };
                deleteItem.Click += (s, e) =>
                {
                    if (s is MenuItem element && element.Tag is ChapterDefinition chapter)
                    {
                        _currentCampaign.Chapters.Remove(chapter);
                        UpdateView();
                    }
                };
                newChapterItem.ContextMenu.Items.Add(deleteItem);
                var cutsceneButton = new TreeViewItem()
                {
                    Header = "Intro Cutscene",
                    Tag = chapter.Intro
                };
                cutsceneButton.MouseDoubleClick += (s, e) =>
                {
                    if (s is TreeViewItem element && element.Tag is CutsceneDefinition cutscene)
                    {
                        EditViewCanvas.Children.Clear();
                        EditViewCanvas.Children.Add(new CutsceneControl(cutscene, _currentCampaign.Speakers));
                    }
                };
                newChapterItem.Items.Add(cutsceneButton);
                var generalButton = new TreeViewItem()
                {
                    Header = "General",
                    Tag = chapter
                };
                generalButton.MouseDoubleClick += (s, e) =>
                {
                    if (s is TreeViewItem element && element.Tag is ChapterDefinition chapter)
                    {
                        EditViewCanvas.Children.Clear();
                        EditViewCanvas.Children.Add(new ChapterControl(_currentCampaign, chapter));
                    }
                };
                newChapterItem.Items.Add(generalButton);
                ChaptersItem.Items.Add(newChapterItem);
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

        private void SaveCampaign()
        {
            if (_targetFile != null && _currentCampaign != null)
                File.WriteAllText(_targetFile.FullName, JsonSerializer.Serialize(_currentCampaign));
        }

        private void CampaignNameTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded)
                return;
            _currentCampaign.Name = CampaignNameTextbox.Text;
            UpdateView();
        }

        private void CampaignDescriptionTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded)
                return;
            _currentCampaign.Description = CampaignDescriptionTextbox.Text;
            UpdateView();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCampaign();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CampaignRewardTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded)
                return;
            _currentCampaign.Reward = Int32.Parse(CampaignRewardTextbox.Text);
            UpdateView();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
        }

        private void AddNewChapter_Click(object sender, RoutedEventArgs e)
        {
            _currentCampaign.Chapters.Add(new ChapterDefinition(
                Guid.NewGuid(),
                null,
                "New Chapter",
                Guid.Empty,
                new CutsceneDefinition(Guid.NewGuid(), new List<ConversationDefinition>()),
                Guid.Empty,
                new List<BugDefender.Core.Users.Models.UserCriterias.IUserCriteria>()));
            UpdateView();
        }

        private void CampaignOverItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditViewCanvas.Children.Clear();
            EditViewCanvas.Children.Add(new CutsceneControl(_currentCampaign.CampaignOver, _currentCampaign.Speakers));
        }

        private void SpeakersItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditViewCanvas.Children.Clear();
            EditViewCanvas.Children.Add(new SpeakersControl(_currentCampaign));
        }
    }
}