using UnityEngine;
using Mercuna;

namespace MercunaExamples
{
    // Simple example script that upon start sends agent to destination object
    [RequireComponent(typeof(Mercuna3DNavigation))]
    [RequireComponent(typeof(MercunaObstacle))]
    [AddComponentMenu("Mercuna 3D Navigation/Examples/Navigate To Object")]
    public class NavigateToObject : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Destination - an object the agent will move to.")]
        private GameObject _destination;

        private bool m_bStarted = false;

        // Destination object - when set at runtime the agent will switch destination.
        public GameObject destination
        {
            get
            {
                return _destination;
            }
            set
            {
                _destination = value;
                if (m_bStarted)
                {
                    GetComponent<Mercuna3DNavigation>().NavigateToObject(_destination);
                }
            }
        }

        private void Start()
        {
            // Request for your agent to navigate to the destination
            GetComponent<Mercuna.Mercuna3DNavigation>().NavigateToObject(_destination);
            m_bStarted = true;
        }
    }
}
