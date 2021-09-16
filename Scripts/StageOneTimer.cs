using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageOneTimer : MonoBehaviour
{
    public Text controllText;
    private bool canMoveOn;
    public Animator animator;
    public Image[] image;

    private void Start()
    {
        StartCoroutine(stageOneTimer());
        controllText.enabled = false;
        animator.SetBool("canMoveOn", false);
    }

    private IEnumerator stageOneTimer()
    {
        yield return new WaitForSeconds(23f);
        canMoveOn = true;
        controllText.enabled = true;
        animator.SetBool("canMoveOn", true);
        for(int i = 0; i < image.Length; i++)
        {
            image[i].enabled = false;
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && canMoveOn == true)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
        }
    }
}
