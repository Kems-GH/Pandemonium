using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerParticles : MonoBehaviour 
{
    private ParticleSystem[] ps;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitToDestroy());
    }

    IEnumerator WaitToDestroy()
    {
        ps = this.gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particles in ps)
        {
            particles.Play();
        }
        yield return new WaitForSeconds(2);
        Destroy(this.gameObject);
    }
}
