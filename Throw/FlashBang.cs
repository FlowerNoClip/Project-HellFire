using UnityEngine;

public class FlashBang : MonoBehaviour
{
    [SerializeField] private float fusetime = 3f;
    [SerializeField] private GameObject explosionPrefab;
    private Camera cam;
    

    private void Start()
    {
        Invoke("Explode", fusetime);
        cam = Camera.main;
    }

    private void Explode()
    {
        Destroy(gameObject);
        if (CheckVisibility())
        {
            Debug.Log("Go Blind");
            BlindnessEffect.activeInstance.GoBlind();
        }
        else
        {
            Destroy(Instantiate(explosionPrefab, transform.position, Quaternion.identity), 5);
        }
        
    }

    private bool CheckVisibility()
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(cam);
        var point = transform.position;

        foreach(var p in planes)
        {
            if(p.GetDistanceToPoint(point) > 0)
            {
                Ray ray = new Ray(cam.transform.position, transform.position - cam.transform.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                    return hit.transform.gameObject == this.gameObject;
                else return false;
            }
            else return false;
        }

        return false;
    }

    
}
