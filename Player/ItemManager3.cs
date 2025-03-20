using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;
public class ItemManager3 : MonoBehaviour
{
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private StoreData storeData;
    [SerializeField]private ItemManager IM;



    private void Awake()
    {
        inputSystem = GetComponentInParent<InputSystem>();
        

        //storeData = inputSystem.store.GetComponentInChildren<StoreData>();
        //storeData.itemManager = gameObject.GetComponent<ItemManager3>();
    }
    //Ư������ �˻��ؼ� �̾��� ������������ 1�̸� �ֹ��⿡ �߰�, �ƴϸ� �������⿡ �߰�
    private void Start()
    {
        storeData = Store.Instance.returnStoreData();
        storeData.itemManager = this;
    }


    public void InitializeGun(int IDX)
    {
        
        switch(IDX)
        {
            case 3000:
                //���
                IM.GetGunIndex(IDX);
                //Instantiate(MainGunArr[0], new Vector3(0.2f , -0.25f, 0.2f),Quaternion.identity , MainGun.transform);
                break;
            case 3001:
                break;
            case 3002:
                break;
            case 3003:
                break;
            case 3004:
                break;
            case 3005:
                break;
            case 3006:
                break;
            case 3007:
                //���۷�����
                IM.GetGunIndex(IDX);
                break;
            case 3008:
                break;
            case 3009:
                break;
            case 3010:
                break;
            case 3011:
                break;
            case 3012:
                break;
            case 3013:
                break;
            case 3014:
                break;
            case 3015:
                //Ŭ����
                IM.GetGunIndex(IDX);
                //Instantiate(SubGunArr[0], new Vector3(0.2f, -0.25f, 0.2f), Quaternion.identity, MainGun.transform);
                break;
            case 3016:
                break;
            default:
                break;


        }

    }


    public void InitializeSkill(int IDX)
    {
        Debug.Log(IDX);
        switch (IDX)
        {
            //����ź
            case 3:
                IM.GetSkillIndex(IDX);
                break;
            default:
                break;
        }
    }

    public void IntializeGrenade(int IDX)
    {
        Debug.Log(IDX);
        switch (IDX)
        {
            case 6000://����ź
                break;
            case 6001:
                IM.GetGrenadeIndex(IDX);
                break;
            default:
                break;

        }
    }
}
