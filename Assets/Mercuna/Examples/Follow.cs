using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mercuna;

namespace MercunaExamples
{
    /* Follow a target object */
    [RequireComponent(typeof(Mercuna3DNavigation))]
    [RequireComponent(typeof(MercunaObstacle))]
    [AddComponentMenu("Mercuna 3D Navigation/Examples/Follow")]
    public class Follow : MonoBehaviour
    {
        [Tooltip("Target - the agent will follow this target.")]
        public GameObject target;

        [Tooltip("Approximate distance to stay from the target.")]
        public float distance = 5.0f;

        // Use this for initialization
        private void Start()
        {
            // Check that a target has been set
            if (target)
            {
                // Request for your agent to follow this target
                GetComponent<Mercuna3DNavigation>().Track(target, null, distance);
            }
        }
    }
}
