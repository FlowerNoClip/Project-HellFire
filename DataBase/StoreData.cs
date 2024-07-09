using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Unity.VisualScripting;
using System.Collections;
using FishNet.Component.Spawning;
public class StoreData : DBConnect
{
    public struct StoresData
    {
        public int index;
        public int item_index;
        public int buy_currency;
        public int item_price;
        public int max_stock;
    }
    public struct StoresDataString
    {
        public int index;
        public string item_index;
        public string item_desc;
        public int item_price;
        public int max_stock;
        public override string ToString()
        {
            return $"Item Name: {item_index}, Item Desc: {item_desc} , Iteml Price : {item_price}";
        }
    }
    public TextMeshProUGUI vineValue;
    public PlayerSkillInfo playerSkillInfo;
    public WeaponData weaponData;
    public ArmorData armorData;
    public ThrowableData throwableData;
    public Canvas canvas;
    public ItemManager3 itemManager;
    public Button[] skillButtons;
    public RoundManager roundManager;
    [HideInInspector] public TextMeshProUGUI[] skillButtonText;
    public Button[] weaponButtons;
    [HideInInspector] public TextMeshProUGUI[] weaponButtonsText;
    public Button[] armorButtons;
    [HideInInspector] public TextMeshProUGUI[] armorButtonsText;
    public Button[] throwableButtons;
    [HideInInspector] public TextMeshProUGUI[] throwableButtonsText;
    public StoresData[] storesDataArray;
    public StoresDataString[] storesDataStringArray;
    public StoresData storesDatas = new StoresData();
    public StoresDataString storesDataString = new StoresDataString();
    public int[] skillprice;
    public int[] weaponprice;
    public int[] armorprice;
    public int[] throwableprice;
    public TextMeshProUGUI[] skillpriceTexts;
    public TextMeshProUGUI[] weaponpriceTexts;
    public TextMeshProUGUI[] armorpriceTexts;
    public TextMeshProUGUI[] throwablepriceTexts;
    public int vine = 10000;
    private void Awake()
    {
        skillprice = new int[4];
        weaponprice = new int[16];
        armorprice = new int[2];
        throwableprice = new int[2];
        canvas = GetComponent<Canvas>();

        skillButtonText = new TextMeshProUGUI[skillButtons.Length];
        weaponButtonsText = new TextMeshProUGUI[weaponButtons.Length];
        armorButtonsText = new TextMeshProUGUI[armorButtons.Length];
        throwableButtonsText = new TextMeshProUGUI[throwableButtons.Length];
        vineValue.text = vine.ToString();
        for (int i = 0; i < skillButtons.Length; i++)
        {
            skillButtonText[i] = skillButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        }
        for (int i = 0; i < weaponButtons.Length; i++)
        {
            weaponButtonsText[i] = weaponButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        }

        for (int i = 0; i < armorButtons.Length; i++)
        {
            armorButtonsText[i] = armorButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        }

        for (int i = 0; i < throwableButtons.Length; i++)
        {
            throwableButtonsText[i] = throwableButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        }
    }
    protected override void Start()
    {
        base.Start();
        // 데이터 배열이 비어 있고 데이터베이스 연결 테스트가 성공하면 데이터 저장
        if (ConnectionTest())
        {
            GetStoreData();
        }
        else
        {
            Debug.Log("DB 연결 테스트 실패.");
        }
        StartCoroutine(SettingStore());
    }
    private IEnumerator SettingStore()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(0.1f);
        // Call the methods sequentially
        SkillStore();
        WeaponStore();
        ArmorStore();
        GetThrowableData();
        AddListener(); // Assuming AddListner() was a typo and should be AddListener()
        yield return new WaitForSeconds(0.1f);

