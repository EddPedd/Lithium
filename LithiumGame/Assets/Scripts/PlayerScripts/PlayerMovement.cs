    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Allm�nna variabler
    public Rigidbody2D rb;
    public CapsuleCollider2D cc;
    public int maxEnergy;
    public int currentEnergy;

    //Vector f�r Musens position p� sk�rmen
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

    //Metod f�r att �terst�lla energin f�r r�relsef�rm�gor
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
        //S�tt ig�ng "dash" om spelaren klickar p� q
        if(Input.GetKeyDown("q") && currentEnergy>=2)
        {
            //F�rs�k till att implementera st�dframes att kolla inf�r dashen
            checkingFrames = true;
        }

        if (checkingFrames)
        {
            //R�kna antalet frames som letats
            checkedFrames++;
            Debug.Log("Checked if isDashing " + checkedFrames + " frames");

            //Dasha mot musen om du klickat p� "q" de senaste checkade framesen
            if (canDash)
            {
                Debug.Log("Dashing as a result of pressing q");
                mouseDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                mouseDirection.z = 0;

                StartDashing(mouseDirection, 2);
                checkedFrames = 0;
            }

            //Om lika m�nga frames har kollats som skas s� slutar vi leta efter en dash och
            //�terst�ller antalet r�knade checkade frames
            if (checkedFrames >= framesToCheck)
            {
                checkingFrames = false;
                checkedFrames = 0;
            }
        }


        //Ett stadeie av "dashing" med en best�md velocitet och riktning
        if (isDashing)
        {
            //Egenskaperna av dashen
            rb.velocity = dashDirection.normalized * dashVelocity;
            dashDuration += Time.deltaTime;
            
            //avbryter dashing om den varat tiden ut
            //OBS! L�gg till att den avbryts vid kontakt med marken
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

        //Tillf�llig testkod f�r att kunna ladda om med energi
        if (Input.GetKeyDown("p"))
        {
            GainEnergy(maxEnergy);
        }
    }

    //Metod f�r att s�tta ig�ng dash
    public void StartDashing(Vector3 direction, int cost)
    {
        isWallClimbing = false;
        isDashing = true;
        canDash = false;
        //Se till s� att riktningen p� dashen �r mot musen, kostar energi och varar lika l�nge som den ska 
        dashDirection = direction;
        currentEnergy -= cost;
        dashDuration = 0;
        Debug.Log("Player is dashing in the direction of" + direction);
        Debug.Log("current energy = " + currentEnergy);

    }

    //Metod f�r att avbryta dash och f�r att g�ra saker som sker n�r dashen avbryts
    public void CancelDashing()
    {
        isDashing = false;
        canDash = true;
        rb.velocity = Vector2.zero;

        //Se om spelaren �r n�ra en v�g f�r att s�tta ig�ng wallclimb
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

    //Gizmo f�r ovan beskrivna PhysicsCircle 
    private void OnDrawGizmos()
    {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, someFloat);
    }

    //Kod f�r Wallclimb
    //Kod fr�n chatGBT f�r att r�kna ut vilken sida av en plattform som spelaren colliderar med f�r att
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Se kollision med v�gg och vilken sida av v�ggen, �r det en sida s� vill vi wallclimba, om annars g�r vi ingenting
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

    //Metod f�r att antingen s�tta ig�ng wallclimb eller g�ra en extra dash vid kontakt med v�ggen
    private void WallClimb(bool dashingIs)
    {
        //Om man tar kontakt med v�ggen under "dash" s� f�r man en ytterligare dash
        if (dashingIs)
        {
            StartDashing(Vector3.up, 0);
        }
        //Om inte s� f�r man vanlig Wallclimb
        else
        {
            wallClimbDuration = 0;
            isWallClimbing = true;
            rb.gravityScale = 0;
        }
    }

    //Metod som drar ig�ng d� man slutar wallclimba
    private void CancelWallClimb()
    {
        isWallClimbing=false;
        rb.gravityScale = 1;
    }

}
