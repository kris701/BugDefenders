using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TDGame.OpenGL.Engine.Controls;
using TDGame.OpenGL.Engine.Helpers;
using TDGame.OpenGL.Engine.Screens;

namespace TDGame.OpenGL.Screens.UsersScreen
{
    public partial class UsersScreen : BaseScreen
    {
        private List<ButtonControl> _usersButtons = new List<ButtonControl>();
        private List<ButtonControl> _usersDeleteButtons = new List<ButtonControl>();
        private TextInputControl _nameInputBox;

        public override void Initialize()
        {
            AddControl(0, new TileControl(this)
            {
                FillColor = TextureManager.GetTexture(new Guid("46ce91a9-78bc-4d77-95c1-46adfce971b2")),
                Width = 1000,
                Height = 1000
            });

            AddControl(0, new LabelControl(this)
            {
                HorizontalAlignment = Engine.Alignment.Middle,
                Y = 100,
                Width = 350,
                Text = "Users",
                Font = BasicFonts.GetFont(48),
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
            });

            _nameInputBox = new TextInputControl(this)
            {
                HorizontalAlignment = Engine.Alignment.Middle,
                Y = 200,
                Width = 250,
                Height = 50,
                Limit = 10,
                Font = BasicFonts.GetFont(24),
                FillColor = BasicTextures.GetBasicRectange(Color.LightGray),
            };
            AddControl(0, _nameInputBox);
            AddControl(0, new ButtonControl(this, AddUserButton_Click)
            {
                HorizontalAlignment = Engine.Alignment.Middle,
                Y = 250,
                Width = 250,
                Height = 50,
                Text = "Add User",
                Font = BasicFonts.GetFont(24),
                FillColor = BasicTextures.GetBasicRectange(Color.LightGray),
                FillClickedColor = BasicTextures.GetClickedTexture(),
                FillDisabledColor = BasicTextures.GetDisabledTexture()
            });

            UpdateUsersList();

            AddControl(0, new ButtonControl(this, clicked: (x) =>
            {
                SwitchView(new MainMenu.MainMenu(Parent));
            })
            {
                Y = 950,
                X = 800,
                Width = 200,
                Height = 50,
                Text = "Back",
                Font = BasicFonts.GetFont(24),
                FillColor = BasicTextures.GetBasicRectange(Color.Gray),
                FillClickedColor = BasicTextures.GetClickedTexture(),
            });

#if DEBUG
            AddControl(0, new ButtonControl(this, clicked: (x) => SwitchView(new UsersScreen(Parent)))
            {
                X = 0,
                Y = 0,
                Width = 50,
                Height = 25,
                Text = "Reload",
                Font = BasicFonts.GetFont(10),
                FillColor = BasicTextures.GetBasicRectange(Color.White),
                FillClickedColor = BasicTextures.GetBasicRectange(Color.Gray)
            });
#endif
            base.Initialize();
        }

        private void UpdateUsersList()
        {
            foreach (var item in _usersButtons)
                RemoveControl(1, item);
            foreach (var item in _usersDeleteButtons)
                RemoveControl(2, item);
            var allUsers = Parent.UserManager.GetAllUsers();
            var count = 0;
            foreach (var user in allUsers)
            {
                var newControl = new ButtonControl(this, ChangeUserButton_Click)
                {
                    X = 250,
                    Y = 310 + (count * 35 + 5),
                    Width = 500,
                    Height = 30,
                    Text = $"{user.Name}",
                    Font = BasicFonts.GetFont(16),
                    FillColor = BasicTextures.GetBasicRectange(Color.LightGray),
                    FillClickedColor = BasicTextures.GetClickedTexture(),
                    FillDisabledColor = BasicTextures.GetDisabledTexture(),
                    Tag = user
                };
                if (user.IsPrimary)
                    newControl.FillColor = BasicTextures.GetBasicRectange(Color.DarkGreen);
                _usersButtons.Add(newControl);
                AddControl(1, newControl);

                var newDeleteControl = new ButtonControl(this, RemoveUserButton_Click)
                {
                    X = 750,
                    Y = 310 + (count++ * 35 + 5),
                    Width = 30,
                    Height = 30,
                    Text = "X",
                    Font = BasicFonts.GetFont(16),
                    FillColor = BasicTextures.GetBasicRectange(Color.Red),
                    FillClickedColor = BasicTextures.GetClickedTexture(),
                    FillDisabledColor = BasicTextures.GetDisabledTexture(),
                    Tag = user
                };
                if (user.IsPrimary)
                    newControl.FillColor = BasicTextures.GetBasicRectange(Color.DarkGreen);
                _usersDeleteButtons.Add(newDeleteControl);
                AddControl(2, newDeleteControl);


            }
        }
    }
}
