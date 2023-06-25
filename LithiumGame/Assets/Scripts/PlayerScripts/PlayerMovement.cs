    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public int maxEnergy;
    public int currentEnergy;

    private Vector3 mouseDirection;

    public bool canDash = true;
    public bool isDashing = false;
    public float dashVelocity;
    private float dashDuration;
    public float dashMaxDuration;

    void Start()
    {
        currentEnergy = maxEnergy;
    }

    //Metod för att återställa energin för rörelseförmågor
    public void GainEnergy (int amount)
    {
        currentEnergy += amount;
        if(currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy;
        }
        Debug.Log("CurrentEnergy =" +  currentEnergy);
    }

    void Update()
    {
        //Sätt igång "dash" om spelaren klickar på q
        if(Input.GetKeyDown("q") && canDash && currentEnergy>=2)
        {
            isDashing = true;
            canDash = false;
            //Se till så att riktningen på dashen är mot musen, kostar energi och varar lika länge som den ska 
            mouseDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            mouseDirection.z = 0;
            currentEnergy -= 2;
            dashDuration = 0;
            Debug.Log("Player is dashing in the direction of" + mouseDirection);
            Debug.Log("current energy = " + currentEnergy);
        }

        //Ett stadeie av "dashing" med en bestämd velocitet och riktning
        if (isDashing)
        {
            rb.velocity = mouseDirection.normalized * dashVelocity;
            dashDuration += Time.deltaTime;
            
            //avbryter dashing om den varat tiden ut
            //OBS! Lägg till att den avbryts vid kontakt med marken
            if( dashDuration >= dashMaxDuration)
            {
                isDashing=false;
                canDash=true;
            }
        }

        if (Input.GetKeyDown("p"))
        {
            GainEnergy(maxEnergy);
        }



    }

    

}
