using Microsoft.Xna.Framework;
using National_Park_Simulator.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace National_Park_Simulator.Engine.Utils
{
    public class PriorityQueue<TElement, TPriority>
    {
        private List<Tuple<TElement, TPriority>> elements = new List<Tuple<TElement, TPriority>>();

        public int Count
        {
            get { return elements.Count; }
        }

        public void Enqueue(TElement item, TPriority priority)
        {
            elements.Add(Tuple.Create(item, priority));
        }

        public TElement Dequeue()
        {
            Comparer<TPriority> comparer = Comparer<TPriority>.Default;
            int bestIndex = 0;

            for (int i = 0; i < elements.Count; i++)
            {
                if (comparer.Compare(elements[i].Item2, elements[bestIndex].Item2) < 0)
                {
                    bestIndex = i;
                }
            }

            TElement bestItem = elements[bestIndex].Item1;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }
    public static class AStar
    {
        private static List<Point> DIRS = new List<Point> {
            new Point(1, 0),
            new Point(0, -1),
            new Point(-1, 0),
            new Point(0, 1)
        };
        /*private static Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();
        private static Dictionary<Point, double> costSoFar = new Dictionary<Point, double>();*/
        public static double Heuristic(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        public static Dictionary<Point, Point> FindPath(Map map, Point start, Point goal)
        {
            Dictionary<Point, Point> path = new Dictionary<Point, Point>();
            Dictionary<Point, double> costSoFar = new Dictionary<Point, double>();
            PriorityQueue<Point, double> frontier = new PriorityQueue<Point, double>();
            frontier.Enqueue(start, 0);

            path[start] = start;
            costSoFar[start] = 0;

            while (frontier.Count > 0)
            {
                Point current = frontier.Dequeue();

                if (current.Equals(goal))
                {
                    break;
                }

                foreach (Point next in NeighborsForVisitor(current, map))
                {
                    double newCost = costSoFar[current]
                        + map.Cost(next);
                    if (!costSoFar.ContainsKey(next)
                        || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        double priority = newCost + Heuristic(next, goal);
                        frontier.Enqueue(next, priority);
                        path[next] = current;
                    }
                }
            }

            return path;
        }


        private static List<Point> NeighborsForVisitor(Point center, Map map)
        {
            List<Point> points = new List<Point>();
            foreach (Point d in DIRS)
            {
                Point next = new Point(center.X + d.X, center.Y + d.Y);
                if (map.InBounds(next) && map.Passable(next))
                {
                    points.Add(next);
                }
            }
            return points;
        }
    }
}
