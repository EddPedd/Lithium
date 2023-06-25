using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPlantScript : MonoBehaviour
{
    public PlayerMovement playerM;
    
    // Start is called before the first frame update
    void Start()
    {
        playerM = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collider )
    {
        if(collider.tag == "Player")
        {
            playerM.GainEnergy(10);
            Debug.Log(collider.name + " triggered " + gameObject.name);
        }
        else
        {
            Debug.Log(collider.name + " collided with something that isn't the Player");
        }
    }
}
