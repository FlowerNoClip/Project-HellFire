using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using TMPro;

public class BurstShot : MonoBehaviour // 점사 (수동)
{
    [SerializeField] private InputActionProperty RightActivate;
    [SerializeField] private InputActionProperty ABtn;

    [SerializeField] private GameObject bullet = null;
    [SerializeField] private Transform spawnPoint = null;
    [SerializeField] private TMP_Text CountText = null;

    private float speed = 50.0f;
    private int maxBullet = 30;
    private int curBullet = 0;
    private int burstCount = 3;
    private float interval = 0.1f; // 발사 간격

    private bool isFiring = false;
    private Coroutine firingCoroutine = null;

    private void Start()
    {
        XRGrabInteractable XGI = GetComponent<XRGrabInteractable>();

        XGI.activated.AddListener(StartFiring);
        XGI.deactivated.AddListener(StopFiring);

        curBullet = maxBullet;
    }

    private void Update()
    {
        if (ABtn.action.ReadValue<float>() > 0.5f && isFiring == false)
        {
            curBullet = maxBullet;
            CountText.text = curBullet.ToString();
        }
    }

    private void StartFiring(ActivateEventArgs arg)
    {
        if (!isFiring)
        {
            isFiring = true;
            firingCoroutine = StartCoroutine(BurstFire());
        }
    }

    private void StopFiring(DeactivateEventArgs arg)
    {
        if (isFiring)
        {
            if (firingCoroutine != null)
            {
                isFiring = false;
                StopCoroutine(firingCoroutine);
            }
        }
    }

    private IEnumerator BurstFire()
    {
        while (isFiring && curBullet > 0)
        {
            for (int i = 0; i < burstCount; i++)
            {
                FireBullet();
                yield return new WaitForSeconds(interval);
            }
            yield return new WaitForSeconds(interval + 0.1f);
        }
    }

    private void FireBullet()
    {
        if (RightActivate.action.ReadValue<float>() > 0.5f && curBullet > 0)
        {
            GameObject copyBullet = Instantiate(bullet);

            copyBullet.transform.position = spawnPoint.position;

            copyBullet.GetComponent<Rigidbody>().linearVelocity = spawnPoint.forward * speed;

            curBullet--;
            CountText.text = curBullet.ToString();
        }
    }
}
