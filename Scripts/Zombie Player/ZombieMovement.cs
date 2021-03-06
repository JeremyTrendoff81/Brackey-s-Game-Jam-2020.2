using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieMovement : MonoBehaviour
{

    public GameObject player;

    public Pathfinding.AIDestinationSetter destinationSetter;

    public Rigidbody2D rigidbody;
    public Animator animator;
    public int directionFacing;

    public float health;

    public GameObject lifeDrop;



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        destinationSetter.target = player.transform;
        health = 3f; 
    }

    void FixedUpdate()
    {
        determineDirectionFacing();
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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( collision.tag == "Bullet")
        {
            health -= player.GetComponent<PlayerController>().activeGun.GetComponent<GunController>().Damage;
            collision.GetComponent<bulletBehaviour>().canCheck = false;
            Destroy(collision.gameObject);
            if (health <= 0)
            {
                dropHealth();
                Destroy(this.gameObject);
            }
        }
    }

    private void dropHealth()
    {
        Instantiate(lifeDrop, new Vector3(transform.position.x, transform.position.y, 2), Quaternion.identity);
    }
}