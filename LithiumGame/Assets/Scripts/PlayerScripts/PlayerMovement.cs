    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Allmänna variabler
    public Rigidbody2D rb;
    public int maxEnergy;
    public int currentEnergy;
    private bool gravityBool = true;
    public float gravityInt = 10f;

    //Vector för Musens position på skärmen
    private Vector3 mouseDirection;

    //Dash-varabler
    public bool canDash = true;
    public bool isDashing = false;
    public float dashVelocity;
    private float dashDuration;
    public float dashMaxDuration;
    public Vector3 dashDirection;

    //WallClimb variabler
    public bool isWallClimbing = false;
    public float wallClimbDuration;
    public float wallClimbMaxDuration;

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
        //Gravitation
        if(gravityBool==true )
        {
            rb.velocity = Vector2.down * gravityInt;
        }

        //Sätt igång "dash" om spelaren klickar på q
        if(Input.GetKeyDown("q") && canDash && currentEnergy>=2)
        {
            mouseDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            mouseDirection.z = 0;

            StartDashing(mouseDirection, 2);
        }

        //Ett stadeie av "dashing" med en bestämd velocitet och riktning
        if (isDashing)
        {
            //Egenskaperna av dashen
            rb.velocity = dashDirection.normalized * dashVelocity;
            dashDuration += Time.deltaTime;
            
            //avbryter dashing om den varat tiden ut
            //OBS! Lägg till att den avbryts vid kontakt med marken
            if( dashDuration >= dashMaxDuration)
            {
                CancelDashing();
            }
        }

        //Ett stadie av Wallclimbing
        if(isWallClimbing)
        {
            rb.velocity = new Vector3(0,0,0);
        }

        //Tillfällig testkod för att kunna ladda om med energi
        if (Input.GetKeyDown("p"))
        {
            GainEnergy(maxEnergy);
        }
    }

    //Metod för att sätta igång dash
    public void StartDashing(Vector3 direction, int cost)
    {
        isWallClimbing = false;
        isDashing = true;
        canDash = false;
        //Se till så att riktningen på dashen är mot musen, kostar energi och varar lika länge som den ska 
        dashDirection = direction;
        currentEnergy -= cost;
        dashDuration = 0;
        Debug.Log("Player is dashing in the direction of" + direction);
        Debug.Log("current energy = " + currentEnergy);

    }

    //Metod för att avbryta dash och för att göra saker som sker när dashen avbryts
    public void CancelDashing()
    {
        isDashing = false;
        canDash = true;
        rb.velocity = Vector2.zero;
    }

    //Kod för Wallclimb
    //Kod från chatGBT för att räkna ut vilken sida av en plattform som spelaren colliderar med för att
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Se kollision med vägg och vilken sida av väggen, är det en sida så vill vi wallclimba, om annars gör vi ingenting
        if(collision.gameObject.tag == "Platform")
        {
            // Get the contact points of the collision
            ContactPoint2D[] contacts = collision.contacts;

            // Loop through each contact point
            foreach (ContactPoint2D contact in contacts)
            {
                // Determine the collision side based on the normal vector of the contact point
                Vector2 normal = contact.normal;
                if(normal == Vector2.up || normal == Vector2.down)
                {
                    return;
                }

                if (normal == Vector2.left)
                {
                    // Collision with the left side of a platform
                    WallClimb(isDashing);
                    Debug.Log(gameObject.name + "Collided with left side of a " + collision.collider.tag);
                }
                else if (normal == Vector2.right)
                {
                    // Collision with the right side
                    WallClimb(isDashing);
                    Debug.Log(gameObject.name + "Collided with right side of a " + collision.collider.tag);
                }
            }

        }
    }


    private void OnCollisionExit2D (Collision2D collision)
    {
        if(collision.collider.tag == "Platform")
        {
            CancelWallClimb();
        }
    }

    private void WallClimb(bool dashingIs)
    {
        if (dashingIs)
        {
            StartDashing(Vector3.up, 0);
        }
        else
        {
            isWallClimbing = true;
            gravityBool = false;

        }
    }

    private void CancelWallClimb()
    {
        isWallClimbing=false;
        gravityBool = true;
    }

}
