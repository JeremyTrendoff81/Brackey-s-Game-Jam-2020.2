using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/* 
 * Author: Benjamin Kerr
 * Date: 7/23/2020 --> 
 * This class is used to control the player's movement, what type of input device is being, interaction with other gameobjects and attacking inputs and management 
*/

public class PlayerController : MonoBehaviour
{
    // Any varable that will need to be stored will have a (STORED) beside it //



    //These are varables related to the player's unity components
    private Rigidbody2D rigidbody; // This is the rigidbody attachted to the player
    private Animator animator; // This is the player's animator which will be used to control sprite animations;

    // These variables are related to the camera's position in the world
    private Vector3 upperLeftScreen = new Vector3(0, Screen.height);
    private Vector3 upperRightScreen = new Vector3(Screen.width, Screen.height);
    private Vector3 lowerLeftScreen = new Vector3(0, 0);
    private Vector3 lowerRightScreen = new Vector3(Screen.width, 0);


    // These variables are related to the player's movement
    private float acceleration; // The acceleration varaible will determin how fast the player can get up to speed
    [HideInInspector] public float friction; // The friction variable will work against the player in the opposite direction
    private Vector3 frictionVector; // The friction vector is used to slow down and apply a force to the player in the opposite direction
    private Vector3 movementVector; // The movement vector is used to keep track of the player currently movement vector for that frame
    private float xAxsisMaxSpeed; // This varibale will limit how fast the player can travel on the horizontal axsis
    private float yAxsisMaxSpeed; // This variable will limit how fast the player can traven on the vertiical axsis

    // These variables are related to movement;
    public int directionFacing; // This intager will indicate what caridinal direction the player is facing. 1 --> North | 2 --> South | 3 --> East | 4 --> West
    public bool canMove; // This bool is used to indicate wheather or not the player is able to move (may be triggered during cutscenes
    public bool isControllerOn; // This bool is used to indicate whether or not a controller is being used
    private KeyCode northKey; // The north key is the key on the keyboard that can be used to move north on the screen  (STORED)
    private KeyCode southKey; // The south key is the key on the keyboard that can be used to move south on the screen  (STORED)
    private KeyCode westKey; // The west key is the key on the keyboard that can be used to move west on the screen  (STORED)
    private KeyCode eastKey; // The east key is the key on the keyboard that can be used to move east on the screen  (STORED)

    // These variables are related to the attacking 
    private KeyCode attackKey; // This is the key that the player will use to attack things in the game  (STORED)
    public GameObject activeGun; // This will be the players current active gun they are using
    private bool isShooting;


    public int lives; // The ammount the 
    public Image[] livesImages;


    public GameObject playerSprite;

    public GameObject camera;
    private float distanceBetween;
    private bool isMoving;

    public GameObject normalUI;
    public GameObject deathScreen;
    public GameObject optionScreen;

    public AudioSource walking;

    


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        deathScreen.SetActive(false);
        optionScreen.SetActive(false);

        distanceBetween = 2;
        // Any varable that will need to be stored will have a (STORED) beside it
        // These variables will have to be loaded from a text file and set in the future

        // Setup friction and physics variables
        canMove = true; // We are able to move at the begining so canMove is set to true
        northKey = KeyCode.W; // North key being setup  (STORED)
        southKey = KeyCode.S; // South key being setup  (STORED)   
        westKey = KeyCode.A; // West key being setup  (STORED)
        eastKey = KeyCode.D; // East key being setup  (STORED)

        // Setup friction and physics variables
        acceleration = 4f; // accerleration value 
        friction = 1f; // friction value
        xAxsisMaxSpeed = 10; // horizontal max speed
        yAxsisMaxSpeed = 10; // vertical max speed

        // Setup components
        rigidbody = GetComponent<Rigidbody2D>(); // Getting the players rigidbody
        //animator = GetComponent<Animator>();


        // Setup attack varables
        attackKey = KeyCode.Mouse0; // attack key being setup  (STORED)



        lives = 3; // Set it up to we have three lives

        animator = playerSprite.GetComponent<Animator>();

        animator.SetInteger("Direction", 2);

        directionFacing = 2;

