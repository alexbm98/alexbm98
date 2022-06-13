using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CondorController : MonoBehaviour
{
    public GameObject destination;

    void Start()
    {
        // Request for your agent to navigate to the destination
        GetComponent<Mercuna.Mercuna3DNavigation>().NavigateToObject(destination);
    }


    // Update is called once per frame
    void Update()
    {
        GetComponent<Mercuna.Mercuna3DNavigation>().NavigateToObject(destination);
    }
}
