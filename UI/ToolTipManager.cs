using JetBrains.Annotations;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ArmorData;
using static PlayerSkillInfo;
using static ThrowableData;
using static WeaponData;

public class ToolTipManager : MonoBehaviour
{
    private TextMeshProUGUI tmpText;
    [SerializeField] private GameObject weaponToolTip;
    [SerializeField] private GameObject armorToolTip;
    [SerializeField] private GameObject throwToolTip;
    [SerializeField] private GameObject skillToolTip;
    [SerializeField] private Button[] buttons;
    [SerializeField] private TextMeshProUGUI[] buttonsText;
    [SerializeField] private TextMeshProUGUI[] weapontoolTipBoxText;
    [SerializeField] private TextMeshProUGUI[] armorTipBoxText;
    [SerializeField] private TextMeshProUGUI[] throwTipBoxText;
    [SerializeField] private TextMeshProUGUI[] skilltoolTipBoxText;
    public StoreData storeData;
    private void Awake()
    {
        if(storeData == null)
        {
            storeData = GetComponent<StoreData>();
        }

        weaponToolTip.SetActive(false);
        armorToolTip.SetActive(false);
        throwToolTip.SetActive(false);
        skillToolTip.SetActive(false);

        buttons = GetComponentsInChildren <Button>();
        // buttonsText 배열 초기화
        buttonsText = new TextMeshProUGUI[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            buttonsText[i] = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
        }

        // 각 버튼에 EventTrigger 추가
        foreach (var button in buttons)
        {
            AddEventTrigger(button);
        }
    }

    private void AddEventTrigger(Button button)
    {
        // EventTrigger 컴포넌트 가져오기 또는 추가하기
        EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = button.gameObject.AddComponent<EventTrigger>();
        }

        // PointerEnter 이벤트 핸들러 추가
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((eventData) => { OnPointerEnter((PointerEventData)eventData, button); });
        eventTrigger.triggers.Add(pointerEnter);

