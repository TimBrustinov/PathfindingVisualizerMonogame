using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathfindingVisualizerMonogame
{
    class GridSquare : Button
    {
        public SquareType Type { get; set; }

        public Point Index { get; set; }
        public GridSquare(Texture2D texture, Vector2 position, Vector2 dimentions, Color color, Point index) 
            : base(texture, position, dimentions, color)
        {
            Index = index;
            Type = SquareType.Default;
        }
    }
}