        yield return null;
    }
    private void AddListener()
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            int index = i;
            skillButtons[i].onClick.AddListener(() => BuySkillClick(index, skillButtons[index]));
        }
        for (int i = 0; i < weaponButtons.Length; i++)
        {
            int index = i;
            weaponButtons[i].onClick.AddListener(() => BuyWeaponClick(index, weaponButtons[index]));
        }
        for (int i = 0; i < throwableButtons.Length; i++)
        {
            int index = i;
            throwableButtons[i].onClick.AddListener(() => BuyThrowableClick(index, throwableButtons[index]));
        }
        for (int i = 0; i < armorButtons.Length; i++)
        {
            int index = i;
            armorButtons[i].onClick.AddListener(() => BuyArmorClick(index, armorButtons[index]));
        }
    }
    private void BuySkillClick(int index, Button button)
    {
        Debug.Log("Button" + index + "clicked");
        TextMeshProUGUI tp = button.GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(index + "번 아이템  : " + tp.text);
        int IDX = playerSkillInfo.classDataStringArray[index].index;
        switch (index)
        {
            case 0:
                if (vine >= skillprice[0])
                {
                    itemManager.InitializeSkill(IDX);
                    //기능 추가
                    Payment(skillprice[0]);
                }
                break;
            case 1:
                if (vine >= skillprice[1])
                {
                    //기능 추가
                    Payment(skillprice[1]);
                }
                break;
            case 2://수류탄
                if (vine >= skillprice[2])
                {
                    itemManager.InitializeSkill(IDX);
                    //기능 추가
                    Payment(skillprice[2]);
                }
                break;
            case 3:
                if (vine >= skillprice[3])
                {
                    //기능 추가
                    Payment(skillprice[3]);
                }
                break;
            default:
                Debug.Log("너 돈 없음");
                break;
        }
    }
    private void BuyWeaponClick(int index, Button button)
    {
        Debug.Log("Button" + index + "clicked");
        TextMeshProUGUI tp = button.GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(index + "번 아이템  : " + tp.text);
        int IDX = weaponData.weaponDataInfoArray[index].index;
        switch (index)
        {
            case 0:
                if (vine >= weaponprice[0])
                {
                    itemManager.InitializeGun(IDX);
                    Payment(weaponprice[0]);
                }
                break;
            case 1:
                if (vine >= weaponprice[1])
                {
                    itemManager.InitializeGun(IDX);
                    Payment(weaponprice[1]);
                }
                break;
            case 2:
                if (vine >= weaponprice[2])
                {
                    itemManager.InitializeGun(IDX);
                    Payment(weaponprice[2]);
                }
                break;
            case 3:
                if (vine >= weaponprice[3])
                {
                    itemManager.InitializeGun(IDX);
                    Payment(weaponprice[3]);
                }
                break;
            case 4:
                if (vine >= weaponprice[4])
                {
                    itemManager.InitializeGun(IDX);
                    Payment(weaponprice[4]);
                }
                break;
            case 5:
                if (vine >= weaponprice[5])
                {
                    itemManager.InitializeGun(IDX);
                    Payment(weaponprice[5]);
                }
                break;
            case 6:
                if (vine >= weaponprice[6])
                {
                    itemManager.InitializeGun(IDX);
                    Payment(weaponprice[6]);
                }
                break;
            case 7:
                if (vine >= weaponprice[7])
                {
                    itemManager.InitializeGun(IDX);
                    Payment(weaponprice[7]);
                }
                break;
            case 8:
                if (vine >= weaponprice[8])
                {
                    itemManager.InitializeGun(IDX);
                    Payment(weaponprice[8]);
                }
                break;
            case 9:
                if (vine >= weaponprice[9])
                {
                    itemManager.InitializeGun(IDX);
                    Payment(weaponprice[9]);
                }
                break;
            case 10:
                if (vine >= weaponprice[10])
                {
                    itemManager.InitializeGun(IDX);
                    Payment(weaponprice[10]);
                }
                break;
            case 11:
                if (vine >= weaponprice[11])
                {
                    itemManager.InitializeGun(IDX);
                    Payment(weaponprice[11]);
                }
                break;
            case 12:
                if (vine >= weaponprice[12])
                {
                    itemManager.InitializeGun(IDX);
                    Payment(weaponprice[12]);
                }
                break;
            case 13:
                if (vine >= weaponprice[13])
                {
                    itemManager.InitializeGun(IDX);
                    Payment(weaponprice[13]);
                }
                break;
            case 14:
                if (vine >= weaponprice[14])
                {
                    itemManager.InitializeGun(IDX);
                    Payment(weaponprice[14]);
                }
                break;
            case 15:
                if (vine >= weaponprice[15])
                {
                    itemManager.InitializeGun(IDX);
                    Payment(weaponprice[15]);
                }
                break;
            default:
                Debug.Log("너 돈 없음");
                break;
        }
    }
    private void BuyArmorClick(int index, Button button)
    {
        Debug.Log("Button" + index + "clicked");
        TextMeshProUGUI tp = button.GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(index + "번 아이템  : " + tp.text);
        switch (index)
        {
            case 0:

                if (vine >= armorprice[0])
                {
                    //기능 추가
                    if(roundManager.player.armor < 25)
                    {
                        roundManager.player.armor = 25;
                        roundManager.player.armorText.text = roundManager.player.armor.ToString();
                        Payment(armorprice[0]);
                    }
                    
                    
                }
                break;
            case 1:
                if (vine >= armorprice[1])
                {
                    if (roundManager.player.armor < 50)
                    {
                        roundManager.player.armor = 50;
                        roundManager.player.armorText.text = roundManager.player.armor.ToString();
                        Payment(armorprice[1]);
                    }
                    //기능 추가
                    
                }
                break;
            default:
                Debug.Log("너 돈 없음");
                break;
        }
    }
    private void BuyThrowableClick(int index, Button button)
    {
        Debug.Log("Button" + index + "clicked");
        TextMeshProUGUI tp = button.GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(index + "번 아이템  : " + tp.text);

        /*
         asdasdasdasdas
         */
        int IDX = throwableData.throwableDataInfoArray[index].index;
        switch (index)
        {
            case 0:
                if (vine >= throwableprice[0])
                {
                    itemManager.IntializeGrenade(IDX);
                    //기능 추가
                    Payment(throwableprice[0]);
                }
                break;
            case 1:
                if (vine >= throwableprice[1])
                {
                    itemManager.IntializeGrenade(IDX);
                    //기능 추가
                    Payment(throwableprice[1]);
                }
                break;
            default:
                Debug.Log("너 돈 없음");
                break;
        }
    }
    public void SkillStore()
    {
        for (int j = 0; j < playerSkillInfo.skills.Length - 1; j++)
        {
            var skillIndex = playerSkillInfo.skills[j];
            bool found = false;
            for (int i = 0; i < stringDataArray.Length; i++)
            {
                if (stringDataArray[i].index == skillIndex)
                {

                    skillButtonText[j].text = stringDataArray[i].stringInfo;

                    found = true;
                    break;  // 일치하는 항목을 찾으면 내부 루프 종료
                }
            }
            if (!found)
            {
                skillButtonText[j].text = "찾을수 없음"; // 예시
            }
        }
        for (int j = 0; j < playerSkillInfo.skills.Length - 1; j++)
        {
            for (int i = 0; i < storesDataArray.Length; i++)
            {

                if (storesDataArray[i].item_index == playerSkillInfo.prices[j])
                {
                    skillprice[j] = storesDataArray[i].item_price;
                    skillpriceTexts[j].text = skillprice[j].ToString();
                }
            }
        }
    }
    private void WeaponStore()
    {
        for (int i = 0; i < weaponData.weaponDataInfoArray.Length; i++)
        {
            bool found = false;
            for (int j = 0; j < storesDataArray.Length; j++)
            {
                if (weaponData.weaponDataInfoArray[i].index == storesDataArray[j].item_index)
                {
                    // 일치하는 항목을 찾았을 때 해당 weaponButtonsText에만 설정하고 루프 종료
                    weaponButtonsText[i].text = weaponData.weaponDataInfoArray[i].weapon_Name;
                    found = true;
                    break;
                }
            }
            // 만약 일치하는 항목을 찾지 못했다면 초기화할 수 있는 로직 추가
            if (!found)
            {
                //weaponButtonsText[i].text = "찾을수 없음"; // 혹은 다른 초기화 로직을 추가할 수 있음
            }
        }
        for (int j = 0; j < weaponData.weaponDataInfoArray.Length; j++)
        {
            for (int i = 0; i < storesDataArray.Length; i++)
            {
                if (storesDataArray[i].item_index == weaponData.weaponDataInfoArray[j].index)
                {
                    weaponprice[j] = storesDataArray[i].item_price;
                    weaponpriceTexts[j].text = weaponprice[j].ToString();
                }
            }
        }
    }
    private void ArmorStore()
    {
        for (int i = 0; i < armorData.armorDataInfoArray.Length; i++)
        {
            bool found = false;
            for (int j = 0; j < storesDataArray.Length; j++)
            {
                if (armorData.armorDataInfoArray[i].index == storesDataArray[j].item_index)
                {
                    armorButtonsText[i].text = armorData.armorDataInfoArray[i].armor_Name;
                    found = true;
                    break; // 일치하는 항목을 찾았으므로 내부 루프 종료
                }
            }
            // 일치하는 항목을 찾지 못했을 경우 초기화 로직 추가 가능
            if (!found)
            {
                // armorButtonsText[i].text = ""; // 혹은 다른 초기화 로직을 추가할 수 있음
            }
        }
        for (int j = 0; j < armorData.armorDataInfoArray.Length; j++)
        {
            for (int i = 0; i < storesDataArray.Length; i++)
            {
                if (storesDataArray[i].item_index == armorData.armorDataInfoArray[j].index)
                {
                    armorprice[j] = storesDataArray[i].item_price;
                    armorpriceTexts[j].text = armorprice[j].ToString();
                }
            }
        }
    }
    private void GetThrowableData()
    {
        for (int i = 0; i < throwableData.throwableDataInfoArray.Length; i++)
        {

            bool found = false;
            for (int j = 0; j < storesDataArray.Length; j++)
            {

                if (throwableData.throwableDataInfoArray[i].index == storesDataArray[j].item_index)
                {

                    throwableButtonsText[i].text = throwableData.throwableDataInfoArray[i].throwables_Name;
                    found = true;
                    break; // 일치하는 항목을 찾았으므로 내부 루프 종료
                }
            }
            // 일치하는 항목을 찾지 못했을 경우 초기화 로직 추가 가능
            if (!found)
            {
                // armorButtonsText[i].text = ""; // 혹은 다른 초기화 로직을 추가할 수 있음
            }
        }
        for (int j = 0; j < throwableData.throwableDataInfoArray.Length; j++)
        {
            for (int i = 0; i < storesDataArray.Length; i++)
            {
                if (storesDataArray[i].item_index == throwableData.throwableDataInfoArray[j].index)
                {
                    throwableprice[j] = storesDataArray[i].item_price;
                    throwablepriceTexts[j].text = throwableprice[j].ToString();
                }
            }
        }
    }
    private void GetStoreData()
    {
        string query = "SELECT * FROM `storetable`";
        List<StoresData> GetData = new List<StoresData>();
        try
        {
            using (MySqlConnection conn = new MySqlConnection(conStr))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        storesDatas.index = reader.GetInt32(0);
                        storesDatas.item_index = reader.GetInt32(1);
                        storesDatas.buy_currency = reader.GetInt32(2);
                        storesDatas.item_price = reader.GetInt32(3);
                        storesDatas.max_stock = reader.GetInt32(4);

                        GetData.Add(storesDatas);
                        /*Debug.Log($"index: {storesDatas.index}, item_index: {storesDatas.item_index}, buy_currency: {storesDatas.buy_currency}, " +
                                  $"item_price: {storesDatas.item_price}, max_stock: {storesDatas.max_stock}");*/
                    }
                }
            }
            storesDataArray = GetData.ToArray();
        }
        catch (Exception e)
        {
            Debug.Log("쿼리 오류: " + e.Message);
        }
        //디버깅용
        //PrintAllWeaponData();
    }
    //돈 지급 함수
    private void Compensation(int idx)
    {
        switch (idx)
        {
            //0 : 킬, 1 : 폭설, 2 : 승리, 3 : 1패, 4 : 2연패, 5 : 3연패, 6 : 세이브(생존패배), 7: 연장라운드
            case 0:
                vine += 300;
                break;
            case 1:
                vine += 400;
                break;
            case 2:
                vine += 3500;
                break;
            case 3:
                vine += 2400;
                break;
            case 4:
                vine += 2900;
                break;
            case 5:
                vine += 3400;
                break;
            case 6:
                vine += 1500;
                break;
            case 7:
                vine += 6000;
                break;
            default:
                break;
        }
        // Check if vine exceeds 16000 and adjust if necessary
        if (vine > 16000)
        {
            vine = 16000;
        }
        // Update the vineValue text
        vineValue.text = vine.ToString();
    }
    private void Payment(int price)
    {
        vine -= price;
        vineValue.text = vine.ToString();
    }
}