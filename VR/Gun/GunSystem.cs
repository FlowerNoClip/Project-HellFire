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
    
    // �� ����, inspector���� �ѱ⿡ �°� ����
    [SerializeField] private int damage;
    [SerializeField] private float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    // �߻� ����, ��ź��, ��Ÿ�, ���� �ð�, �Ѿ��� ������ ����
    [SerializeField] private int maxBullets, bulletsPerTap;
    // �ִ� źâ, �ߴ� �Ѿ� ��
    private int curBullets, bulletsShot;
    // ������ �Ѿ�, �߻��ϴ� �Ѿ� ��

    private bool shooting, reloading; // ���� Ȯ��

    [SerializeField] private Camera guncam; // �ѱ� �� ī�޶�
    [SerializeField] private Transform spawnPoint; // �Ѿ��� �߻�Ǵ� ��ġ
    [SerializeField] private RaycastHit rayHit;

    [SerializeField] private GameObject muzzleFlash, bulletHoleGraphic;
    // ����, ��ź ����

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