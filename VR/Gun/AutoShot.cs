using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class AutoShot : MonoBehaviour // 연발
{
    [SerializeField] private InputActionProperty RightActivate;
    [SerializeField] private InputActionProperty ABtn;

    [SerializeField] private GameObject bullet = null;
    [SerializeField] private Transform spawnPoint = null;
    [SerializeField] private TMP_Text CountText = null;

    private float speed = 50.0f;
    private int maxBullet = 30;
    private int curBullet = 0;
    private float interval = 0.1f; // 발사 간격

    private bool isFiring = false; // 발사 상태
    private Coroutine firingCoroutine = null; // 발사 코루틴

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
            firingCoroutine = StartCoroutine(AutoFire());
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

    private IEnumerator AutoFire()
    {
        while (isFiring && curBullet > 0)
        {
            FireBullet();
            yield return new WaitForSeconds(interval);
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
