using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mercuna;

namespace MercunaExamples
{
    public class ScoredPoint
    {
        public ScoredPoint(Vector3 _position, float _score)
        {
            position = _position;
            score = _score;
        }

        public Vector3 position;
        public float score;
    };

    public class WaitForSecondsOrEvent : CustomYieldInstruction
    {
        public bool eventHappened;
        private float endTime;

        public override bool keepWaiting
        {
            get
            {
                return !eventHappened && Time.time < endTime;
            }
        }

        public WaitForSecondsOrEvent()
        {
            eventHappened = false;
            endTime = 0;
        }

        public WaitForSecondsOrEvent(float delay)
        {
            ResetDelay(delay);
        }

        public void ResetDelay(float delay)
        {
            eventHappened = false;
            endTime = Time.time + delay;
        }
    }

    /* Circle around a target object */
    [RequireComponent(typeof(Mercuna3DNavigation))]
    [RequireComponent(typeof(MercunaObstacle))]
    [AddComponentMenu("Mercuna 3D Navigation/Examples/Circle Around")]
    public class CircleAround : MonoBehaviour
    {
        [Tooltip("Target - the agent will circle around this target.")]
        public GameObject target;

        [Tooltip("Approximate distance to stay from the target.")]
        public float distance;

        public bool debugDrawQuery;

        // Cached reference to the navigation component.
        private Mercuna3DNavigation m_navComponent;
        private Rigidbody m_rigidBody;

        // Remember last set of points for debug draw
        private List<ScoredPoint> m_scoredPoints = new List<ScoredPoint>();

        private WaitForSecondsOrEvent m_yieldWait = new WaitForSecondsOrEvent();

        void Start()
        {
            m_navComponent = GetComponent<Mercuna3DNavigation>();
            m_rigidBody = GetComponent<Rigidbody>();

            StartCoroutine(UpdateDestination());
        }

        IEnumerator UpdateDestination()
        {
            MercunaNavOctree navOctree = MercunaNavOctree.GetInstance();

            while (true)
            {
                // Find our movement direction, or use our forward direction if we're not moving.
                Vector3 movementDir = m_rigidBody.velocity;
                if (movementDir.sqrMagnitude < float.Epsilon)
                {
                    movementDir = transform.forward;
                }
                else
                {
                    movementDir.Normalize();
                }

                // Generate a ring of points around the target
                List<Vector3> points = PointGenerator.CreateRing(target.transform.position, 20, 1, distance, distance, 1, 0.0f, 0.0f);

                // Check which points are reachable by the agent
                List<Vector3> reachablePoints = navOctree.IsReachable(transform.position, points, m_navComponent.radius);

                // Check which points have a direct line of sight to the target
                List<Vector3> lineofSightPoints = new List<Vector3>();
                foreach (var point in reachablePoints)
                {
                    if (navOctree.Raycast(point, target.transform.position, m_navComponent.radius))
                    {
                        lineofSightPoints.Add(point);
                    }
                }

                // Check that some points pass the test
                if (lineofSightPoints.Count == 0)
                {
                    if (reachablePoints.Count == 0)
                    {
                        // No reachable points.  Fail, the agent will stop moving.
                        Debug.LogErrorFormat("{0} couldn't find any reachable point to move to while performing circle around {1}", name, target.name);
                        break;
                    }
                    else
                    {
                        // Fallback to moving to a reachable point.
                        lineofSightPoints = reachablePoints;
                    }
                }

                // Score points
                m_scoredPoints = new List<ScoredPoint>();
                float radius = m_navComponent.radius;
                foreach (var point in lineofSightPoints)
                {
                    // Get the vector from our current position to the test point.
                    Vector3 toTestPoint = point - transform.position;
                    float distance = toTestPoint.magnitude;
                    float dotProduct = Vector3.Dot(toTestPoint.normalized, transform.forward);

                    // Prefer points in front and closer, but not too close.
                    float score = distance > 6.0f * radius ? (dotProduct + 2.0f) / (distance + 1.0f) : 0.0f;

                    m_scoredPoints.Add(new ScoredPoint(point, score));
                }

                // Sort points by score
                m_scoredPoints.Sort((x, y) => y.score.CompareTo(x.score));

                // Go a third of the way to point with best score, then replan
                m_navComponent.NavigateToLocation(m_scoredPoints[0].position, OnMoveComplete, (m_scoredPoints[0].position - transform.position).magnitude * 0.67f);

                // Replan when we get some way towards the destination, or every 0.25s to take account of the movement of the target.
                m_yieldWait.ResetDelay(0.25f);
                yield return m_yieldWait;
            }
        }

        private void OnDrawGizmos()
        {
            if (debugDrawQuery && m_scoredPoints.Count > 0)
            {
                // Draw all the points that where generated but not used
                for (int i = 1; i < m_scoredPoints.Count; i++)
                {
                    Gizmos.color = new Color(1.0f, 1.0f - ((float)i / (float)m_scoredPoints.Count), 0.0f);
                    Gizmos.DrawSphere(m_scoredPoints[i].position, 0.2f);
                }

                // Draw the chosen point
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(m_scoredPoints[0].position, 0.2f);

                // Draw line from agent to destination
                Debug.DrawLine(transform.position, m_scoredPoints[0].position, Color.gray);
            }
        }

        private void OnMoveComplete(bool success)
        {
            // Regardless of whether the move was successful, just move to another destination.
            m_yieldWait.eventHappened = true;
        }
    }
}
