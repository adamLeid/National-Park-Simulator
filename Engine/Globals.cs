#region Includes
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace National_Park_Simulator.Engine
{
    // passes anything through this function. The various delegate functions are defined in Game globals and world
    public delegate void PassObject(object i);
    // passes anything and returns anything you want in this function
    public delegate object PassObjectAndReturn(object i);

    public class Globals
    {
        // Fields to hold various variables
        public static string[] stringArray;

        public static GraphicsDevice GraphicsDevice;
        // Native Dimensions
        public static float nativeScreenWidth = 640f;
        // Native Dimensions
        public static float nativeScreenHeight = 360f;
        // Largest Width
        public static int DrawImageMaxWidth = 5140;
        // Current Screen Dimensions
        public static int screenHeight, screenWidth;
        // Current Scale
        public static float MasterScale = 1f;
        // Previouse Scale
        public static float PrevMasterScale;
        // Map Dimensions
        public static int MapWidth, MapHeight;

        public static System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US"); // for xml parsing
        // Font Paths
        public static string FontS6 = "Fonts/RyftHavenS6";
        public static string FontS12 = "Fonts/RyftHavenS12";
        public static string FontS16 = "Fonts/RyftHavenS16";
        public static string FontS24 = "Fonts/RyftHavenS24";
        // AA Shader
        public static Effect AA_Effect;

        public static string appDataFilePath = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/RyftHaven/";

        public static GameTime gameTime;

        //public static Camera camera;

        //default to false
        public static bool isCameraActive = false;

        public const float zoomDif = 0.1f;

        // Grid display
        public static bool DrawGrid = true;
        public static int TileGridSize = 16;
        // Collision display
        public static bool DisplayCollision = false;
        // Clock wise rotation float in radians
        public static float NintyDegreeRotCW = 1.5708f;
        // Counter clock wise rotation float in radians
        public static float NintyDegreeRotCCW = -1.5708f;

        //returns the distance between two vectors
        public static float GetDistance(Vector2 pos, Vector2 target)
        {
            return Vector2.Distance(pos, target);
        }

        /// <summary>
        /// Helps you move the exact distance you need at a constant speed
        /// </summary>
        /// <param name="focus"></param>
        /// <param name="pos"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static Vector2 RadialMovement(Vector2 focus, Vector2 pos, float speed)
        {
            float dist = Globals.GetDistance(pos, focus);

            if (dist <= speed)
            {
                return focus - pos;
            }
            else
            {
                return (focus - pos) * speed / dist;
            }
        }

        /// <summary>
        /// Returns the angle required to rotate towards the focus point from the position point.
        /// </summary>
        /// <param name="Pos"></param>
        /// <param name="focus"></param>
        /// <returns></returns>
        public static float RotateTowards(Vector2 Pos, Vector2 focus)
        {

            float h, sineTheta, angle;
            if (Pos.Y - focus.Y != 0)
            {
                h = (float)Math.Sqrt(Math.Pow(Pos.X - focus.X, 2) + Math.Pow(Pos.Y - focus.Y, 2));
                sineTheta = (float)(Math.Abs(Pos.Y - focus.Y) / h); //* ((item.Pos.Y-focus.Y)/(Math.Abs(item.Pos.Y-focus.Y))));
            }
            else
            {
                h = Pos.X - focus.X;
                sineTheta = 0;
            }

            angle = (float)Math.Asin(sineTheta);

            // Drawing diagonial lines here.
            //Quadrant 2
            if (Pos.X - focus.X > 0 && Pos.Y - focus.Y > 0)
            {
                angle = (float)(Math.PI * 3 / 2 + angle);
            }
            //Quadrant 3
            else if (Pos.X - focus.X > 0 && Pos.Y - focus.Y < 0)
            {
                angle = (float)(Math.PI * 3 / 2 - angle);
            }
            //Quadrant 1
            else if (Pos.X - focus.X < 0 && Pos.Y - focus.Y > 0)
            {
                angle = (float)(Math.PI / 2 - angle);
            }
            else if (Pos.X - focus.X < 0 && Pos.Y - focus.Y < 0)
            {
                angle = (float)(Math.PI / 2 + angle);
            }
            else if (Pos.X - focus.X > 0 && Pos.Y - focus.Y == 0)
            {
                angle = (float)Math.PI * 3 / 2;
            }
            else if (Pos.X - focus.X < 0 && Pos.Y - focus.Y == 0)
            {
                angle = (float)Math.PI / 2;
            }
            else if (Pos.X - focus.X == 0 && Pos.Y - focus.Y > 0)
            {
                angle = (float)0;
            }
            else if (Pos.X - focus.X == 0 && Pos.Y - focus.Y < 0)
            {
                angle = (float)Math.PI;
            }

            return angle;
        }

        public static double RotateTowards2(Vector2 Center, Vector2 Target, int sigFig)
        {
            double x = Target.X - Center.X;
            double y = Target.Y - Center.Y;
            return Math.Round(Math.Atan2(y, x) * (180d / Math.PI), sigFig);
        }

        /// <summary>
        /// Flattens a multidimensional array for serialization
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static T[] Flatten<T>(T[,] arr)
        {
            int rows0 = arr.GetLength(0);
            int rows1 = arr.GetLength(1);
            T[] arrFlattened = new T[rows0 * rows1];
            for (int j = 0; j < rows1; j++)
            {
                for (int i = 0; i < rows0; i++)
                {
                    var test = arr[i, j];
                    arrFlattened[i + j * rows0] = arr[i, j];
                }
            }
            return arrFlattened;
        }
        /// <summary>
        /// Expands the flattened array to multidimensional array via row count desired
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="rows0"></param>
        /// <returns></returns>
        public static T[,] Expand<T>(T[] arr, int rows0)
        {
            int length = arr.GetLength(0);
            int rows1 = length / rows0;
            T[,] arrExpanded = new T[rows0, rows1];
            for (int j = 0; j < rows1; j++)
            {
                for (int i = 0; i < rows0; i++)
                {
                    arrExpanded[i, j] = arr[i + j * rows0];
                }
            }
            return arrExpanded;
        }

    }
}