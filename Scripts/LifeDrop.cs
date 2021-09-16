using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeDrop : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.GetComponent<PlayerController>().lives < 3)
        {
            collision.GetComponent<PlayerController>().lives += 1;
            collision.GetComponent<PlayerController>().livesImages[collision.GetComponent<PlayerController>().lives - 1].GetComponent<Animator>().SetBool("IsKilled", false);
            Destroy(this.gameObject);
        }
    }
}
