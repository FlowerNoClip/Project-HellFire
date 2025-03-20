using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    private WeaponData weaponData;
    private DBHolder dbHolder;
    RecoilPatterns recoilPatterns;
    [SerializeField] private ItemManager2 itemManager2;
    [SerializeField] private GameObject[] MainGunArr;
    [SerializeField] private GameObject[] SubGunArr;
    [SerializeField] private GameObject[] SkillArr;
    [SerializeField] private GameObject[] GrenadeArr;
    [SerializeField] private MainGunController MainGun;
    [SerializeField] private SubGunController SubGun;
    [SerializeField] private knifeController knifeController;
    [SerializeField] private GameObject SkillController;
    [SerializeField] private GameObject SkillController2;
    [SerializeField] private GameObject GrenadeController;



    Gun status;
    public void Awake() {
        dbHolder = DBHolder.Instance;
        weaponData = dbHolder.GetComponent<WeaponData>();
        recoilPatterns = new RecoilPatterns();
    }
    private void Start() {

        weaponData.GetWeaponData();
        knifeController.SetKnife(knifeController.transform.GetChild(0).GetComponent<Knife>());
        
    }
    public void GetSkillIndex(int IDX)
    {
        if(IDX == 3)
        {
            GameObject G = Instantiate(SkillArr[0], SkillController2.transform.position, SkillController2.transform.rotation, SkillController2.transform);

            itemManager2.Item2SetSkill2(G);
            itemManager2.SetSkill2();

        }
    }

    public void GetGrenadeIndex(int IDX)
    {
        if(IDX == 6001)
        {
            GameObject G = Instantiate(GrenadeArr[0], GrenadeController.transform.position, GrenadeController.transform.rotation, GrenadeController.transform);

            itemManager2.Item2SetGrenade(G);
            itemManager2.SetGrenade();
        }
    }

    public void GetGunIndex(int IDX)
    {
        
        if (IDX == 3000)
        {
            DeleteGun(MainGun.gameObject);
            GameObject g = Instantiate(MainGunArr[0], MainGun.transform.position, MainGun.transform.rotation, MainGun.transform);
            
            itemManager2.Item2SetMainGun(g);
            status = g.GetComponent<Gun>();

            

            MainGun.SetGun(status);
            for (int i = 0; i < weaponData.weaponDataInfoArray.Length; i++)
            {
                if(weaponData.weaponDataInfoArray[i].index == IDX)
                {
                    UpdateGunInformation(i, IDX);
                    break;
                }
            }
            itemManager2.SetMainGun();
        }
        if(IDX == 3007)
        {
            DeleteGun(MainGun.gameObject);
            GameObject g = Instantiate(MainGunArr[1], MainGun.transform.position, MainGun.transform.rotation, MainGun.transform);

            itemManager2.Item2SetMainGun(g);
            status = g.GetComponent<Gun>();
            MainGun.SetGun(status);
            for (int i = 0; i < weaponData.weaponDataInfoArray.Length; i++)
            {
                if (weaponData.weaponDataInfoArray[i].index == IDX)
                {
                    UpdateGunInformation(i, IDX);
                    break;
                }
            }
            itemManager2.SetMainGun();

        }
        if(IDX == 3015)
        {
            DeleteGun(SubGun.gameObject);
            GameObject g = Instantiate(SubGunArr[0], SubGun.transform.position, SubGun.transform.rotation, SubGun.transform);
            
            itemManager2.Item2SetSubGun(g);
            status = g.GetComponent<Gun>();
            SubGun.SetGun(status);

            for (int i = 0; i < weaponData.weaponDataInfoArray.Length; i++)
            {
                if (weaponData.weaponDataInfoArray[i].index == IDX)
                {
                    UpdateGunInformation(i, IDX);
                    break;
                }
            }
            itemManager2.SetSubGun();
        }
        
    }
    public void DeleteGun(GameObject gun)
    {
        if(gun.transform.childCount > 0)
        {
            Destroy(gun.transform.GetChild(0).gameObject);
        }

    }
    public void UpdateGunInformation(int i, int gi)
    {

        status.RecoilPatterns = recoilPatterns.ReturnRecoil(gi);
        status.IDX = gi;
        status.Weapon_Name = weaponData.weaponDataInfoArray[i].weapon_Name;
        status.Weapon_Type = weaponData.weaponDataInfoArray[i].weapon_Type;
        status.Weapon_Class = weaponData.weaponDataInfoArray[i].weapon_Class;
        status.Fire_Type = weaponData.weaponDataInfoArray [i].weapon_Type;
        status.Fire_Rate1 = weaponData.weaponDataInfoArray[i].fire_Rate1;
        status.Aming_Mode = weaponData.weaponDataInfoArray[i].aiming_Mode;
        status.Reload_SPD = weaponData.weaponDataInfoArray[i].reload_SPD;
        status.Loaded_Ammo_Count = weaponData.weaponDataInfoArray[i].loaded_Ammo_Count;
        status.Spare_Ammo_Count = 30000;
        status.Wall_Penetration = weaponData.weaponDataInfoArray[i].wall_Penetration;
        status.First_Shot_Spread = weaponData.weaponDataInfoArray[i].first_Shot_Spread1;
        status.Equip_Run_SPD1 = weaponData.weaponDataInfoArray [i].equip_Run_SPD1;

    }
}
