using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBehaviour : MonoBehaviour
{
    public Vector2 slope;
    private float timeOut;
    public float Damage;
    private Rigidbody2D rigidbody;
    public bool canCheck;
    public float exploadDistance;
    public float currentDistance;
    public GameObject Player;
    public GameObject Parent;
    public GameObject explotion;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        canCheck = true;
    }


    public void determinDistanceOfExpostion(Vector2 a, Vector2 b)
    {
        exploadDistance = Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2));
    }
    public void determinDistanceOfExpostionCurrent(Vector2 a, Vector2 b)
    {
        currentDistance = Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2));
    }

    private void FixedUpdate()
    {
        determinDistanceOfExpostionCurrent(Parent.transform.position, transform.position);
        if (exploadDistance <= currentDistance)
        {
            rigidbody.velocity = new Vector2(0, 0);

            float distance = Mathf.Sqrt(Mathf.Pow(Player.transform.position.x - transform.position.x, 2) + Mathf.Pow(Player.transform.position.y - transform.position.y, 2));

            if(distance < 1.5f)
            {
                Player.GetComponent<PlayerController>().dealDamageToPlayer();
            }
            spawnExploation();

            Destroy(this.gameObject);
        }
    } 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            canCheck = false;
            spawnExploation();
            Destroy(this.gameObject);
        }
    }

    public void spawnExploation()
    {
        Instantiate(explotion, transform.position, Quaternion.identity);
    }
}
