using BugDefender.Core.Game.Models.GameStyles;
using BugDefender.Core.Resources;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GameStyleDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameStyleDefinition _currentGameStyle;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadGameStyle_Click(object sender, RoutedEventArgs e)
        {
            var targetFile = LoadFile(".json", "Json Files (.json)|*.json");
            if (targetFile != null)
            {
                var parsed = JsonSerializer.Deserialize<GameStyleDefinition>(File.ReadAllText(targetFile.FullName));
                if (parsed == null)
                    throw new Exception("Error parsing file!");
                _currentGameStyle = parsed;
                UpdateView();
            }
        }

        private void SaveGameStyle_Click(object sender, RoutedEventArgs e)
        {
            var targetFile = SaveFile(".json", "Json Files (.json)|*.json");
            if (targetFile != null && _currentGameStyle != null)
            {
                try
                {
                    _currentGameStyle.ID = Guid.Parse(GameStyleIDTextbox.Text);
                    _currentGameStyle.Name = GameStyleNameTextbox.Text;
                    _currentGameStyle.Description = GameStyleDescriptionTextbox.Text;
                    _currentGameStyle.EvolutionRate = float.Parse(GameStyleEvolutionRateTextbox.Text);
                    _currentGameStyle.EnemySpeedMultiplier = float.Parse(GameStyleSpeedMultiplierTextbox.Text);
                    _currentGameStyle.RewardMultiplier = float.Parse(GameStyleRewardMultiplierTextbox.Text);
                    _currentGameStyle.StartingHP = int.Parse(GameStyleStartingHPTextbox.Text);
                    _currentGameStyle.StartingMoney = int.Parse(GameStyleStartingMoneyTextbox.Text);
                    _currentGameStyle.EnemyWaveMultiplier = float.Parse(GameStyleEnemyWaveMultiplierTextbox.Text);
                    _currentGameStyle.ProjectileSpeedCap = int.Parse(GameStyleProjectileSpeedCapTextbox.Text);
                    _currentGameStyle.BossEveryNWave = int.Parse(GameStyleBossEveryNWaveTextbox.Text);
                    _currentGameStyle.MoneyPrWave = int.Parse(GameStyleMoneyPrWaveTextbox.Text);
                    _currentGameStyle.TurretRefundPenalty = float.Parse(GameStyleTurretRefundPenaltyTextbox.Text);

                    _currentGameStyle.TurretBlackList.Clear();
                    foreach (var item in TurretBlackListCombobox.Items)
                        if (item is CheckBox combo && combo.IsChecked == true && combo.Tag is Guid guid)
                            _currentGameStyle.TurretBlackList.Add(guid);

                    _currentGameStyle.TurretWhiteList.Clear();
                    foreach (var item in TurretWhiteListCombobox.Items)
                        if (item is CheckBox combo && combo.IsChecked == true && combo.Tag is Guid guid)
                            _currentGameStyle.TurretWhiteList.Add(guid);

                    if (_currentGameStyle.TurretBlackList.Count > 0 &&
                        _currentGameStyle.TurretWhiteList.Count > 0)
                        throw new Exception("Gamestyle cannot have both a turret black and white list!");

                    _currentGameStyle.EnemyBlackList.Clear();
                    foreach (var item in EnemyBlackListCombobox.Items)
                        if (item is CheckBox combo && combo.IsChecked == true && combo.Tag is Guid guid)
                            _currentGameStyle.EnemyBlackList.Add(guid);

                    _currentGameStyle.EnemyWhiteList.Clear();
                    foreach (var item in EnemyWhiteListCombobox.Items)
                        if (item is CheckBox combo && combo.IsChecked == true && combo.Tag is Guid guid)
                            _currentGameStyle.EnemyWhiteList.Add(guid);

                    if (_currentGameStyle.EnemyBlackList.Count > 0 &&
                        _currentGameStyle.EnemyWhiteList.Count > 0)
                        throw new Exception("Gamestyle cannot have both a enemy black and white list!");

                    File.WriteAllText(targetFile.FullName, JsonSerializer.Serialize(_currentGameStyle));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Gamestyle is invalid: {ex.Message}");
                }
            }
        }

        private void NewGameStyle_Click(object sender, RoutedEventArgs e)
        {
            _currentGameStyle = new GameStyleDefinition(
                Guid.NewGuid(),
                "",
                "",
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                new List<Guid>(),
                new List<Guid>(),
                new List<Guid>(),
                new List<Guid>(),
                1,
                1,
                false);
            UpdateView();
        }

        private void UpdateView()
        {
            GameStyleIDTextbox.Text = _currentGameStyle.ID.ToString();
            GameStyleNameTextbox.Text = _currentGameStyle.Name;
            GameStyleDescriptionTextbox.Text = _currentGameStyle.Description;
            GameStyleEvolutionRateTextbox.Text = $"{_currentGameStyle.EvolutionRate}";
            GameStyleSpeedMultiplierTextbox.Text = $"{_currentGameStyle.EnemySpeedMultiplier}";
            GameStyleRewardMultiplierTextbox.Text = $"{_currentGameStyle.RewardMultiplier}";
            GameStyleStartingHPTextbox.Text = $"{_currentGameStyle.StartingHP}";
            GameStyleStartingMoneyTextbox.Text = $"{_currentGameStyle.StartingMoney}";
            GameStyleEnemyWaveMultiplierTextbox.Text = $"{_currentGameStyle.EnemyWaveMultiplier}";
            GameStyleProjectileSpeedCapTextbox.Text = $"{_currentGameStyle.ProjectileSpeedCap}";
            GameStyleBossEveryNWaveTextbox.Text = $"{_currentGameStyle.BossEveryNWave}";
            GameStyleMoneyPrWaveTextbox.Text = $"{_currentGameStyle.MoneyPrWave}";
            GameStyleTurretRefundPenaltyTextbox.Text = $"{_currentGameStyle.TurretRefundPenalty}";
            if (_currentGameStyle.CampaignOnly)
                GameStyleIsCampaignOnlyCheckbox.IsChecked = true;
            else
                GameStyleIsCampaignOnlyCheckbox.IsChecked = false;

            TurretBlackListCombobox.Items.Clear();
            foreach (var id in ResourceManager.Turrets.GetResources())
            {
                var turret = ResourceManager.Turrets.GetResource(id);
                TurretBlackListCombobox.Items.Add(new CheckBox()
                {
                    Content = $"{turret.Name} ({id})",
                    IsChecked = _currentGameStyle.TurretBlackList.Contains(id),
                    Tag = id
                });
            }

            TurretWhiteListCombobox.Items.Clear();
            foreach (var id in ResourceManager.Turrets.GetResources())
            {
                var turret = ResourceManager.Turrets.GetResource(id);
                TurretWhiteListCombobox.Items.Add(new CheckBox()
                {
                    Content = $"{turret.Name} ({id})",
                    IsChecked = _currentGameStyle.TurretWhiteList.Contains(id),
                    Tag = id
                });
            }

            EnemyBlackListCombobox.Items.Clear();
            foreach (var id in ResourceManager.Enemies.GetResources())
            {
                var enemy = ResourceManager.Enemies.GetResource(id);
                EnemyBlackListCombobox.Items.Add(new CheckBox()
                {
                    Content = $"{enemy.Name} ({id})",
                    IsChecked = _currentGameStyle.EnemyBlackList.Contains(id),
                    Tag = id
                });
            }

            EnemyWhiteListCombobox.Items.Clear();
            foreach (var id in ResourceManager.Enemies.GetResources())
            {
                var enemy = ResourceManager.Enemies.GetResource(id);
                EnemyWhiteListCombobox.Items.Add(new CheckBox()
                {
                    Content = $"{enemy.Name} ({id})",
                    IsChecked = _currentGameStyle.EnemyWhiteList.Contains(id),
                    Tag = id
                });
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

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}