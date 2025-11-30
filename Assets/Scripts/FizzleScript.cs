using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FizzleScript : MonoBehaviour
{
    [SerializeField] float fizzleTime;

    void Update()
    {
        fizzleTime -= Time.deltaTime;
        if (fizzleTime < 0)
        {
            Destroy(this.gameObject);
        }
    }
}