        // PointerExit 이벤트 핸들러 추가 (예시)
        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((eventData) => { OnPointerExit((PointerEventData)eventData, button); });
        eventTrigger.triggers.Add(pointerExit);
    }

    public void OnPointerEnter(PointerEventData eventData, Button button)
    {


        // 버튼의 부모 객체 가져오기
        Transform buttonParent = button.transform.parent;

        // 부모 객체에서 TextMeshProUGUI 컴포넌트 찾기
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        // 무기 정보 배열에서 무기 이름과 일치하는 정보 찾기
        if (buttonParent.CompareTag("WeaponGroup"))
        {
            weaponToolTip.SetActive(true);
            armorToolTip.SetActive(false);
            throwToolTip.SetActive(false);
            skillToolTip.SetActive(false);

            for (int i = 0; i < storeData.weaponData.weaponDataInfoArray.Length; i++)
            {
                WeaponDataInfo weaponInfo = storeData.weaponData.weaponDataInfoArray[i];
                if (buttonText.text == weaponInfo.weapon_Name)
                {
                    //Debug.Log("똑같다.");
                    SetWeaponToolTipTexts(weaponInfo);
                    return; // 조건을 만족하는 요소를 찾으면 함수 종료
                }
            }
        }
        if (buttonParent.CompareTag("ArmorGroup"))
        {
            weaponToolTip.SetActive(false);
            armorToolTip.SetActive(true);
            throwToolTip.SetActive(false);
            skillToolTip.SetActive(false);

            for (int i = 0; i < storeData.weaponData.weaponDataInfoArray.Length; i++)
            {
                ArmorDataInfo armorData = storeData.armorData.armorDataInfoArray[i];
                if (buttonText.text == armorData.armor_Name)
                {
                    //Debug.Log("똑같다.");
                    SetArmorToolTipTexts(armorData);
                    return; // 조건을 만족하는 요소를 찾으면 함수 종료
                }
            }

        }
        if (buttonParent.CompareTag("ThrowGroup"))
        {
            weaponToolTip.SetActive(false);
            armorToolTip.SetActive(false);
            throwToolTip.SetActive(true);
            skillToolTip.SetActive(false);
            for (int i = 0; i < storeData.throwableButtonsText.Length; i++)
            {
                ThrowableDataInfo throwableData = storeData.throwableData.throwableDataInfoArray[i];
                if (buttonText.text == throwableData.throwables_Name)
                {
                    //Debug.Log("똑같다.");
                    SetArmorToolTipTexts(throwableData);
                    return; // 조건을 만족하는 요소를 찾으면 함수 종료
                }
            }


        }
        if (buttonParent.CompareTag("SkillGroup"))
        {
            weaponToolTip.SetActive(false);
            armorToolTip.SetActive(false);
            throwToolTip.SetActive(false);
            skillToolTip.SetActive(true);
            for(int i = 0; i < storeData.skillButtonText.Length; i++)
            {
                SkillDataString skillData = storeData.playerSkillInfo.skillDataStringArray[i];
                if(buttonText.text == skillData.skill_name)
                {
                    SetSkillToolTipTexts(skillData);
                }
            }
        }
    }

            public void OnPointerExit(PointerEventData eventData, Button button)
     {
        Transform buttonParent = button.transform.parent;
        if (buttonParent.CompareTag("WeaponGroup"))
        {
            weaponToolTip.SetActive(false);
        }
        if (buttonParent.CompareTag("SkillGroup"))
        {
            skillToolTip.SetActive(false);
        }
        if (buttonParent.CompareTag("ThrowGroup"))
        {
            throwToolTip.SetActive(false);
        }
        if (buttonParent.CompareTag("SkillGroup"))
        {
            skillToolTip.SetActive(false);
        }

        //Debug.Log("마우스가 버튼에서 벗어났습니다.");
     }

    private void SetWeaponToolTipTexts(WeaponDataInfo weaponInfo)
    {
        weapontoolTipBoxText[0].text = weaponInfo.weapon_Name;
        weapontoolTipBoxText[1].text = weaponInfo.weapon_Desc;
        weapontoolTipBoxText[2].text = ConvertWeaponTypeToString(weaponInfo.weapon_Type);
        weapontoolTipBoxText[3].text = ConvertWeaponClassToString(weaponInfo.weapon_Class);
        weapontoolTipBoxText[4].text = ConvertFireTypeToString(weaponInfo.fire_Type);
        weapontoolTipBoxText[5].text = ConvertAimingModeToString(weaponInfo.aiming_Mode);
        weapontoolTipBoxText[6].text = weaponInfo.mounting_Time.ToString();
        weapontoolTipBoxText[7].text = weaponInfo.equip_Run_SPD1.ToString();
        weapontoolTipBoxText[8].text = weaponInfo.equip_Run_SPD2.ToString();
        weapontoolTipBoxText[9].text = ConvertWallPenetrationToString(weaponInfo.wall_Penetration);
        weapontoolTipBoxText[10].text = weaponInfo.first_Shot_Spread1.ToString();
        weapontoolTipBoxText[11].text = weaponInfo.first_Shot_Spread2.ToString();
        weapontoolTipBoxText[12].text = weaponInfo.reload_SPD.ToString();
        weapontoolTipBoxText[13].text = weaponInfo.fire_Rate1.ToString();
        weapontoolTipBoxText[14].text = weaponInfo.fire_Rate2.ToString();
        weapontoolTipBoxText[15].text = weaponInfo.loaded_Ammo_Count.ToString();
        weapontoolTipBoxText[16].text = weaponInfo.spare_Ammo_Count.ToString();
    }

    private void SetSkillToolTipTexts(SkillDataString skillData)
    {
        skilltoolTipBoxText[0].text = skillData.skill_name;
        skilltoolTipBoxText[1].text = skillData.skill_desc;
    }

    private void SetArmorToolTipTexts(ArmorDataInfo armorDataInfo)
    {
        armorTipBoxText[0].text = armorDataInfo.armor_Name;
        armorTipBoxText[1].text = armorDataInfo.armor_Desc;
        armorTipBoxText[2].text = ConvertArmorTypeToString(armorDataInfo.armor_Type);
        armorTipBoxText[3].text = armorDataInfo.armor_Defense.ToString();
    }

    private void SetArmorToolTipTexts(ThrowableDataInfo throwableData)
    {
        throwTipBoxText[0].text = throwableData.throwables_Name;
        throwTipBoxText[1].text = throwableData.throwables_Desc;
    }

    private string ConvertArmorTypeToString(int  armorType)
    {
        return armorType switch
        {
            1 => "소형",
            2 => "중형",
        };
    }
    private string ConvertWeaponTypeToString(int weaponType)
    {
        return weaponType switch
        {
            1 => "소총",
            2 => "기관단총",
            3 => "기관총",
            4 => "저격소총",
            5 => "마크스맨",
            6 => "산탄총",
            7 => "권총",
            _ => "알 수 없음"
        };
    }

    private string ConvertWeaponClassToString(int weaponClass)
    {
        return weaponClass switch
        {
            1 => "주무기",
            2 => "보조무기",
            3 => "궁극기",
            _ => "알 수 없음"
        };
    }

    private string ConvertFireTypeToString(int fireType)
    {
        return fireType switch
        {
            1 => "연발",
            2 => "단발",
            _ => "알 수 없음"
        };
    }

    private string ConvertAimingModeToString(int aimingMode)
    {
        return aimingMode switch
        {
            1 => "지향 사격",
            2 => "지향 사격 + 조준 사격",
            _ => "알 수 없음"
        };
    }

    private string ConvertWallPenetrationToString(int wallPenetration)
    {
        return wallPenetration switch
        {
            1 => "낮음",
            2 => "중간",
            3 => "높음",
            _ => "알 수 없음"
        };
    }
}

