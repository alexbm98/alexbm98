using UnityEngine;
using Mercuna;

namespace MercunaExamples
{
    // Simple example script that upon start sends agent to destination position
    [RequireComponent(typeof(Mercuna3DNavigation))]
    [RequireComponent(typeof(MercunaObstacle))]
    [AddComponentMenu("Mercuna 3D Navigation/Examples/Navigate To Location")]
    public class NavigateToLocation : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Destination - a position for the agent to move to.")]
        private Vector3 _destination;

        private bool m_bStarted = false;

        // Destination - when set at runtime the agent will immediately switch destination.
        public Vector3 destination
        {
            get
            {
                return _destination;
            }
            set
            {
                _destination = value;
                if (enabled && m_bStarted)
                {
                    GetComponent<Mercuna.Mercuna3DNavigation>().NavigateToLocation(_destination);
                }
            }
        }

        private void Start()
        {
            // Request for your agent to navigate to the destination
            GetComponent<Mercuna3DNavigation>().NavigateToLocation(_destination);
            m_bStarted = true;
        }
    }
}
