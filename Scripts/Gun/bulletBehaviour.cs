using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletBehaviour : MonoBehaviour
{
    public Vector2 slope;
    private float timeOut;
    public float Damage;
    private Rigidbody2D rigidbody;
    public bool canCheck;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        canCheck = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // rigidbody.transform.Translate(slope * Time.deltaTime);
    }



    public IEnumerator outOfReach()
    {
        yield return new WaitForSeconds(0.3f);
        if (canCheck == true)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player" && collision.tag != "Enemy" && collision.tag != "Dedection" && collision.tag != "Boss") 
        {
            canCheck = false;
            Destroy(this.gameObject);
        }
    }
}
