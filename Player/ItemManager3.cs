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
    //특정값을 검사해서 이안의 무기종류값이 1이면 주무기에 추가, 아니면 보조무기에 추가
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
                //밴달
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
                //오퍼레이터
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
                //클래식
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
            //수류탄
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
            case 6000://연막탄
                break;
            case 6001:
                IM.GetGrenadeIndex(IDX);
                break;
            default:
                break;

        }
    }
}