        controllerGunLocation();

        isShooting = false;

    }

    // FixedUpdate is called every frame on a fixed setting after physics calculations are complete
    void FixedUpdate()
    {
        if (canMove == true) // If the player is able to move (not in an interaction or in a cutscene
        {
            acceleratePlayer(); // This funnction will move the player forward in a direction
            applyFriction(); // This function will apply a friction to the player's movement vector slowing them down in the opposite direction
            rigidbody.transform.Translate(movementVector * Time.deltaTime); // Then we apply the movement vector to the gameobjects rigidbody2D
        }
    }

    private void Update()
    {
        shoot();
        if (isShooting == true)
        {
            determinMovementVectorOnAttack();
        }
        controllerGunLocation();
    }

    // The movePlayer function will have the ability to move the player with key or button presses
    private void acceleratePlayer()
    {
        camera.GetComponent<CameraController>().target = new Vector2(rigidbody.transform.position.x, rigidbody.transform.position.y);
        movementVector.x = (float)System.Math.Round(movementVector.x, 1); // Round the charicters x position to one decimal place
        movementVector.y = (float)System.Math.Round(movementVector.y, 1); // Round the charicters y position to one decimal place

        if (Input.GetKey(northKey)) // See if the key being pressed is the north key
        {
            camera.GetComponent<CameraController>().target = new Vector2(rigidbody.transform.position.x, rigidbody.transform.position.y + directionFacing);
            animator.SetInteger("Direction", 1);
            directionFacing = 1;
            // If it is pressed we move north
            movementVector.y += acceleration; // Accelerate the player
            // This is used to make sure the player does not move past a certain speed
            if (movementVector.y >= yAxsisMaxSpeed) // If the player is above or currently at the max speed verticly
            {
                movementVector.y = yAxsisMaxSpeed; // Set the players speed to the max speed
            }
        }
        else if (Input.GetKey(southKey)) // See if the key being pressed is the south key
        {
            camera.GetComponent<CameraController>().target = new Vector2(rigidbody.transform.position.x, rigidbody.transform.position.y - directionFacing);
            animator.SetInteger("Direction", 2);
            directionFacing = 2;
            // If it is pressed we move south
            movementVector.y -= acceleration; // Accelerate the player
            // This is used to make sure the player does not move past a certain speed
            if (Mathf.Abs(movementVector.y) >= yAxsisMaxSpeed) // If the player is above or currently at the max speed verticly
            {
                movementVector.y = yAxsisMaxSpeed * -1; // Set the players speed to the max speed
            }
        }




        if (Input.GetKey(westKey)) // See if the key being pressed is the west key
        {
            //activeGun.transform.localScale = new Vector3(0, 0, -1);
            camera.GetComponent<CameraController>().target = new Vector2(rigidbody.transform.position.x - directionFacing, rigidbody.transform.position.y);
            animator.SetInteger("Direction", 4);
            directionFacing = 4;
            // If it is pressed we move west
            movementVector.x -= acceleration; // Accerlerate the player
            // This is used to make sure the player does not move past a certain speed
            if (Mathf.Abs(movementVector.x) >= xAxsisMaxSpeed) // If the player is above or currently at the max speed horizontally
            {
                movementVector.x = xAxsisMaxSpeed * -1; // Set the players speed to the max speed
            }
        }
        else if (Input.GetKey(eastKey)) // See if the key being pressed is the east key
        {
            camera.GetComponent<CameraController>().target = new Vector2(rigidbody.transform.position.x + directionFacing, rigidbody.transform.position.y);
            //activeGun.transform.localScale = new Vector3(0, 0, 1);
            animator.SetInteger("Direction", 3);
            directionFacing = 3;
            // If it is pressed we move east
            movementVector.x += acceleration; // Accelerate the player
            // This is used to make sure the player does not move past a certain speed
            if (movementVector.x >= xAxsisMaxSpeed) // If the player is above or currently at the max speed horizontally
            {
                movementVector.x = xAxsisMaxSpeed; // Set the players speed to the max speed
            }
        }
    }

    // The applyFriction function will be responsible for applying a force to the player in the opposite direction when they are moving
    private void applyFriction()
    {
        // First we want to reset our friction values
        frictionVector.y = 0; // reset vertical friction 
        frictionVector.x = 0; // reset horizontal friction vector

        // The friction as to be applied in the oposite direction
        // The following else if statment is used to correctly setup the friction value to meet the conditiona set above
        if (movementVector.y > 0) // If the vertical movement vector is above 0 
        {
            frictionVector.y = friction; // The friction vector will be positive
        }
        else if (movementVector.y < 0) // If the vertical movement vector is below 0
        {
            frictionVector.y = friction * -1; // The friction vector will be negative
        }


        // The friction as to be applied in the oposite direction
        // The following else if statment is used to correctly setup the friction value to meet the conditiona set above
        if (movementVector.x > 0) // If the horizontal movement vector is above 0
        {
            frictionVector.x = friction; // The friction vector will be positive
        }
        else if (movementVector.x < 0) // If the horizontal movement vector is below 0
        {
            frictionVector.x = friction * -1; // The friction vector will be negative
        }

        // Finally we want to apply the friction vector to the movement vector if nessasary
        if (frictionVector.y != 0 || frictionVector.x != 0) // Check to see if either the horizontal or vertical friction vector values are above 0
        {
            movementVector = new Vector3(movementVector.x - frictionVector.x, movementVector.y - frictionVector.y); // Apply the friction vector to the movement vector
            animator.SetBool("IsRunning", true);
            walking.enabled = true;
        }
        else
        {
            animator.SetBool("IsRunning", false);
            walking.enabled = false;
        }
    }

    private void shoot()
    {
        if (Input.GetKey(attackKey))
        {
            if (activeGun.GetComponent<GunController>().canShoot == true)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 slope;
                float mx = mousePosition.x - transform.position.x;
                float my = mousePosition.y - transform.position.y;
                float angle = Mathf.Atan(my / mx);
                
                if (mousePosition.x > transform.position.x)
                {
                    angle = (180 / Mathf.PI) * angle - 90;
                } 

                else if (mousePosition.x < transform.position.x)
                {
                    angle = (180 / Mathf.PI) * angle + 90;
                }
                
                slope = (Vector2.ClampMagnitude(mousePosition - activeGun.transform.position, 1)).normalized * activeGun.GetComponent<GunController>().bulletSpeed;
                activeGun.GetComponent<GunController>().shoot(slope, angle);
                isShooting = true;
            }
        }
        else
        {
            isShooting = false;
        }
    }



    private void determinMovementVectorOnAttack()
    {

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // This is the mouse's world position
        //print(mousePosition);
        // These variables repersent the world positions of the corners of the screen
        Vector3 upperLeftCorner = Camera.main.ScreenToWorldPoint(upperLeftScreen); // upper left corner position
        Vector3 upperRightCorner = Camera.main.ScreenToWorldPoint(upperRightScreen); // upper right corner position
        Vector3 lowerLeftCorner = Camera.main.ScreenToWorldPoint(lowerLeftScreen); // lower left corner position
        Vector3 lowerRightCorner = Camera.main.ScreenToWorldPoint(lowerRightScreen); // lower right corner position



        // This if else stamtment is used to determine what directional coderent the player wants to attack in
        if (calculateIfMouseIsInTriangle(mousePosition, rigidbody.transform.position, upperLeftCorner, upperRightCorner)) // Does the player want to attack to the north?
        {
            animator.SetInteger("Direction", 1);
            camera.GetComponent<CameraController>().target = new Vector2(rigidbody.transform.position.x, rigidbody.transform.position.y + directionFacing);
            directionFacing = 1;
            //print("mouse found north of the player"); 
        }
        else if (calculateIfMouseIsInTriangle(mousePosition, rigidbody.transform.position, upperRightCorner, lowerRightCorner)) // Does the player want to attack to the east?
        {
            animator.SetInteger("Direction", 3);
            camera.GetComponent<CameraController>().target = new Vector2(rigidbody.transform.position.x + directionFacing, rigidbody.transform.position.y);
            directionFacing = 3;
            //print("mouse found east of the player");
        }
        else if (calculateIfMouseIsInTriangle(mousePosition, rigidbody.transform.position, lowerRightCorner, lowerLeftCorner)) // Does the player want to attack to the south?
        {
            animator.SetInteger("Direction", 2);
            camera.GetComponent<CameraController>().target = new Vector2(rigidbody.transform.position.x, rigidbody.transform.position.y - directionFacing);
            directionFacing = 2;
            //print("mouse found south of the player");
        }
        else if (calculateIfMouseIsInTriangle(mousePosition, rigidbody.transform.position, lowerLeftCorner, upperLeftCorner)) // Does the player want to attack to the west?
        {
            animator.SetInteger("Direction", 4);
            camera.GetComponent<CameraController>().target = new Vector2(rigidbody.transform.position.x - directionFacing, rigidbody.transform.position.y);
            directionFacing = 4;
            //print("mouse found west of the player");
        }
    }


    private void controllerGunLocation()
    {
            //activeGun.GetComponent<GunController>().switchGun();
            switch (directionFacing)
            {
                case (1):
                    // Gun is not active and disable sprite rendera
                    activeGun.GetComponent<SpriteRenderer>().sprite = null;
                    break;
                case (2):
                    activeGun.GetComponent<SpriteRenderer>().sprite = activeGun.GetComponent<GunController>().gunForward;
                    // Gun has a very spesific sprites
                    break;
                case (3):
                    // Gun has a flips in the z axis
                    activeGun.transform.localScale = new Vector3(1, 1, 1);
                    activeGun.GetComponent<GunController>().switchSprite();
                    break;
                case (4):
                    activeGun.transform.localScale = new Vector3(-1, 1, 1);
                    activeGun.GetComponent<GunController>().switchSprite();
                    // Gun has fliped in the z axsis
                    break;
        }
    }

    // This function will calculate if point p is inside the triangle abc
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

    // This function will be called in scene controllers to determin what direction the player is facing
    private void setDirectionPlayerIsFacing(int direction)
    {
        directionFacing = direction; // Set the direction facing to the correct direction
        //Setup in animator for future // Display the correct animation
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemey Bullet" || collision.tag == "Berserker" || collision.tag == "Boss" || collision.tag == "Ghost")
        {
            livesImages[lives - 1].GetComponent<Animator>().SetBool("IsKilled", true);
            lives -= 1;
            
            if (collision.tag == "Enemey Bullet")
            {
                collision.GetComponent<bulletBehaviour>().canCheck = false;
                Destroy(collision.gameObject);
            }

            if (lives <= 0)
            {
                canMove = false;
                Time.timeScale = 0;
                playerSprite.GetComponent<SpriteRenderer>().color = Color.clear;
                activeGun.GetComponent<GunController>().canShoot = false;
                normalUI.SetActive(false);
                deathScreen.SetActive(true);
                // kill the player
            }
        }
        if(collision.tag == "Rocket")
        {
            livesImages[lives - 1].GetComponent<Animator>().SetBool("IsKilled", true);
            lives -= 1;
            if (lives <= 0)
            {
                canMove = false;
                Time.timeScale = 0;
                playerSprite.GetComponent<SpriteRenderer>().color = Color.clear;
                activeGun.GetComponent<GunController>().canShoot = false;
                normalUI.SetActive(false);
                deathScreen.SetActive(true);
                // kill the player
            }
            else
            {
                livesImages[lives - 1].GetComponent<Animator>().SetBool("IsKilled", true);
                lives -= 1;
                collision.GetComponent<RocketBehaviour>().canCheck = false;
                Destroy(collision.gameObject);

                if (lives <= 0)
                {
                    canMove = false;
                    Time.timeScale = 0;
                    playerSprite.GetComponent<SpriteRenderer>().color = Color.clear;
                    activeGun.GetComponent<GunController>().canShoot = false;
                    normalUI.SetActive(false);
                    deathScreen.SetActive(true);
                    // kill the player
                }
            }
        }
    }


    public void dealDamageToPlayer()
    {
        livesImages[lives - 1].GetComponent<Animator>().SetBool("IsKilled", true);
        lives -= 1;
    }
}
