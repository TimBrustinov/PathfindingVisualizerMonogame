using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PathfindingVisualizerMonogame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        Texture2D texture;
        GridSquare[,] gridSquares;
        List<GridSquare> typePallete = new List<GridSquare>();
        List<CoolButton> coolButtons = new List<CoolButton>();

        Texture2D pixel;
        GridSquare square;


        MouseState ms = new MouseState();
        SquareType selectedType = SquareType.Default;

        public Dictionary<SquareType, Color> TypeToColor = new Dictionary<SquareType, Color>();
        public Dictionary<Color, SquareType> ColorToType = new Dictionary<Color, SquareType>();

        SpriteFont font;

        //Keep track of previous green, and previous red
        GridSquare start;
        GridSquare end;

        List<GridSquare> path;
        List<GridSquare> visitedSquares;

        public int pathCount;
        public int visitedCount;

        //think of a good name  #brainstorming
        Dictionary<Point, Vertex<GridSquare>> PositionToVertex = new Dictionary<Point, Vertex<GridSquare>>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        int rows = 20;
        int cols = 20;
        int spacing = 1;
        int size = 20;
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = cols * (size + spacing) - spacing + 100;
            graphics.PreferredBackBufferHeight = rows * (size + spacing) - spacing;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //start.Color = Color.Green;
            //end.Color = Color.Red;
            //texture = Content.Load<Texture2D>("beachball");
            //ball = new Ball(texture, new Vector2(1, 1), new Vector2(5, 5));

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            gridSquares = new GridSquare[rows, cols];

            // Creates grid squares
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    var square = new GridSquare(pixel, new Vector2((size + spacing) * x, (size + spacing) * y), new Vector2(size, size), Color.White, new Point(x, y));
                    gridSquares[y, x] = square;
                }
            }
            // creates color pallete 
            Color[] colors = new Color[] { Color.Red, Color.Green, Color.Gray };

            TypeToColor.Add(SquareType.Start, Color.Green);
            TypeToColor.Add(SquareType.End, Color.Red);
            TypeToColor.Add(SquareType.Default, Color.White);
            TypeToColor.Add(SquareType.Wall, Color.Gray);
            TypeToColor.Add(SquareType.Path, Color.Aqua);
            TypeToColor.Add(SquareType.Visited, Color.LightGreen);

            ColorToType = TypeToColor.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            int palleteSpacing = size + 5;
            int yOffset = 0;
            for (int i = 0; i < colors.Length; i++)
            {
                //Make this a button not a grid square!
                var currentColor = new GridSquare(pixel, new Vector2(GraphicsDevice.Viewport.Bounds.Width - size, i * palleteSpacing + yOffset), new Vector2(size, size), colors[i], new Point(-1, -1));
                currentColor.Type = ColorToType[currentColor.Color];
                typePallete.Add(currentColor);
            }



            font = Content.Load<SpriteFont>("Font");
            int coolButtonsSpacing = 10;
            Vector2 dimentions = font.MeasureString("BFS");
            var bfs = new CoolButton(font, "BFS", pixel, new Vector2(GraphicsDevice.Viewport.Bounds.Width - dimentions.X, typePallete[typePallete.Count - 1].Position.Y + dimentions.Y + coolButtonsSpacing), dimentions, Color.Gray, Color.White, AlgorithmType.BFS);
            dimentions = font.MeasureString("Dijkstra");
            var dijkstraAlgoritm = new CoolButton(font, "Dijkstra", pixel, new Vector2(GraphicsDevice.Viewport.Bounds.Width - dimentions.X, bfs.Position.Y + dimentions.Y + coolButtonsSpacing), dimentions, Color.Gray, Color.White, AlgorithmType.Dijkstra);
            dimentions = font.MeasureString("AStar");
            var aStar = new CoolButton(font, "AStar", pixel, new Vector2(GraphicsDevice.Viewport.Bounds.Width - dimentions.X, dijkstraAlgoritm.Position.Y + dimentions.Y + coolButtonsSpacing), dimentions, Color.Gray, Color.White, AlgorithmType.Astar);

            coolButtons.Add(bfs);
            coolButtons.Add(dijkstraAlgoritm);
            coolButtons.Add(aStar);

            for (int y = 0; y < gridSquares.GetLength(0); y++)
            {
                for (int x = 0; x < gridSquares.GetLength(1); x++)
                {
                    var v = new Vertex<GridSquare>(gridSquares[y, x], x, y);
                    PositionToVertex.Add(new Point(x, y), v);
                }
            }

            // TODO: use this.Content to load your game content here
        }

        Stopwatch pathWatch = new Stopwatch();
        Stopwatch visitedWatch = new Stopwatch();
        
        protected override void Update(GameTime gameTime)
        {

            ms = Mouse.GetState();
            var pressed = ms.LeftButton == ButtonState.Pressed;

            if (pathWatch.ElapsedMilliseconds > 50 && path != null && visitedCount == visitedSquares.Count - 1)
            {
                if (pathCount < path.Count - 1)
                {
                    pathCount++;
                }
                pathWatch.Restart();
            }
            if (visitedWatch.ElapsedMilliseconds > 20 && visitedSquares != null)
            {
                if (visitedCount < visitedSquares.Count - 1)
                {
                    visitedCount++;
                }
                visitedWatch.Restart();
            }

            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            // TODO: Add your update logic here
            var rect = new Rectangle();
            var rect2 = gridSquares[0, 0].Hitbox;

            for (int i = 0; i < typePallete.Count; i++)
            {
                if (typePallete[i].GetMouseAction(ms) == ClickResult.LeftClicked)
                {
                    selectedType = ColorToType[typePallete[i].Color];
                }
            }

            bool changeMade = false;
            // fix squares not staying the same color
            for (int y = 0; y < gridSquares.GetLength(0); y++)
            {
                for (int x = 0; x < gridSquares.GetLength(1); x++)
                {
                    if (gridSquares[y, x].GetMouseAction(ms) == ClickResult.LeftClicked)
                    {
                        if (selectedType == SquareType.Start)
                        {
                            if (gridSquares[y, x].Type != SquareType.End)
                            {
                                if (start != null)
                                {
                                    start.Type = SquareType.Default;
                                }
                                start = gridSquares[y, x];
                                changeMade = true;
                            }
                        }

                        if (selectedType == SquareType.End)
                        {
                            if (gridSquares[y, x].Type != SquareType.Start)
                            {
                                if (end != null)
                                {
                                    end.Type = SquareType.Default;
                                }
                                end = gridSquares[y, x];
                                changeMade = true;
                            }
                        }

                        if (gridSquares[y, x].Type == SquareType.Start || gridSquares[y, x].Type == SquareType.End) continue;

                        gridSquares[y, x].Type = selectedType;
                        gridSquares[y, x].Color = TypeToColor[selectedType];
                        changeMade = true;

                    }
                    else if (gridSquares[y, x].GetMouseAction(ms) == ClickResult.RightClicked)
                    {
                        gridSquares[y, x].Type = SquareType.Default;
                        changeMade = true;
                    }
                    else if (gridSquares[y, x].GetMouseAction(ms) == ClickResult.Hovering && gridSquares[y, x] != start && gridSquares[y, x] != end)
                    {
                        gridSquares[y, x].Color = TypeToColor[selectedType];
                    }
                    else
                    {
                        gridSquares[y, x].Color = TypeToColor[gridSquares[y, x].Type];
                    }
                }
            }
            if(changeMade)
            {
                ClearTiles();
            }
            for (int i = 0; i < coolButtons.Count; i++)
            {
                if (coolButtons[i].GetMouseAction(ms) == ClickResult.LeftClicked)
                {
                    coolButtons[i].Color = Color.Green;
                    Graph<GridSquare> graph = CreateGraph();
                    if(start != null && end != null)
                    {
                        Vertex<GridSquare> startVertex = PositionToVertex[start.Index];
                        Vertex<GridSquare> endVertex = PositionToVertex[end.Index];

                        if (coolButtons[i].AlgorithmType == AlgorithmType.Astar)
                        {
                            // DEBUG heap cuz its dosent work lol 
                            ClearTiles();
                            (path, visitedSquares) = graph.AStarSearchAlgorithm(startVertex, endVertex);
                            pathWatch.Start();
                            visitedWatch.Start();
                        }
                        else if (coolButtons[i].AlgorithmType == AlgorithmType.Dijkstra)
                        {
                            ClearTiles();
                            (path, visitedSquares) = graph.DijkstraAlgorithm(startVertex, endVertex);
                            pathWatch.Start();
                            visitedWatch.Start();
                        }
                        else
                        {
                            ClearTiles();
                            (path, visitedSquares) = graph.BFS(startVertex, endVertex);
                            pathWatch.Start();
                            visitedWatch.Start();
                        }
                        for (int j = 0; j < coolButtons.Count; j++)
                        {
                            if (coolButtons[j] != coolButtons[i])
                            {
                                coolButtons[j].Color = Color.Gray;
                            }
                        }
                        pathCount = 0;
                        visitedCount = 0;
                    }
                   
                    //for (int j = 1; j < path.Count - 1; j++)
                    //{
                    //    path[j].Type = SquareType.Path;
                    //}
                }
            }


            //for (int i = 0; i < list.Count; i++)
            //{
            //    if(list[i].GetMouseAction(ms) == ClickResult.Clicked)
            //    {
            //        list[i].Color = Color.Green;
            //        for (int j = 0; j < list.Count; j++)
            //        {
            //            if(list[i] != list[j])
            //            {
            //                list[j].Type = ColorToType[Color.White];
            //            }
            //        }
            //    }
            //}
            //ball.Velocity.X += 0.1f;
            //Position(ref x1, ref y1, ref xSpeed1, ref ySpeed1);
            //Position(ref x2, ref y2, ref xSpeed2, ref ySpeed2);

            base.Update(gameTime);
        }

        void ClearTiles()
        {
            if (path != null)
            {
                foreach (var item in path)
                {
                    if (item.Type == SquareType.Start || item.Type == SquareType.End) continue;
                    item.Type = SquareType.Default;
                }
                path = null;
            }
            if (visitedSquares != null)
            {
                foreach (var item in visitedSquares)
                {
                    if (item.Type == SquareType.Start || item.Type == SquareType.End) continue;
                    item.Type = SquareType.Default;
                }
                visitedSquares = null;
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            foreach (var square in gridSquares)
            {
                square.Draw(spriteBatch);
            }
            foreach (var square in typePallete)
            {
                square.Draw(spriteBatch);
            }
            foreach (var button in coolButtons)
            {
                button.Draw(spriteBatch);
            }
            if (visitedSquares != null)
            {
                for (int i = 1; i < visitedCount; i++)
                {
                    visitedSquares[i].Type = SquareType.Visited;
                    visitedSquares[i].Color = Color.LightGreen;
                    visitedSquares[i].Draw(spriteBatch);
                }
            }
            if (path != null && visitedCount == visitedSquares.Count - 1)
            {
                for (int i = 1; i < pathCount; i++)
                {
                    path[i].Type = SquareType.Path;
                    path[i].Color = Color.Aqua;
                    path[i].Draw(spriteBatch);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        Graph<GridSquare> CreateGraph()
        {
            Graph<GridSquare> graph = new Graph<GridSquare>();
            PositionToVertex.Clear();
            Point[] offsets = new Point[] {new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1), new Point(1, 1), new Point(1, -1), new Point(-1, 1),  new Point(-1, -1)};
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    var vertex = new Vertex<GridSquare>(gridSquares[y, x], x, y);
                    if (gridSquares[y, x].Type == SquareType.Wall) continue;
                    graph.AddVertex(vertex);
                    PositionToVertex.Add(new Point(x, y), vertex);
                }
            }
            ;
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    //if i am a wall continue
                    if (!PositionToVertex.ContainsKey(new Point(x, y)))
                    {
                        continue;
                    }

                    for (int i = 0; i < offsets.Length; i++)
                    {
                        Point newLocation = new Point(x + offsets[i].X, y + offsets[i].Y);
                      
                        if (newLocation.X >= 0 && newLocation.Y >= 0 && newLocation.X <= cols - 1 && newLocation.Y <= rows - 1 && PositionToVertex.ContainsKey(newLocation))
                        {
                            // Does not work with edges 
                            graph.AddEdge(PositionToVertex[new Point(x, y)], PositionToVertex[newLocation], FindDistance(x , y, newLocation.X, newLocation.Y));
                        }
                    }
                    //if (PositionToVertex.ContainsKey(new Point(x, y)) == false) continue;

                    //if (x > 0 && PositionToVertex.ContainsKey(new Point(x - 1, y)))
                    //{
                    //    graph.AddEdge(PositionToVertex[new Point(x, y)], PositionToVertex[new Point(x - 1, y)], 1);
                    //}
                    //if (x < cols - 1 && PositionToVertex.ContainsKey(new Point(x + 1, y)))
                    //{
                    //    graph.AddEdge(PositionToVertex[new Point(x, y)], PositionToVertex[new Point(x + 1, y)], 1);
                    //}
                    //if (y > 0 && PositionToVertex.ContainsKey(new Point(x, y - 1)))
                    //{
                    //    graph.AddEdge(PositionToVertex[new Point(x, y)], PositionToVertex[new Point(x, y - 1)], 1);
                    //}
                    //if (y < rows - 1 && PositionToVertex.ContainsKey(new Point(x, y + 1)))
                    //{
                    //    graph.AddEdge(PositionToVertex[new Point(x, y)], PositionToVertex[new Point(x, y + 1)], 1);
                    //}
                }
            }
            return graph;
        }
        public double FindDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

    }
}
