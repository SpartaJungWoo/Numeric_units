using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject destroyEffect;

    public void Destroy()
    {
        Instantiate(destroyEffect, transform.position, destroyEffect.transform.rotation);
        Destroy(gameObject);
    }
}
