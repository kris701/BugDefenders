using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TDGame.OpenGL.Engine.Helpers;

namespace TDGame.OpenGL.Engine.Controls
{
    public class GridControl : BaseChildrenContainer
    {
        public string FillColorName { get; set; } = "";
        public Texture2D FillColor { get; set; } = BasicTextures.GetBasicRectange(Color.Transparent);
        public int Margin { get; set; } = 1;
        public List<int> RowDefinitions { get; set; } = new List<int>() { 1 };
        public List<int> ColumnDefinitions { get; set; } = new List<int>() { 1 };

        public GridControl()
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            spriteBatch.Draw(FillColor, new Rectangle(X, Y, Width, Height), Color.White);
            foreach (var child in Children)
                child.Draw(gameTime, spriteBatch);
        }

        public override void LoadContent(ContentManager content)
        {
            if (FillColorName != "")
                FillColor = content.Load<Texture2D>(FillColorName);

            foreach (var child in Children)
                child.LoadContent(content);
        }

        public override void Refresh()
        {
            if (!IsVisible)
                return;
            if (!IsEnabled)
                return;

            if (Height == 0)
            {
                foreach (var child in Children)
                {
                    if (child.IsVisible)
                    {
                        child.Refresh();
                        Height += child.Height + Margin;
                    }
                }
            }

            int totalColumnValue = 0;
            foreach (var column in ColumnDefinitions)
                totalColumnValue += column;
            int totalRowValue = 0;
            foreach (var row in RowDefinitions)
                totalRowValue += row;

            var yOffsets = new int[RowDefinitions.Count, ColumnDefinitions.Count];
            var xOffsets = new int[ColumnDefinitions.Count];
            for (int j = 0; j < ColumnDefinitions.Count; j++)
            {
                for (int i = 0; i < RowDefinitions.Count; i++)
                {
                    if (i > 0)
                        yOffsets[i, j] = yOffsets[i - 1, j] + (int)(((double)Height / (double)totalRowValue) * RowDefinitions[i - 1]);
                    else
                        yOffsets[i, j] = Y;
                }
                if (j > 0)
                    xOffsets[j] = xOffsets[j - 1] + (int)(((double)Width / (double)totalColumnValue) * ColumnDefinitions[j - 1]);
                else
                    xOffsets[j] = X;
            }

            foreach (var child in Children)
            {
                if (child.IsVisible)
                {
                    int row = GetBoundedRow(child.Row);
                    int column = GetBoundedColumn(child.Column);

                    child.X = xOffsets[column] + Margin;
                    child.Y = yOffsets[row, column] + Margin;
                    child.Width = GetColumnWithSpan(column, child.ColumnSpan, totalColumnValue);
                    child.Height = GetRowWithSpan(row, child.RowSpan, totalRowValue);
                    child.Refresh();
                }
            }
        }

        private int GetColumnWithSpan(int column, int span, int totalColumnValue)
        {
            int retVal = 0;
            for (int i = column; i < column + span; i++)
            {
                if (i >= ColumnDefinitions.Count)
                    break;
                retVal += (int)(((double)Width / (double)totalColumnValue) * ColumnDefinitions[i]);
            }
            return retVal - 2 * Margin;
        }

        private int GetRowWithSpan(int row, int span, int totalRowValue)
        {
            int retVal = 0;
            for (int i = row; i < row + span; i++)
            {
                if (i >= RowDefinitions.Count)
                    break;
                retVal += (int)(((double)Height / (double)totalRowValue) * RowDefinitions[i]);
            }
            return retVal - 2 * Margin;
        }

        private int GetBoundedRow(int row)
        {
            if (row >= RowDefinitions.Count)
                return RowDefinitions.Count - 1;
            return row;
        }

        private int GetBoundedColumn(int column)
        {
            if (column >= ColumnDefinitions.Count)
                return ColumnDefinitions.Count - 1;
            return column;
        }
    }
}
