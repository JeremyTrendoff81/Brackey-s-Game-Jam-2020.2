using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public GameObject player;
    public GameObject EnemyBullet;
    public GameObject Rocket;
    public GameObject timeCapsule;

    public Pathfinding.AIDestinationSetter destinationSetter;

    public Pathfinding.AIPath AIPath;

    public float health;
    public int enemyCounter;
    private Vector3 slope;
    public float bulletSpeed;
    public float angle;
    private bool canShoot;
    public float timeToFire;

    public Rigidbody2D rigidbody;
    public Animator animator;
    public int directionFacing;
    private float distance;
    private bool backToBeginningAttackType;
    private float timeToSwitch;
    private enum AttackType {Enemy, Basooka, Berserker};
    private AttackType attackType;

    public GameObject activeSprite;
    public Sprite forwardGun;
    public Sprite mainGun;

    public Sprite Pistol;
    public Sprite PistolForward;
    public Sprite Bazooka;
    public Sprite BazookaForwards;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //destinationSetter.target = player.transform;
        health = 500f;
        attackType = AttackType.Enemy;
        bulletSpeed = 15;
        timeToFire = 1f;
        timeToSwitch = 10f;
    }

    private void controllerGunLocation()
    {
        //activeGun.GetComponent<GunController>().switchGun();
        switch (directionFacing)
        {
            case (1):
                // Gun is not active and disable sprite rendera
                activeSprite.GetComponent<SpriteRenderer>().sprite = null;
                break;
            case (2):
                activeSprite.GetComponent<SpriteRenderer>().sprite = forwardGun;
                // Gun has a very spesific sprites
                break;
            case (3):
                // Gun has a flips in the z axis
                activeSprite.transform.localScale = new Vector3(1, 1, 1);
                activeSprite.GetComponent<SpriteRenderer>().sprite = mainGun;
                break;
            case (4):
                activeSprite.transform.localScale = new Vector3(-1, 1, 1);
                activeSprite.GetComponent<SpriteRenderer>().sprite = mainGun;
                // Gun has fliped in the z axsis
                break;
        }
    }

    private void seeIfEnemyIsInRange()
    {
        if (destinationSetter.target == null)
        {
            distance = calcDistance();
            if (distance < 8.5)
            {
                destinationSetter.target = player.transform;
                StartCoroutine(WaitToAttack());
                StartCoroutine(WaitToSwitch());
            }
            Debug.DrawLine(transform.position, player.transform.position);
        }
    }

    private float calcDistance()
    {
        return Mathf.Sqrt(Mathf.Pow(player.transform.position.x - transform.position.x, 2) + Mathf.Pow(player.transform.position.y - transform.position.y, 2));
    } 

    // Update is called once per frame
    void FixedUpdate()
    {
        determineDirectionFacing();
        controllerGunLocation();
        seeIfEnemyIsInRange();
        
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
        if (collision.tag == "Bullet")
        {
            health -= player.GetComponent<PlayerController>().activeGun.GetComponent<GunController>().Damage;
            collision.GetComponent<bulletBehaviour>().canCheck = false;
            Destroy(collision.gameObject);
            if (health <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void aim()
    {
        slope = (Vector3.ClampMagnitude(player.transform.position - transform.position, 1)).normalized * bulletSpeed;
        float mx = player.transform.position.x - transform.position.x;
        float my = player.transform.position.y - transform.position.y;
        angle = Mathf.Atan(my / mx);
        
        if (player.transform.position.x > transform.position.x)
        {
            angle = (180 / Mathf.PI) * angle - 90;
        }

        else if (player.transform.position.x < transform.position.x)
        {
            angle = (180 / Mathf.PI) * angle + 90;
        }
    }

    private void fire(Vector3 slope, float angle)
    {
        GameObject currentEnemyBullet;
        currentEnemyBullet = Instantiate(EnemyBullet, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.Euler(0, 0, angle));
        currentEnemyBullet.GetComponent<Rigidbody2D>().velocity = slope;
        currentEnemyBullet.GetComponent<bulletBehaviour>().slope = slope;
        currentEnemyBullet.GetComponent<bulletBehaviour>().Damage = 1;
        //StartCoroutine(currentEnemyBullet.GetComponent<bulletBehaviour>().outOfReach());
    }

    private void fireRocket(Vector3 slope, float angle)
    {
        GameObject currentEnemyBullet;
        currentEnemyBullet = Instantiate(Rocket, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.Euler(0, 0, angle));
        currentEnemyBullet.GetComponent<RocketBehaviour>().Player = player;
        currentEnemyBullet.GetComponent<Rigidbody2D>().velocity = slope;
        currentEnemyBullet.GetComponent<RocketBehaviour>().slope = slope;
        currentEnemyBullet.GetComponent<RocketBehaviour>().Damage = 1;
        currentEnemyBullet.GetComponent<RocketBehaviour>().determinDistanceOfExpostion(player.transform.position, transform.position);
        currentEnemyBullet.GetComponent<RocketBehaviour>().Parent = this.gameObject;
    }

    private void fireGun() 
    {
        forwardGun = PistolForward;
        mainGun = Pistol;
        AIPath.maxSpeed = 5;
        AIPath.endReachedDistance = 3;
        timeToSwitch = 5f;

        aim();
        fire(slope, angle);
    }

    private void fireBaskooka()
    {
        forwardGun = BazookaForwards;
        mainGun = Bazooka;
        AIPath.maxSpeed = 3;
        AIPath.endReachedDistance = 3;
        timeToSwitch = 10f;

        // Fire the rocket launcher
        aim();
        fireRocket(slope, angle);
    }

    private void berserkerCharge()
    {
        forwardGun = null;
        mainGun = null;
        AIPath.maxSpeed = 12;
        AIPath.endReachedDistance = 0;
        backToBeginningAttackType = true;
        timeToSwitch = 10f;
        // Charge at player like berserker 
    }

    private void attack()
    {

        switch(attackType)
        {
            case AttackType.Enemy:
                fireGun();
                break;

            case AttackType.Basooka:
                fireBaskooka();
                break;

            case AttackType.Berserker:
                berserkerCharge();
                break;
        }

    }

    private void changeAttackTypes() 
    {
       attackType++;
       if (backToBeginningAttackType)
        {
            attackType = AttackType.Enemy;
            backToBeginningAttackType = false;
        }
    }

    public IEnumerator WaitToAttack()
    {
        yield return new WaitForSeconds(timeToFire);
        attack();
        StartCoroutine(WaitToAttack());
    }

    public IEnumerator WaitToSwitch()
    {
        yield return new WaitForSeconds(timeToSwitch);
        changeAttackTypes();

        StartCoroutine(WaitToSwitch());
    }
}
