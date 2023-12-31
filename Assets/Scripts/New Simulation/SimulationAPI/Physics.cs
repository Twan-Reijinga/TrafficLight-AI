using System;
using System.Collections.Generic;
using System.Linq;

namespace SimulationAPI
{
    public class Physics
    {
        Simulator super;

        public Physics(Simulator super)
        {
            this.super = super;
        }

        /// <summary>
        /// line line intersection between two lines given two points each, line 1 extends infinitely beyond point 2
        /// </summary>
        /// <param name="line1p1">The origin of the first line</param>
        /// <param name="line1p2">The direction of the first line, in relation to the origin</param>
        /// <param name="line2p1">point 1 of the line to intersect with</param>
        /// <param name="line2p2">point 2 of the line to intersect with</param>
        /// <returns>Returns a Vector2 if hit, otherwise null</returns>

        public static Vector2 LineLineIntersect(Vector2 line1p1, Vector2 line1p2, Vector2 line2p1, Vector2 line2p2)
        {
            float d =
                (line1p2.x - line1p1.x) * (line2p2.y - line2p1.y) - (line1p2.y - line1p1.y) * (line2p2.x - line2p1.x);

            if (d == 0)
            {
                return null; // no intersection
            }

            float t = ((line2p1.x - line1p1.x) * (line1p2.y - line1p1.y) - (line2p1.y - line1p1.y) * (line1p2.x - line1p1.x)) / d;
            float u = ((line2p2.y - line2p1.y) * (line2p2.x - line1p1.x) + (line2p1.x - line2p2.x) * (line2p2.y - line1p1.y)) / d;

            if (u > 0 && t > 0 && t < 1)
            {
                Vector2 intersection = new Vector2(
                    line2p1.x + t * (line2p2.x - line2p1.x),
                    line2p1.y + t * (line2p2.y - line2p1.y)
                );
                return intersection;
            }
            return null; // no intersection
        }
    }
}