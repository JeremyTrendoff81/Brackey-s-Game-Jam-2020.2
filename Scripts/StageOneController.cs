using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageOneController : MonoBehaviour
{
    public GameObject Door;
    public Sprite doorPart2;


    // Start is called before the first frame update
    public void updateDoor()
    {
        Door.GetComponent<SpriteRenderer>().sprite = doorPart2;
        Door.GetComponent<BoxCollider2D>().enabled = false;
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        updateDoor();
        Destroy(this.gameObject);
    }
}
