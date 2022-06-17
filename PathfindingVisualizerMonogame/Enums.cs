using System;
using System.Collections.Generic;
using System.Text;

namespace PathfindingVisualizerMonogame
{
    public enum ClickResult
    {
        LeftClicked,
        Hovering,
        RightClicked,
        Nothing
    }
    public enum SquareType
    {
        Default,
        Start,
        End,
        Wall,
        Path,
        Visited
    }
    public enum AlgorithmType
    {
        Astar,
        Dijkstra,
        BFS
    }
}
