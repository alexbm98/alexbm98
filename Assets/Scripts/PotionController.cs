using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDestroy()
    {
        if (this.gameObject.tag == "Health")
        {
            PlayerManager.Instance.currentHealth += 25.0f;
        }

        if (this.gameObject.tag == "Stamina")
        {
            PlayerManager.Instance.currentStamina += 25.0f;
        }
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(this.gameObject);
        }
    }
}
