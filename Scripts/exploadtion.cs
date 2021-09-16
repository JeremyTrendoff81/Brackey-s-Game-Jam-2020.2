using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exploadtion : MonoBehaviour
{

    private void Start()
    {
        StartCoroutine(destroy());
    }

    public IEnumerator destroy()
    {
        yield return new WaitForSeconds(0.4f);
        Destroy(this.gameObject);
    }
}
