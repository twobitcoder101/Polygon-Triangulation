using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Flat.Physics
{
    public enum WindingOrder
    {
        Clockwise, CounterClockwise, Invalid
    }

    public static class PolygonHelper
    {
        public static float FindPolygonArea(Vector2[] vertices)
        {
            float totalArea = 0f;

            for(int i = 0; i < vertices.Length; i++)
            {
                Vector2 a = vertices[i];
                Vector2 b = vertices[(i+1) % vertices.Length];

                float dy = (a.Y + b.Y) / 2f;
                float dx = b.X - a.X;

                float area = dy * dx;
                totalArea += area;
            }

            return MathF.Abs(totalArea);
        }

        public static bool Triangulate(Vector2[] vertices, out int[] triangles, out string errorMessage)
        {
            triangles = null;
            errorMessage = string.Empty;

            if(vertices is null)
            {
                errorMessage = "The vertex list is null.";
                return false;
            }

            if(vertices.Length < 3)
            {
                errorMessage = "The vertex list must have at least 3 vertices.";
                return false;
            }

            if(vertices.Length > 1024)
            {
                errorMessage = "The max vertex list length is 1024";
                return false;
            }

            //if (!PolygonHelper.IsSimplePolygon(vertices))
            //{
            //    errorMessage = "The vertex list does not define a simple polygon.";
            //    return false;
            //}

            //if(PolygonHelper.ContainsColinearEdges(vertices))
            //{
            //    errorMessage = "The vertex list contains colinear edges.";
            //    return false;
            //}

            //PolygonHelper.ComputePolygonArea(vertices, out float area, out WindingOrder windingOrder);

            //if(windingOrder is WindingOrder.Invalid)
            //{
            //    errorMessage = "The vertices list does not contain a valid polygon.";
            //    return false;
            //}

            //if(windingOrder is WindingOrder.CounterClockwise)
            //{
            //    Array.Reverse(vertices);
            //}

            List<int> indexList = new List<int>();
            for(int i = 0; i < vertices.Length; i++)
            {
                indexList.Add(i);
            }

            int totalTriangleCount = vertices.Length - 2;
            int totalTriangleIndexCount = totalTriangleCount * 3;

            triangles = new int[totalTriangleIndexCount];
            int triangleIndexCount = 0;

            while(indexList.Count > 3)
            {
                for (int i = 0; i < indexList.Count; i++)
                {
                    int a = indexList[i];
                    int b = Util.GetItem(indexList, i - 1);
                    int c = Util.GetItem(indexList, i + 1);

                    Vector2 va = vertices[a];
                    Vector2 vb = vertices[b];
                    Vector2 vc = vertices[c];

                    Vector2 va_to_vb = vb - va;
                    Vector2 va_to_vc = vc - va;

                    // Is ear test vertex convex?
                    if(Util.Cross(va_to_vb, va_to_vc) < 0f)
                    {
                        continue;
                    }

                    bool isEar = true;

                    // Does test ear contain any polygon vertices?
                    for(int j = 0; j < vertices.Length; j++)
                    {
                        if(j == a || j == b || j == c)
                        {
                            continue;
                        }

                        Vector2 p = vertices[j];

                        if(PolygonHelper.IsPointInTriangle(p, vb, va, vc))
                        {
                            isEar = false;
                            break;
                        }                            
                    }

                    if(isEar)
                    {
                        triangles[triangleIndexCount++] = b;
                        triangles[triangleIndexCount++] = a;
                        triangles[triangleIndexCount++] = c;

                        indexList.RemoveAt(i);
                        break;
                    }
                }
            }

            triangles[triangleIndexCount++] = indexList[0];
            triangles[triangleIndexCount++] = indexList[1];
            triangles[triangleIndexCount++] = indexList[2];

            return true;
        }

        public static bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            Vector2 ab = b - a;
            Vector2 bc = c - b;
            Vector2 ca = a - c;

            Vector2 ap = p - a;
            Vector2 bp = p - b;
            Vector2 cp = p - c;

            float cross1 = Util.Cross(ab, ap);
            float cross2 = Util.Cross(bc, bp);
            float cross3 = Util.Cross(ca, cp);

            if(cross1 > 0f || cross2 > 0f || cross3 > 0f)
            {
                return false;
            }

            return true;
        }

        public static bool IsSimplePolygon(Vector2[] vertices)
        {
            throw new NotImplementedException();
        }

        public static bool ContainsColinearEdges(Vector2[] vertices)
        {
            throw new NotImplementedException();
        }

        public static void ComputePolygonArea(Vector2[] vertices, out float area, out WindingOrder windingOrder)
        {
            throw new NotImplementedException();
        }
    }
}