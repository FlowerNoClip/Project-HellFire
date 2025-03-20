using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GunSystem : MonoBehaviour
{
    [SerializeField] private InputActionProperty Rightactivate;
    [SerializeField] private InputActionProperty ABtn;
    
    // 총 정보, inspector에서 총기에 맞게 설정
    [SerializeField] private int damage;
    [SerializeField] private float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    // 발사 간격, 산탄율, 사거리, 장전 시간, 총알이 나가는 간격
    [SerializeField] private int maxBullets, bulletsPerTap;
    // 최대 탄창, 발당 총알 수
    private int curBullets, bulletsShot;
    // 장전된 총알, 발사하는 총알 수

    private bool shooting, reloading; // 상태 확인

    [SerializeField] private Camera guncam; // 총구 앞 카메라
    [SerializeField] private Transform spawnPoint; // 총알이 발사되는 위치
    [SerializeField] private RaycastHit rayHit;

    [SerializeField] private GameObject muzzleFlash, bulletHoleGraphic;
    // 섬광, 총탄 구멍

    [SerializeField] private TextMeshPro text;
    private Coroutine fireCoroutine = null;

    private void Awake()
    {
        curBullets = maxBullets;
        shooting = false;
        reloading = false;
    }

    private void Start()
    {
        XRGrabInteractable XGI = GetComponent<XRGrabInteractable>();

        XGI.activated.AddListener(StartFire);
        XGI.deactivated.AddListener(StopFire);


    }

    private void Update()
    {
        text.SetText(curBullets + " / " + maxBullets);

        if (ABtn.action.ReadValue<float>() > 0.5f && !reloading && curBullets < maxBullets)
        {
            reloading = true;
            Invoke("ReloadFinished", reloadTime);
        }
    }

    private void StartFire(ActivateEventArgs arg)
    {
        if (!shooting && !reloading && curBullets > 0)
        {
            shooting = true;
            fireCoroutine = StartCoroutine(FireBullet());
        }
    }

    private void StopFire(DeactivateEventArgs arg)
    {
        if (shooting)
        {
            shooting = false;
            if (fireCoroutine != null)
            {
                StopCoroutine(fireCoroutine);
            }
        }
    }

    private IEnumerator FireBullet()
    {
        while (shooting && curBullets > 0)
        {
            for (int i = 0; i < bulletsPerTap; i++)
            {
                BulletSystem();
                yield return new WaitForSeconds(timeBetweenShots);
            }

            yield return new WaitForSeconds(timeBetweenShooting);
        }

        if (curBullets <= 0)
        {
            shooting = false;
        }
    }

    private void BulletSystem()
    {
        if (Rightactivate.action.ReadValue<float>() > 0.5f && shooting && curBullets > 0)
        {
            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);

            Vector3 direction = guncam.transform.forward + new Vector3(x, y, 0);

            if (Physics.Raycast(guncam.transform.position, direction, out rayHit, range))
            {
                Debug.Log(damage);
            }

            Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));
            Instantiate(muzzleFlash, spawnPoint.position, Quaternion.identity);

            curBullets--;
            bulletsShot--;
        }
    }

    private void ReloadFinished()
    {
        curBullets = maxBullets;
        reloading = false;
    }
}