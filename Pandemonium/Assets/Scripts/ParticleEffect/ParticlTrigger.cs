using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlTrigger : MonoBehaviour
{
    private ParticleSystem[] ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = this.gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particles in ps)
        {
            particles.Play();
        }
        Destroy(this.gameObject);
    }
}
