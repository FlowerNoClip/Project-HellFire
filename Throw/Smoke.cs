using System.Collections;
using FishNet.Object;
using UnityEngine;
using UnityEngine.VFX;

public class Smoke : NetworkBehaviour
{
    [SerializeField] private VisualEffect vfx;
    [SerializeField] private float fusetime = 3f;
    [SerializeField] private float duration = 16f;
    [SerializeField] private float rayDisatnce = 0.2f;

    private void Start()
    {
        vfx = GetComponent<VisualEffect>();
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Invoke("ActiveSmoke", fusetime);
    }
    private void ActiveSmoke()
    {
        if (vfx != null)
        {
            vfx.Play();
            Invoke("StopSmoke", duration);
        }
    }
    private void StopSmoke()
    {
        if (vfx != null)
        {
            vfx.Stop();
            Destroy(transform.gameObject);
        }
    }
}