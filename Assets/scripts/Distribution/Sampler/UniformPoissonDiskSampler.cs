//using System;
//using System.Collections.Generic;
//using Random = System.Random;

//namespace LuftSchloss.Util {
//    // Adapated from java source by Herman Tulleken
//    // http://www.luma.co.za/labs/2008/02/27/poisson-disk-sampling/

//    // The algorithm is from the "Fast Poisson Disk Sampling in Arbitrary Dimensions" paper by Robert Bridson
//    // http://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph07-poissondisk.pdf

//    public static class UniformPoissonDiskSampler {
//        public const int DefaultPointsPerIteration = 30;

//        static readonly float SquareRootTwo = (float) Math.Sqrt(2);

//        struct Settings {
//            public Vector2f TopLeft, LowerRight, Center;
//            public Vector2f Dimensions;
//            public float? RejectionSqDistance;
//            public float MinimumDistance;
//            public float CellSize;
//            public int GridWidth, GridHeight;
//        }

//        struct State {
//            public Vector2f?[,] Grid;
//            public List<Vector2f> ActivePoints, Points;
//        }

//        public static List<Vector2f> SampleCircle(Vector2f center, float radius, float minimumDistance) {
//            return SampleCircle(center, radius, minimumDistance, DefaultPointsPerIteration);
//        }
//        public static List<Vector2f> SampleCircle(Vector2f center, float radius, float minimumDistance, int pointsPerIteration) {
//            return Sample(center - new Vector2f(radius), center + new Vector2f(radius), radius, minimumDistance, pointsPerIteration);
//        }

//        public static List<Vector2f> SampleRectangle(Vector2f topLeft, Vector2f lowerRight, float minimumDistance) {
//            return SampleRectangle(topLeft, lowerRight, minimumDistance, DefaultPointsPerIteration);
//        }
//        public static List<Vector2f> SampleRectangle(Vector2f topLeft, Vector2f lowerRight, float minimumDistance, int pointsPerIteration) {
//            return Sample(topLeft, lowerRight, null, minimumDistance, pointsPerIteration);
//        }

//        static List<Vector2f> Sample(Vector2f topLeft, Vector2f lowerRight, float? rejectionDistance, float minimumDistance, int pointsPerIteration) {
//            var settings = new Settings {
//                TopLeft = topLeft,
//                LowerRight = lowerRight,
//                Dimensions = lowerRight - topLeft,
//                Center = (topLeft + lowerRight) / 2,
//                CellSize = minimumDistance / SquareRootTwo,
//                MinimumDistance = minimumDistance,
//                RejectionSqDistance = rejectionDistance == null ? null : rejectionDistance * rejectionDistance
//            };
//            settings.GridWidth = (int) (settings.Dimensions.x / settings.CellSize) + 1;
//            settings.GridHeight = (int) (settings.Dimensions.y / settings.CellSize) + 1;

//            var state = new State {
//                Grid = new Vector2f?[settings.GridWidth, settings.GridHeight],
//                ActivePoints = new List<Vector2f>(),
//                Points = new List<Vector2f>()
//            };

//            AddFirstPoint(ref settings, ref state);

//            while (state.ActivePoints.Count != 0) {
//                var listIndex = RandomHelper.Random.Next(state.ActivePoints.Count);

//                var point = state.ActivePoints[listIndex];
//                var found = false;

//                for (var k = 0; k < pointsPerIteration; k++)
//                    found |= AddNextPoint(point, ref settings, ref state);

//                if (!found)
//                    state.ActivePoints.RemoveAt(listIndex);
//            }

//            return state.Points;
//        }

//        static void AddFirstPoint(ref Settings settings, ref State state) {
//            var added = false;
//            while (!added) {
//                var d = RandomHelper.Random.NextDouble();
//                var xr = settings.TopLeft.x + settings.Dimensions.x * d;

//                d = RandomHelper.Random.NextDouble();
//                var yr = settings.TopLeft.y + settings.Dimensions.y * d;

//                var p = new Vector2f((float) xr, (float) yr);
//                if (settings.RejectionSqDistance != null && Vector2f.DistanceSquared(settings.Center, p) > settings.RejectionSqDistance)
//                    continue;
//                added = true;

//                var index = Denormalize(p, settings.TopLeft, settings.CellSize);

//                state.Grid[(int) index.x, (int) index.y] = p;

//                state.ActivePoints.Add(p);
//                state.Points.Add(p);
//            }
//        }

//        static bool AddNextPoint(Vector2f point, ref Settings settings, ref State state) {
//            var found = false;
//            var q = GenerateRandomAround(point, settings.MinimumDistance);

//            if (q.x >= settings.TopLeft.x && q.x < settings.LowerRight.x &&
//                q.y > settings.TopLeft.y && q.y < settings.LowerRight.y &&
//                (settings.RejectionSqDistance == null || Vector2f.DistanceSquared(settings.Center, q) <= settings.RejectionSqDistance)) {
//                var qIndex = Denormalize(q, settings.TopLeft, settings.CellSize);
//                var tooClose = false;

//                for (var i = (int) Math.Max(0, qIndex.x - 2); i < Math.Min(settings.GridWidth, qIndex.x + 3) && !tooClose; i++)
//                    for (var j = (int) Math.Max(0, qIndex.y - 2); j < Math.Min(settings.GridHeight, qIndex.y + 3) && !tooClose; j++)
//                        if (state.Grid[i, j].HasValue && Vector2f.Distance(state.Grid[i, j].Value, q) < settings.MinimumDistance)
//                            tooClose = true;

//                if (!tooClose) {
//                    found = true;
//                    state.ActivePoints.Add(q);
//                    state.Points.Add(q);
//                    state.Grid[(int) qIndex.x, (int) qIndex.y] = q;
//                }
//            }
//            return found;
//        }

//        static Vector2f GenerateRandomAround(Vector2f center, float minimumDistance) {
//            var d = RandomHelper.Random.NextDouble();
//            var radius = minimumDistance + minimumDistance * d;

//            d = RandomHelper.Random.NextDouble();
//            var angle = MathHelper.TwoPi * d;

//            var newX = radius * Math.Sin(angle);
//            var newY = radius * Math.Cos(angle);

//            return new Vector2f((float) (center.x + newX), (float) (center.y + newY));
//        }

//        static Vector2f Denormalize(Vector2f point, Vector2f origin, double cellSize) {
//            return new Vector2f((int) ((point.x - origin.x) / cellSize), (int) ((point.y - origin.y) / cellSize));
//        }
//    }

//    //public static class RandomHelper {
//    //    public static readonly Random Random = new Random();
//    //}

//    //public static class MathHelper {
//    //    public const float Pi = (float) Math.PI;
//    //    public const float HalfPi = (float) (Math.PI / 2);
//    //    public const float TwoPi = (float) (Math.PI * 2);
//    //}	
//}
