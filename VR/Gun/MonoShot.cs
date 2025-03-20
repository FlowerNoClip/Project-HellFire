using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MonoShot : MonoBehaviour
{
    [SerializeField] private InputActionProperty RightActivate;
    [SerializeField] private InputActionProperty ABtn;

    [SerializeField] private GameObject bullet = null;
    [SerializeField] private Transform spawnPoint = null;
    [SerializeField] private TMP_Text CountText = null; // ź�� ǥ�� â

    private float speed = 50.0f; // �Ѿ� �ӵ�
    private int maxBullet = 10; // �ִ� ź��
    private int curBullet = 0; // ���� ������ ź��
    private bool isFiring = false;

    private void Start()
    {
        XRGrabInteractable XGI = GetComponent<XRGrabInteractable>();

        XGI.activated.AddListener(FireBullet);

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

    private void FireBullet(ActivateEventArgs arg)
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
