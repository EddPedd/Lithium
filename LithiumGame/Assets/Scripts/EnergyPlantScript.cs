using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPlantScript : MonoBehaviour
{
    public PlayerMovement playerM;
    public GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerM = player.GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collider )
            {
                if(collider.name == player.name)
                {
                    playerM.GainEnergy(10);
                    Debug.Log(collider.name + " triggered " + gameObject.name);
                    Destroy(gameObject);    
                }
                else
                {
                    Debug.Log(collider.name + " collided with something that isn't the Player");
                }
            }
}
