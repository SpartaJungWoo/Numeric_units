using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParticleRendererLayerScript : MonoBehaviour
{
    void Start()
    {
        GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = "Default";
        GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = 1;
    }
}
