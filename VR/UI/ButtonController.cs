using UnityEngine;

public class ButtonController : MonoBehaviour
{  
    //Setting버튼 파트
    [SerializeField] GameObject SettingMenu;
    [SerializeField] GameObject ActivationUI1;
    [SerializeField] GameObject ActivationUI2;
    [SerializeField] GameObject ActivationUI3;
    [SerializeField] GameObject CrossHairMaking;
    GameObject Setting;
    GameObject SettingBack;
    GameObject KeySetting;
    GameObject CrossHair;
    GameObject Sound;

    GameObject Home;

    GameObject StartButton;

    GameObject Shop;

    GameObject Character;


    void Start()
    {
        CrossHairMaking.SetActive(false);
        SettingMenu.SetActive(false);
        ActivationUI1.SetActive(false);
        ActivationUI2.SetActive(false);
        ActivationUI3.SetActive(false);
    }

    public void SettingButton()
    {
        SettingMenu.SetActive(true);
    }

    public void SettingBackButton()
    {
        SettingMenu.SetActive(false);
    }

    public void CrossHairButton()
    {
        CrossHairMaking.SetActive(true);
        ActivationUI1.SetActive(true);
        ActivationUI2.SetActive(false);
        ActivationUI3.SetActive(false);
    }

    public void SoundButton()
    {
        ActivationUI2.SetActive(true);
        ActivationUI1.SetActive(false);
        ActivationUI3.SetActive(false);
        CrossHairMaking.SetActive(false);
    }
    public void KeySettingButton()
    {
        ActivationUI3.SetActive(true);
        ActivationUI2.SetActive(false);
        ActivationUI1.SetActive(false);
        CrossHairMaking.SetActive(false);
    }
}
