using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PGSauce.Geometry
{
    public static class PGeometry
    {
        /// <summary>
        /// Where is p in relation to a-b
        /// positive -> to the right
        /// zero -> on the line
        /// negative -> to the left
        /// </summary>
        /// <param name="a">Point A of the line</param>
        /// <param name="b">Point B of the line</param>
        /// <param name="p">Tested point</param>
        /// <returns>A determinant</returns>
        public static float WhereIsPointInRelationToLine(Vector2 a, Vector2 b, Vector2 p)
        {
            float determinant = (a.x - p.x) * (b.y - p.y) - (a.y - p.y) * (b.x - p.x);

            return determinant;
        }
        
        /// <summary>
        /// Where is p in relation to line
        /// positive -> to the right
        /// zero -> on the line
        /// negative -> to the left
        /// </summary>
        /// <param name="line">Line</param>
        /// <param name="p">Tested point</param>
        /// <returns>A determinant</returns>
        public static float WhereIsPointInRelationToLine(this Line line, Vector2 p)
        {
            return WhereIsPointInRelationToLine(line.A, line.B, p);
        }
    }
}
