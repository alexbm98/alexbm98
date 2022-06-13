using UnityEngine;
using System.Collections.Generic;

namespace MercunaExamples
{
    class PointGenerator : ScriptableObject
    {
        public static List<Vector3> CreateRing(Vector3 center, int numPointsPerRing, int numRings, float innerRadius, float outerRadius, int numLayers, float minHeight, float maxHeight)
        {
            List<Vector3> points = new List<Vector3>();

            float deltaHeight = numLayers > 1 ? (maxHeight - minHeight) / (numLayers - 1) : 0.0f;
            float deltaRadius = numRings > 1 ? (outerRadius - innerRadius) / (numRings - 1) : 0.0f;
            float angleIncrement = 2 * Mathf.PI / numPointsPerRing;

            for (int i = 0; i < numLayers; i++)
            {
                float height = minHeight + deltaHeight * i;

                for (int j = 0; j < numRings; j++)
                {
                    float radius = innerRadius + deltaRadius * j;

                    for (int k = 0; k < numPointsPerRing; k++)
                    {
                        float angle = k * angleIncrement;
                        points.Add(new Vector3(center.x + Mathf.Cos(angle) * radius, center.y + height, center.z + Mathf.Sin(angle) * radius));
                    }

                }
            }

            return points;
        }
    }
}
