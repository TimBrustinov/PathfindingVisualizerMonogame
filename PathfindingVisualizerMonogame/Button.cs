using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathfindingVisualizerMonogame
{
    class Button
    {
        public Texture2D Texture;
        public Vector2 Position;
        public Vector2 Dimentions;
        public Color Color { get; set; }
        public Rectangle Hitbox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)Dimentions.X, (int)Dimentions.Y);
            }
        }

        public Button(Texture2D texture, Vector2 position, Vector2 dim, Color color)
        {
            Texture = texture;
            Position = position;
            Dimentions = dim;
            Color = color;
        }
        public ClickResult GetMouseAction(MouseState ms)
        {
            // make color change in class
            if (Hitbox.Contains(ms.Position))
            {
                if (ms.LeftButton == ButtonState.Pressed)
                {
                    return ClickResult.LeftClicked;
                }
                else if(ms.RightButton == ButtonState.Pressed)
                {
                    return ClickResult.RightClicked;
                }
                return ClickResult.Hovering;
            }
            return ClickResult.Nothing;
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, (int)Dimentions.X, (int)Dimentions.Y), Color);
        }
    }
}
