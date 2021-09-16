using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewindMechanic : MonoBehaviour
{
    private Vector3 rewindLocation; // Vector to hold the rewind location   
    private Vector3 zombieLocation; // Vector to hold the zombie spawn location
    
    private Rigidbody2D rigidbody2D; // Represents the player
    
    private GameObject RewindMarker; // RewindMarker Sprite
    public GameObject OrignialRewindMarker; // Sprite to allow for multiple markers to be placed
    
    public GameObject OrignialZombiePlayer; // Sprite to allow for multiple zombies
    private GameObject ZombiePlayer; // Zombie Sprite

    public GameObject player;

    private bool isReady = false; // Boolean to notify when rewind is possible.'


    public Animator rewindAnimator;
    public Animator flashAnimator;

    private bool canRewind;
    public Slider rewindSlider;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>(); // Assign ridgidbody
        rewindSlider.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && canRewind == true) // If the Q key is pressed...
        { 
            switch (isReady) 
            {
                case false: // Once...
                    setRewindLocation(); // set the rewind location
                    createRewindMarker(); // create and spawn the rewind marker
                    break;

                case true: // Again...
                    this.GetComponent<PlayerController>().canMove = false;
                    setZombieLocation(); // set the zobie spawn location
                    rewindAnimator.SetBool("IsRewind", true);
                    StartCoroutine(waitFlash());
                    rewindSlider.value = 0;
                    StartCoroutine(SpawnZombiePlayer(1.0f)); // spawn the zombie           
                    break;
            }

        }

    }

    private IEnumerator waitFlash()
    {
        yield return new WaitForSeconds(0.32f);
        flashAnimator.SetBool("isFlash", true);
        yield return new WaitForSeconds(0.05f);
        rewind(); // rewind
        this.GetComponent<PlayerController>().canMove = true;
        destroyRewindMarker(); // destroy the rewind marker 
        flashAnimator.SetBool("isFlash", false);
    }

    private void rewind() // Function to rewind time 
    {
        rigidbody2D.transform.position = rewindLocation; // rewind
        isReady = false; // get ready to reset rewind position
        canRewind = false;
    }

    private void setRewindLocation() // Function to set the rewind location
    {
        rewindLocation = rigidbody2D.transform.position; //set the location
        isReady = true; // ready to rewind
    }

    private void createRewindMarker() // Function to create the rewind marker
    {
        RewindMarker = Instantiate(OrignialRewindMarker, new Vector3(rigidbody2D.transform.position.x, rigidbody2D.transform.position.y,1), Quaternion.identity); // Create the marker
    }

    private void destroyRewindMarker()
    {
        Destroy(RewindMarker); //Destroy the marker
    }

    private IEnumerator SpawnZombiePlayer(float waitTime) // Spawns in zombie with delay
    {
        yield return new WaitForSeconds(waitTime); // delay
        ZombiePlayer = Instantiate(OrignialZombiePlayer, zombieLocation, Quaternion.identity); // spawn
        rewindAnimator.SetBool("IsRewind", false);
    }

    private void setZombieLocation() 
    {
        zombieLocation = rigidbody2D.transform.position; // set the zombies spawn location
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == ("Time Capsule") && rewindSlider.value < 4)
        {
            rewindSlider.value += 1;
            Destroy(collision.gameObject);

            if(rewindSlider.value == 4)
            {
                canRewind = true;
            }
        }
    }

}
