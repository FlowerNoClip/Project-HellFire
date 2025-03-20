using FishNet.Component.Spawning;
using Steamworks;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ItemManager2 : MonoBehaviour
{
    [SerializeField] Weapon currentWeapon;
    [SerializeField] Gun currentGun;
    [SerializeField] GameObject MainGun;
    [SerializeField] GameObject SubGun;
    [SerializeField] GameObject Knife;
    [SerializeField] GameObject Granade;
    [SerializeField] GameObject Skill1;
    [SerializeField] GameObject Skill2;
    [SerializeField] Weapon mainGun;
    [SerializeField] Weapon subGun;
    [SerializeField] Weapon knife;
    [SerializeField] Weapon grenade;
    [SerializeField] Weapon skill1;
    [SerializeField] Weapon skill2;
    private RoundManager roundManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        knife = Knife.transform.GetChild(0).GetComponent<Weapon>();
        //subGun = SubGun.transform.GetChild(0);
        currentWeapon = knife;
        knife.gameObject.SetActive(true);
        roundManager = RoundManager.Instance;
    }

    
    public void Item2SetMainGun(GameObject g)
    {
        mainGun = g.GetComponent<Weapon>();
       
    }
    public void Item2SetSubGun(GameObject g)
    {
        subGun = g.GetComponent<Weapon>();
    }
    public void Item2SetSkill1(GameObject g)
    {
        skill1 = g.GetComponent<Weapon>();
    }
    public void Item2SetSkill2(GameObject g)
    {
        skill2 = g.GetComponent<Weapon>();
    }
    public void Item2SetGrenade(GameObject g)
    {
        grenade = g.GetComponent<Weapon>();
    }




    // Update is called once per frame

    public void SetMainGun()
    {
        if (mainGun != null)
        {
            currentGun = mainGun.GetComponent<Gun>();
            SetCurrentWeapon(mainGun);
            roundManager.bulletText[0].text = currentGun._currentAmmoInClip.ToString();
            roundManager.bulletText[1].text = currentGun._ammoInReserve.ToString();
        }
        
    }
    public void SetSubGun()
    {
        if (subGun != null)
        {
            currentGun = subGun.GetComponent<Gun>();
            SetCurrentWeapon(subGun);
            roundManager.bulletText[0].text = currentGun._currentAmmoInClip.ToString();
            roundManager.bulletText[1].text = currentGun._ammoInReserve.ToString();
        }
    }
    public void SetKnife()
    {
        if (knife != null)
        {
            SetCurrentWeapon(knife);
        }
    }
    public void SetSkill1()
    {
        if (skill1 != null)
        {
            SetCurrentWeapon(skill1);
        }
    }
    public void SetSkill2()
    {
        if (skill2 != null)
        {
            SetCurrentWeapon(skill2);
        }
    }
    public void SetGrenade()
    {
        if (grenade != null)
        {
            SetCurrentWeapon(grenade);
        }
    }

    public void AttackCurrentWeapon()
    {
        if(currentWeapon != null)
        {
            currentWeapon.Attack();
        }
    }

    public void ZoomCurrentWeapon()
    {
        if (currentWeapon != null)
        {
            currentWeapon.RightClick();
        }
    }


    public void ReloadCurrentWeapon()
    {
       currentGun =  currentWeapon.GetComponent<Gun>();
       currentGun.Reload();
    }

    public Gun ReturnCurrentGun()
    {
        
        return currentGun;
    }


    public void SetCurrentWeapon(Weapon weapon)
    {
            if (weapon == currentWeapon)
            {
                return;
            }

            if (currentWeapon != null)
            {
                currentWeapon.Rebinding();
                currentWeapon.gameObject.SetActive(false);
            }



            weapon.gameObject.SetActive(true);
            currentWeapon = weapon;
            }
        
    
}
