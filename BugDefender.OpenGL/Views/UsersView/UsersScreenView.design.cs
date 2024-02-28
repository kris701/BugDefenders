using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using BugDefender.OpenGL.Engine.Controls;
using BugDefender.OpenGL.Engine.Helpers;
using BugDefender.OpenGL.Engine.Views;

namespace BugDefender.OpenGL.Screens.UsersScreen
{
    public partial class UsersScreenView : BaseView
    {
        private readonly List<ButtonControl> _usersButtons = new List<ButtonControl>();
        private readonly List<ButtonControl> _usersDeleteButtons = new List<ButtonControl>();
        private TextInputControl _nameInputBox;
        private ButtonControl _acceptButton;

        public override void Initialize()
        {
            AddControl(0, new TileControl(Parent)
            {
                FillColor = Parent.UIResources.GetTexture(new Guid("46ce91a9-78bc-4d77-95c1-46adfce971b2")),
                Width = 1000,
                Height = 1000
            });

            AddControl(0, new LabelControl(Parent)
            {
                HorizontalAlignment = Engine.Alignment.Middle,
                Y = 100,
                Height = 75,
                Width = 800,
                Text = "Users",
                FontColor = Color.White,
                Font = BasicFonts.GetFont(48)
            });

            _nameInputBox = new TextInputControl(Parent, AddUserInputTextBox_Enter)
            {
                X = 200,
                Y = 200,
                Width = 400,
                Height = 50,
                Limit = 10,
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = Parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                FillDisabledColor = Parent.UIResources.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8")),
            };
            AddControl(0, _nameInputBox);
            _acceptButton = new ButtonControl(Parent, AddUserButton_Click)
            {
                X = 600,
                Y = 200,
                Width = 200,
                Height = 50,
                Text = "Add User",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                FillDisabledColor = Parent.UIResources.GetTexture(new Guid("5e7e1313-fa7c-4f71-9a6e-e2650a7af968"))
            };
            AddControl(0, _acceptButton);
            CheckValidUserInput();

            UpdateUsersList();

            AddControl(0, new ButtonControl(Parent, clicked: (x) =>
            {
                SwitchView(new MainMenu.MainMenuView(Parent));
            })
            {
                Y = 900,
                X = 750,
                Width = 200,
                Height = 50,
                Text = "Back",
                Font = BasicFonts.GetFont(24),
                FontColor = Color.White,
                FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
            });

#if DEBUG
            AddControl(0, new ButtonControl(Parent, clicked: (x) => SwitchView(new UsersScreenView(Parent)))
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
                var newControl = new ButtonControl(Parent, ChangeUserButton_Click)
                {
                    X = 200,
                    Y = 280 + (count * 50 + 5),
                    Width = 400,
                    Height = 50,
                    Text = $"{user.Name}",
                    Font = BasicFonts.GetFont(16),
                    FontColor = Color.White,
                    FillColor = Parent.UIResources.GetTexture(new Guid("0ab3a089-b713-4853-aff6-8c7d8d565048")),
                    FillClickedColor = Parent.UIResources.GetTexture(new Guid("78bbfd61-b6de-416a-80ba-e53360881759")),
                    FillDisabledColor = Parent.UIResources.GetTexture(new Guid("6fb75caf-80ca-4f03-a1bb-2485b48aefd8")),
                    Tag = user
                };
                if (user.IsPrimary)
                    newControl.FillColor = Parent.UIResources.GetTexture(new Guid("86911ca2-ebf3-408c-98f9-6221d9a322bc"));
                _usersButtons.Add(newControl);
                AddControl(1, newControl);

                var newDeleteControl = new ButtonControl(Parent, RemoveUserButton_Click)
                {
                    X = 600,
                    Y = 280 + (count++ * 50 + 5),
                    Width = 200,
                    Height = 50,
                    Text = "Delete",
                    Font = BasicFonts.GetFont(16),
                    FontColor = Color.Red,
                    FillColor = Parent.UIResources.GetTexture(new Guid("aa60f60c-a792-425b-a225-5735e5a33cc9")),
                    FillClickedColor = Parent.UIResources.GetTexture(new Guid("12a9ad25-3e34-4398-9c61-6522c49f5dd8")),
                    FillDisabledColor = Parent.UIResources.GetTexture(new Guid("5e7e1313-fa7c-4f71-9a6e-e2650a7af968")),
                    Tag = user
                };
                _usersDeleteButtons.Add(newDeleteControl);
                AddControl(2, newDeleteControl);


            }
        }
    }
}
