using Project1.Screens.PathTest;
using TDGame.Core.Maps;
using TDGame.OpenGL.Engine;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Screens;
using TDGame.OpenGL.Textures;

namespace Project1.Screens.GameSetupView
{
    public partial class GameSetupView : BaseScreen
    {
        private string _selectedGameStyle = "";
        private string _selectedMap = "";

        public GameSetupView(IEngine parent) : base(parent)
        {
            Initialize();
        }

        private void StartButton_Click(ButtonControl sender)
        {
            if (_selectedMap != "" && _selectedGameStyle != "")
                Parent.SwitchView(new PathTestScreen(Parent, _selectedMap, _selectedGameStyle));
        }

        private void SelectMap_Click(ButtonControl sender)
        {
            if (sender.Tag is string mapName)
            {
                _selectedMap = mapName;
                var map = MapBuilder.GetMap(mapName);
                _mapPreviewPanel.FillColor = TextureBuilder.GetTexture(map.ID);
            }
        }

        private void SelectGameStyle_Click(ButtonControl sender)
        {
            if (sender.Tag is string gameStyleName)
            {
                _selectedGameStyle = gameStyleName;
            }
        }
    }
}
