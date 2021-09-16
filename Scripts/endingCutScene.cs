using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class endingCutScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(endGame());
    }

    private IEnumerator endGame()
    {
        yield return new WaitForSeconds(65f);
        SceneManager.LoadScene(0, LoadSceneMode.Single);

    }
}
