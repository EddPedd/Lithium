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
    }

    void Update()
    {
        //Sätt igång "dash" om spelaren klickar på q
        if(Input.GetKeyDown("q") && canDash && currentEnergy>=2)
        {
            isDashing = true;
            //Sätt riktningen på dashen till 
            mouseDirection = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition).normalized;
            mouseDirection.z = 0;
            currentEnergy -= 2;
            dashDuration = 0;
            Debug.Log("Player is dashing in the direction of" + mouseDirection);
        }

        if (isDashing)
        {
            rb.velocity = mouseDirection * dashVelocity;
            dashDuration += Time.deltaTime;
            
            if( dashDuration >= dashMaxDuration)
            {
                isDashing=false;
            }
        }
    }

    

}
