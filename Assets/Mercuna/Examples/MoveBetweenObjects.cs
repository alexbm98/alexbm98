using UnityEngine;
using Mercuna;

namespace MercunaExamples
{
    /* MoveBetweenObjects will move an entity around an array of destinations, either sequentially or randomly */
    [RequireComponent(typeof(Mercuna3DNavigation))]
    [RequireComponent(typeof(MercunaObstacle))]
    [AddComponentMenu("Mercuna 3D Navigation/Examples/Move Between Objects")]
    public class MoveBetweenObjects : MonoBehaviour
    {
        [Tooltip("The destinations the agent should move between")]
        public GameObject[] destinations;

        public enum MoveOrderEnum { Sequential, Random };

        [Tooltip("The order the agent should visit the destinations")]
        public MoveOrderEnum moveOrder;

        // Next destination index to move to when moving sequentially.
        private int m_nextIdx;

        // Use this for initialization
        private void Start()
        {
            m_nextIdx = 0;
            SetNextMove();
        }

        private void SetNextMove()
        {
            GameObject destination;
            if (moveOrder == MoveOrderEnum.Sequential)
            {
                // Check for wrap before accessing instead of after incrementing to cover the
                // case a destination has been removed.
                if (m_nextIdx >= destinations.Length)
                {
                    m_nextIdx = 0;
                }
                destination = destinations[m_nextIdx];
                m_nextIdx++;
                // Move to the selected destination and call OnMoveComplete when the agent gets there.
                GetComponent<Mercuna3DNavigation>().NavigateToObject(destination, this.OnMoveComplete);

                /*
                // Example code to add all destinations in a single Mercuna request
                Vector3[] destinationLocations = new Vector3[destinations.Length];
                for (int i = 0; i < destinations.Length; ++i)
                {
                    destinationLocations[i] = destinations[i].transform.position;
                }
                GetComponent<Mercuna3DNavigation>().NavigateToLocations(destinationLocations, this.OnMoveComplete);
                */

                /*
                // Example code to add destinations with on-the-fly Mercuna requests
                // This code makes a move request, and then sequentially uses NavigateToAdditionalLocation to add any further locations
                GetComponent<Mercuna3DNavigation>().NavigateToLocation(destinations[0].transform.position, this.OnMoveComplete);
                
                for (int i=1; i<destinations.Length; ++i)
                {
                    GetComponent<Mercuna3DNavigation>().NavigateToAdditionalLocation(destinations[i].transform.position);
                }*/

            }
            else // moveOrder == MoveOrderEnum.Random
            {

                destination = destinations[Random.Range(0, destinations.Length)];
                // Move to the selected destination and call OnMoveComplete when the agent gets there.
                GetComponent<Mercuna3DNavigation>().NavigateToObject(destination, this.OnMoveComplete);
            }
        }

        private void OnMoveComplete(bool success)
        {
            // Regardless of whether the move was successful, just move to the next destination.
            SetNextMove();
        }
    }
}