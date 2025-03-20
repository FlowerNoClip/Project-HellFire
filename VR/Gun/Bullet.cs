using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float lifeTime = 2f;
    private float destroyDelay = 0.01f;

    private string trigTag = null;
    private string rayTag = null;

    private void Start()
    {
        Destroy(this.gameObject, lifeTime);
    }

    private void Update()
    {
        RayCheck();
        CheckDestroy();
    }

    private void OnCollisionEnter(Collision other)
    {
        trigTag = other.collider.name;
    }

    private void RayCheck()
    {
        Ray ray = Camera.main.ScreenPointToRay(transform.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            rayTag = hit.collider.name;
        }
    }

    private void CheckDestroy()
    {
        if ((trigTag != null) && (rayTag != null))
        {
            Destroy(this.gameObject, destroyDelay);
        }
    }
}