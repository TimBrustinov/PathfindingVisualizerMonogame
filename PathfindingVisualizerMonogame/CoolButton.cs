using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathfindingVisualizerMonogame
{
    class CoolButton : Button
    {
        public SpriteFont Font;
        public Color StringColor;
        string Text;
        public AlgorithmType AlgorithmType;
        public CoolButton(SpriteFont font, string text, Texture2D texture, Vector2 position, Vector2 dim, Color boxColor, Color stringColor, AlgorithmType type)
            : base(texture, position, dim, boxColor)
        {
            Font = font;
            StringColor = stringColor;
            Text = text;
            AlgorithmType = type;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.DrawString(Font, Text, Position, StringColor);
        }
    }
}
