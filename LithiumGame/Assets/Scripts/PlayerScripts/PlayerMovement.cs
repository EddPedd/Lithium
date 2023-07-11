    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Allmänna variabler
    public Rigidbody2D rb;
    public CapsuleCollider2D cc;
    public int maxEnergy;
    public int currentEnergy;
    public bool isGrounded = true;

    //Vector för Musens position på skärmen
    private Vector3 mouseDirection;

    //Dash-varabler
    public bool canDash = true;
    public bool isDashing = false;
    public float dashVelocity;
    private float dashDuration;
    public float dashMaxDuration;
    public Vector3 dashDirection;
    private bool checkingFrames;
    public int framesToCheck;
    private int checkedFrames;

    //WallClimb variabler
    public bool isWallClimbing = false;
    private float wallClimbDuration;
    public float wallClimbMaxDuration;
    public float someFloat;

    void Start()
    {
        currentEnergy = maxEnergy;
    }


    void Update()
    {
        if (Input.GetKeyDown("a") && isGrounded)
        {
            
        }
        
        
        //Sätt igång "dash" om spelaren klickar på q
        if(Input.GetKeyDown("q") && currentEnergy>=2)
        {
            //Försök till att implementera stödframes att kolla inför dashen
            checkingFrames = true;
        }

        if (checkingFrames)
        {
            //Räkna antalet frames som letats
            checkedFrames++;
            Debug.Log("Checked if isDashing " + checkedFrames + " frames");

            //Dasha mot musen om du klickat på "q" de senaste checkade framesen
            if (canDash)
            {
                Debug.Log("Dashing as a result of pressing q");
                mouseDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                mouseDirection.z = 0;

                StartDashing(mouseDirection, 2);
                checkedFrames = 0;
            }

            //Om lika många frames har kollats som skas så slutar vi leta efter en dash och
            //återställer antalet räknade checkade frames
            if (checkedFrames >= framesToCheck)
            {
                checkingFrames = false;
                checkedFrames = 0;
            }
        }


        //Ett stadeie av "dashing" med en bestämd velocitet och riktning
        if (isDashing)
        {
            //Egenskaperna av dashen
            //Lägg till velocity och bestäm en helt ny riktning

            rb.velocity = (rb.velocity + (Vector2)dashDirection).normalized * dashVelocity;
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
            wallClimbDuration += Time.deltaTime;
            if(wallClimbDuration >= wallClimbMaxDuration)
            {
                CancelWallClimb();
            }
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
        rb.gravityScale = 1f;
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
        Vector2 newVelocity = rb.velocity / 2;
        rb.velocity = newVelocity;

        //Se om spelaren är nära en väg för att sätta igång wallclimb
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, someFloat);
        foreach (Collider2D platforms in colliders)
        {
            Debug.Log("Checking for platforms to wallclimb onto after dash");
            if(platforms.tag == "Platform")
            {
                WallClimb(false);
            }
        }
    }

    //Gizmo för ovan beskrivna PhysicsCircle 
    private void OnDrawGizmos()
    {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, someFloat);
    }

    //Metod för att återställa energin för rörelseförmågor
    public void GainEnergy(int amount)
    {
        currentEnergy += amount;
        if (currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy;
        }
        Debug.Log("CurrentEnergy =" + currentEnergy);
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
                if(normal == Vector2.down)
                {
                    return;
                }

                if(normal == Vector2.up)
                {
                    isGrounded = true;
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

    //Allmänna 

    //Metod för att antingen sätta igång wallclimb eller göra en extra dash vid kontakt med väggen
    private void WallClimb(bool dashingIs)
    {
        //Om man tar kontakt med väggen under "dash" så får man en ytterligare dash
        if (dashingIs)
        {
            StartDashing(Vector3.up, 0);
        }
        //Om inte så får man vanlig Wallclimb
        else
        {
            wallClimbDuration = 0;
            isWallClimbing = true;
            rb.gravityScale = 0;
        }
    }

    //Metod som drar igång då man slutar wallclimba
    private void CancelWallClimb()
    {
        isWallClimbing=false;
        rb.gravityScale = 1;
    }

}
