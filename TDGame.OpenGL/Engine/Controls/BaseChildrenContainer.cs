using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TDGame.OpenGL.Engine.Controls
{
    public abstract class BaseChildrenContainer : BaseControl, IChildrenContainer
    {
        public List<IControl> Children { get; set; }

        public BaseChildrenContainer()
        {
            Children = new List<IControl>();
        }

        public override void Initialize()
        {
            foreach (var child in Children)
            {
                child.UIChanged += () => UpdateUI();
                child.Initialize();
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            foreach (var child in Children)
                child.Draw(gameTime, spriteBatch);
        }

        public override void LoadContent(ContentManager content)
        {
            foreach (var child in Children)
                child.LoadContent(content);
        }

        public override void Refresh()
        {
            if (!IsVisible)
                return;
            if (!IsEnabled)
                return;

            foreach (var child in Children)
                    child.Refresh();
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsVisible)
                return;
            if (!IsEnabled)
                return;

            foreach (var child in Children)
                    child.Update(gameTime);
        }

        public override List<IControl> GetLookupStack()
        {
            var newList = new List<IControl>();
            newList.Add(this);
            foreach (var child in Children)
                newList.AddRange(child.GetLookupStack());
            return newList;
        }
    }
}
