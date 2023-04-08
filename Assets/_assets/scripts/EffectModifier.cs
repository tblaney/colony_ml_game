using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectModifier : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> _particles;

    public int _index;

    void Awake() 
    {

    }

    public void Run(float time = 3f, int index = 0)
    {
        _index = index;

        ActivateParticles();
        Invoke("Stop", time);
    }

    public void RunLoop(int index = 0)
    {
        _index = index;

        ActivateParticles();
    }

    public void Stop()
    {
        Destroy(this.gameObject);
    }


    public void ActivateParticles(bool isActive = true) 
    {
        foreach (ParticleSystem system in _particles) 
        {
            if (isActive) {
                system.gameObject.SetActive(true);
                system.Play();
            } else {
                system.gameObject.SetActive(false);
                system.Stop();
            }
        }
    }

    public void SetPosition(Vector3 pos) {
        transform.position = pos;
    }
}
