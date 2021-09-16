using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerController : MonoBehaviour
{

    public GameObject player;

    public Pathfinding.AIDestinationSetter destinationSetter;

    public Rigidbody2D rigidbody;

    public Animator animator;

    public int directionFacing;
    public float health;
    public GameObject berserkerTarget;
    public GameObject parent;

    private bool dontSet;
    private float distance;
    private bool inRange;


    // Start is called before the first frame update
    void Start()
    {
        dontSet = false;
        player = GameObject.FindGameObjectWithTag("Player");
        health = 50f;
        inRange = false; 

        //setBerserkerTarget();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // determineDirectionFacing();

        seeIfEnemyIsInRange(); 

        if (dontSet == false)
        {
            isBaserkerAtTarget();
        }
        
    }

    private void determineDirectionFacing()
    {
        Vector3 playerPosition = player.transform.position;
        //print(mousePosition);
        // These variables repersent the world positions of the corners of the screen
        Vector3 upperLeftCorner = new Vector3(rigidbody.transform.position.x - 50, rigidbody.transform.position.y + 50);
        Vector3 upperRightCorner = new Vector3(rigidbody.transform.position.x + 50, rigidbody.transform.position.y + 50);
        Vector3 lowerLeftCorner = new Vector3(rigidbody.transform.position.x - 50, rigidbody.transform.position.y - 50);
        Vector3 lowerRightCorner = new Vector3(rigidbody.transform.position.x + 50, rigidbody.transform.position.y - 50);



        // This if else stamtment is used to determine what directional coderent the player wants to attack in
        if (calculateIfMouseIsInTriangle(playerPosition, rigidbody.transform.position, upperLeftCorner, upperRightCorner)) // Does the player want to attack to the north?
        {
            animator.SetInteger("Direction", 1);
            directionFacing = 1;
            //print("mouse found north of the player"); 
        }
        else if (calculateIfMouseIsInTriangle(playerPosition, rigidbody.transform.position, upperRightCorner, lowerRightCorner)) // Does the player want to attack to the east?
        {
            animator.SetInteger("Direction", 3);
            directionFacing = 3;
            //print("mouse found east of the player");
        }
        else if (calculateIfMouseIsInTriangle(playerPosition, rigidbody.transform.position, lowerRightCorner, lowerLeftCorner)) // Does the player want to attack to the south?
        {
            animator.SetInteger("Direction", 2);
            directionFacing = 2;
            //print("mouse found south of the player");
        }
        else if (calculateIfMouseIsInTriangle(playerPosition, rigidbody.transform.position, lowerLeftCorner, upperLeftCorner)) // Does the player want to attack to the west?
        {
            animator.SetInteger("Direction", 4);
            directionFacing = 4;
            //print("mouse found west of the player");
        }
    }

    private bool calculateIfMouseIsInTriangle(Vector3 p, Vector2 a, Vector2 b, Vector2 c)
    {
        //Equations by Sebastian Lague
        //https://www.youtube.com/watch?v=HYAgJN3x4GA
        float w1 = (a.x * (c.y - a.y) + (p.y - a.y) * (c.x - a.x) - p.x * (c.y - a.y)) / ((b.y - a.y) * (c.x - a.x) - (b.x - a.x) * (c.y - a.y));
        float w2 = (p.y - a.y - w1 * (b.y - a.y)) / (c.y - a.y);

        if (w1 >= 0 && w2 >= 0 && (w1 + w2) <= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void setBerserkerTarget()
    {  
        berserkerTarget.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        destinationSetter.target = berserkerTarget.transform;
    }



    private void isBaserkerAtTarget()
    {
        if(transform.position.x + 1 > berserkerTarget.transform.position.x && transform.position.x - 1 < berserkerTarget.transform.position.x 
            && transform.position.y + 1 > berserkerTarget.transform.position.y && transform.position.y - 1 < berserkerTarget.transform.position.y)
        {
            StartCoroutine(coolTheFuck());
        }
    }

    private void seeIfEnemyIsInRange()
    {
        if (destinationSetter.target == null)
        {
            distance = Mathf.Sqrt(Mathf.Pow(player.transform.position.x - transform.position.x, 2) + Mathf.Pow(player.transform.position.y - transform.position.y, 2));
            if (distance < 8.5f)
            {
                inRange = true;
            }
            Debug.DrawLine(transform.position, player.transform.position);
        }
    }

    
    private IEnumerator coolTheFuck()
    {
        dontSet = true;
        yield return new WaitForSeconds(1f);
        if (inRange == true) 
        { 
            setBerserkerTarget(); 
        }
        dontSet = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet")
        {
            health -= player.GetComponent<PlayerController>().activeGun.GetComponent<GunController>().Damage;
            collision.GetComponent<bulletBehaviour>().canCheck = false;
            Destroy(collision.gameObject);
            if (health <= 0)
            {
                Destroy(parent);
            }
        }
    }
}
