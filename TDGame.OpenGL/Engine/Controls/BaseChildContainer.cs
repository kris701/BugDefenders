using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TDGame.OpenGL.Engine.Controls
{
    public abstract class BaseChildContainer : BaseControl, IChildContainer
    {
        public IControl Child { get; set; }

        public BaseChildContainer()
        {
            Child = new NoneControl();
        }

        public override void Initialize()
        {
            Child.UIChanged += () => UpdateUI();
            Child.Initialize();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            Child.Draw(gameTime, spriteBatch);
        }

        public override void Refresh()
        {
            if (!IsVisible)
                return;
            if (!IsEnabled)
                return;

            Child.Refresh();
        }

        public override void LoadContent(ContentManager content)
        {
            Child.LoadContent(content);
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsVisible)
                return;
            if (!IsEnabled)
                return;

            Child.Update(gameTime);
        }

        public override List<IControl> GetLookupStack()
        {
            var newList = new List<IControl>();
            newList.Add(this);
            newList.AddRange(Child.GetLookupStack());
            return newList;
        }
    }
}
