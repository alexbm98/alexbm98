using System.Collections.Generic;
using UnityEngine;
using Mercuna;

namespace MercunaExamples
{
    /* Example behavior that will cause an agent to wander randomly around the navigable octree */
    [RequireComponent(typeof(Mercuna3DNavigation))]
    [RequireComponent(typeof(MercunaObstacle))]
    [AddComponentMenu("Mercuna 3D Navigation/Examples/Random Wander")]
    public class RandomWander : MonoBehaviour
    {
        public bool debugDrawQuery;
        private List<Vector3> debugPoints = new List<Vector3>();

        private void Start()
        {
            SetNextMove();
        }

        private void SetNextMove()
        {
            Mercuna3DNavigation navComponent = GetComponent<Mercuna3DNavigation>();
            MercunaNavOctree navOctree = MercunaNavOctree.GetInstance();

            int numAttempts = 0;
            debugPoints.Clear();

            // Ensure a non zero search radius
            float radius = navComponent.radius > 0.0f ? navComponent.radius : 0.1f;

            // Repeatedly try to generate a new destination
            while (true)
            {
                // Generate some random point a reasonable distance from the agent.
                Vector3 newDestination = transform.position + Random.onUnitSphere * radius * Random.Range(10.0f, 30.0f);

                // Save the new point for debug draw
                debugPoints.Add(newDestination);

                // Query Mercuna octree to check if the point is reachable
                if (navOctree.IsReachable(transform.position, newDestination, navComponent.radius))
                {
                    // New destination is reachable so navigate to it
                    navComponent.NavigateToLocation(newDestination, this.OnMoveComplete);
                    return;
                }

                // Limit to 100 attempts and then give up
                if (++numAttempts > 100)
                    break;
            }

            // Repeatedly failed to find any reachable points.  This agent will stop moving.
            Debug.LogErrorFormat("{0} couldn't find any reachable point to move to while performing random wander", name);
        }

        private void OnMoveComplete(bool success)
        {
            // Regardless of whether the move was successful, just move to another destination.
            SetNextMove();
        }

        private void OnDrawGizmos()
        {
            if (debugDrawQuery && debugPoints.Count > 0)
            {
                // Draw all the points that where queried but unreachable
                for (int i = 0; i < debugPoints.Count - 1; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(debugPoints[i], 0.2f);
                }

                // Draw the chosen reachable point
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(debugPoints[debugPoints.Count - 1], 0.2f);

                // Draw line from agent to random destination
                Debug.DrawLine(transform.position, debugPoints[debugPoints.Count - 1], Color.gray);
            }
        }
    }
}
