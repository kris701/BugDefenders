using BugDefender.Core.Campaign.Models;
using BugDefender.Core.Resources;
using BugDefender.Core.Users.Models.UserCriterias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CampaignDesigner.UserControls
{
    public partial class ChapterControl : UserControl
    {
        public ChapterDefinition Chapter { get; }
        private bool _isLoaded;

        public ChapterControl(CampaignDefinition campaign, ChapterDefinition chapter)
        {
            Chapter = chapter;
            InitializeComponent();

            ChapterNameTextbox.Text = chapter.Name;
            NextChapterCombobox.Items.Add(new ComboBoxItem()
            {
                Content = "None (final chapter)",
                Tag = Guid.Empty
            });
            NextChapterCombobox.SelectedIndex = 0;
            int count = 1;
            foreach(var other in campaign.Chapters)
            {
                if (other.ID != chapter.ID)
                {
                    NextChapterCombobox.Items.Add(new ComboBoxItem()
                    {
                        Content = $"{other.Name} ({other.ID})",
                        Tag = other.ID
                    });
                    if (other.ID == chapter.NextChapterID)
                        NextChapterCombobox.SelectedIndex = count;
                    count++;
                }
            }

            count = 0;
            foreach(var id in ResourceManager.Maps.GetResources())
            {
                var map = ResourceManager.Maps.GetResource(id);
                MapIDCombobox.Items.Add(new ComboBoxItem()
                {
                    Content = $"{map.Name} ({id})",
                    Tag = id
                });
                if (id == chapter.MapID)
                    MapIDCombobox.SelectedIndex = count;
                count++;
            }

            count = 0;
            foreach (var id in ResourceManager.GameStyles.GetResources())
            {
                var map = ResourceManager.GameStyles.GetResource(id);
                GamestyleIDCombobox.Items.Add(new ComboBoxItem()
                {
                    Content = $"{map.Name} ({id})",
                    Tag = id
                });
                if (id == chapter.GameStyleID)
                    GamestyleIDCombobox.SelectedIndex = count;
                count++;
            }

            AddNewDefault.Items.Add(new ComboBoxItem()
            {
                Content = "Enemies Killed",
                Tag = () => new EnemiesKilledCriteria()
            });
            AddNewDefault.Items.Add(new ComboBoxItem()
            {
                Content = "Enemies Killed of Type",
                Tag = () => new EnemiesKilledOfTypeCriteria()
            });
            AddNewDefault.Items.Add(new ComboBoxItem()
            {
                Content = "Money Earned",
                Tag = () => new MoneyEarnedCriteria()
            });
            AddNewDefault.Items.Add(new ComboBoxItem()
            {
                Content = "Turret Kills",
                Tag = () => new TurretKillsCriteria()
            });
            AddNewDefault.Items.Add(new ComboBoxItem()
            {
                Content = "Turret Kills of Type",
                Tag = () => new TurretKillsOfTypeCriteria()
            });
            AddNewDefault.Items.Add(new ComboBoxItem()
            {
                Content = "Turrets Placed",
                Tag = () => new TurretsPlacedCriteria()
            });
            AddNewDefault.Items.Add(new ComboBoxItem()
            {
                Content = "Turrets Placed of Type",
                Tag = () => new TurretsPlacedOfTypeCriteria()
            });
            AddNewDefault.Items.Add(new ComboBoxItem()
            {
                Content = "Turrets Sold",
                Tag = () => new TurretsSoldCriteria()
            });
            AddNewDefault.Items.Add(new ComboBoxItem()
            {
                Content = "Turrets Sold of Type",
                Tag = () => new TurretsSoldOfTypeCriteria()
            });
            AddNewDefault.Items.Add(new ComboBoxItem()
            {
                Content = "Waves Started",
                Tag = () => new WavesStartedCriteria()
            });

            CriteriaText.Text = JsonSerializer.Serialize(chapter.Criterias, new JsonSerializerOptions() { WriteIndented = true });
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
        }

        private void ChapterNameTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded)
                return;
            Chapter.Name = ChapterNameTextbox.Text;
        }

        private void NextChapterCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded)
                return;
            if (sender is ComboBox combo && combo.SelectedItem is ComboBoxItem item && item.Tag is Guid guid)
            {
                if (guid == Guid.Empty)
                    Chapter.NextChapterID = null;
                else
                    Chapter.NextChapterID = guid;
            }
        }

        private void MapIDCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded)
                return;
            if (sender is ComboBox combo && combo.SelectedItem is ComboBoxItem item && item.Tag is Guid guid)
                Chapter.MapID = guid;
        }

        private void GamestyleIDCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLoaded)
                return;
            if (sender is ComboBox combo && combo.SelectedItem is ComboBoxItem item && item.Tag is Guid guid)
                Chapter.GameStyleID = guid;
        }

        private void ParseCriteriaButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parsed = JsonSerializer.Deserialize<List<IUserCriteria>>(CriteriaText.Text, new JsonSerializerOptions() { WriteIndented = true });
                if (parsed != null)
                {
                    Chapter.Criterias = parsed;
                    CriteriaText.Text = JsonSerializer.Serialize(Chapter.Criterias, new JsonSerializerOptions() { WriteIndented = true });
                }
            }
            catch { MessageBox.Show("There are syntax errors in the criteria content!"); }
        }

        private void AddNewDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddNewDefault.SelectedItem is ComboBoxItem item && item.Tag is Func<IUserCriteria> func)
            {
                Chapter.Criterias.Add(func());
                CriteriaText.Text = JsonSerializer.Serialize(Chapter.Criterias, new JsonSerializerOptions() { WriteIndented = true });
            }
        }
    }
}
