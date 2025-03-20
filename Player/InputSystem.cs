using FishNet.Component.Spawning;
using System;
using UnityEngine;
using UnityEngine.UI;

public class InputSystem : BaseKeyTable
{
    [SerializeField] private PlayerController pCon;
    [SerializeField] private ItemManager2 IM2;
    public ItemManager2 _IM2
    {
        get { return IM2; }
        set { IM2 = value; }
    }
    [SerializeField] private GameObject store;
    [SerializeField] private Store _store;
    [SerializeField] private RoundManager roundManager;
    [SerializeField] private Image Scope;
    


    public enum KeyBindings
    {
        Skill1,
        Skill2,
        Ult,
        Walk,
        Jump,
        Sit,
        MainWeapon,
        SubWeapon,
        KnifeWeapon,
        Frag,
        Magnus,
        NextWeapon,
        BeforeWeapon,
        DropWeapon,
        SearchWeapon,
        Interaction,
        Plant,
        Ping,
        Store,
        Map,
        MapHold,
        Tip,
        Setting
    }
    private int[] keyBindings = new int[Enum.GetNames(typeof(KeyBindings)).Length]; // 열거형의 길이만큼 배열 생성

    private void Awake()
    {
        _store = Store.Instance;
        store = _store.gameObject;
        store.SetActive(false);
        roundManager = RoundManager.Instance;
    }
    private void Update()
    {
        if (pCon.playerCamera != null)
        {
            for (int i = 0; i < (int)keyBindings.Length; i++)
            {
                if (Input.GetKeyDown((KeyCode)keyBindings[i]))
                {
                    Debug.Log($"{(KeyBindings)i} 키가 눌렸습니다.");

                    // 각 키 바인딩에 따른 동작 처리
                    switch ((KeyBindings)i)
                    {
                        case KeyBindings.Skill1:
                            IM2.SetSkill1();
                            break;
                        case KeyBindings.Skill2:
                            IM2.SetSkill2();
                            // Skill2 키가 눌렸을 때 실행할 코드
                            break;
                        case KeyBindings.Ult:
                            // Ult 키가 눌렸을 때 실행할 코드
                            break;
                        case KeyBindings.Jump:
                            pCon.Jump();
                            // Jump 키가 눌렸을 때 실행할 코드
                            break;
                        case KeyBindings.Sit:
                            pCon.Crouch();
                            // Sit 키가 눌렸을 때 실행할 코드
                            break;
                        case KeyBindings.MainWeapon:
                            // MainWeapon 키가 눌렸을 때 실행할 코드
                            IM2.SetMainGun();

                            break;
                        case KeyBindings.SubWeapon:
                            // SubWeapon 키가 눌렸을 때 실행할 코드
                            IM2.SetSubGun();
                            break;
                        case KeyBindings.KnifeWeapon:
                            // KnifeWeapon 키가 눌렸을 때 실행할 코드
                            IM2.SetKnife();
                            break;
                        case KeyBindings.Frag:
                            // Frag 키가 눌렸을 때 실행할 코드
                            IM2.SetGrenade();
                            break;
                        case KeyBindings.Magnus:
                            // Magnus 키가 눌렸을 때 실행할 코드
                            break;
                        case KeyBindings.NextWeapon:
                            // NextWeapon 키가 눌렸을 때 실행할 코드
                            break;
                        case KeyBindings.BeforeWeapon:
                            // BeforeWeapon 키가 눌렸을 때 실행할 코드
                            break;
                        case KeyBindings.DropWeapon:
                            // DropWeapon 키가 눌렸을 때 실행할 코드
                            break;
                        case KeyBindings.SearchWeapon:
                            // SearchWeapon 키가 눌렸을 때 실행할 코드
                            break;
                        case KeyBindings.Interaction:
                            // Interaction 키가 눌렸을 때 실행할 코드
                            break;
                        case KeyBindings.Ping:
                            // Ping 키가 눌렸을 때 실행할 코드
                            break;
                        case KeyBindings.Store:
                            // Store 키가 눌렸을 때 실행할 코드
/*                            if (!roundManager.buyTime)
                            {
                                break;
                            }*/
                            if (store.activeSelf)
                            {
                                store.SetActive(false);
                                Cursor.lockState = CursorLockMode.Locked;
                                Cursor.visible = false;
                                pCon.shopCont = false;
                            }
                            else if(!store.activeSelf)
                            {
                                store.SetActive(true);
                                Cursor.lockState = CursorLockMode.None;
                                Cursor.visible = true;
                                pCon.shopCont = true;
                            }

                            break;
                        case KeyBindings.Tip:
                            // Tip 키가 눌렸을 때 실행할 코드
                            break;
                        case KeyBindings.Setting:
                            // Setting 키가 눌렸을 때 실행할 코드
                            break;
                        default:
                            Debug.LogWarning($"Unhandled key binding: {(KeyBindings)i}");
                            break;
                    }
                }
                if (Input.GetMouseButton(0))
                {
                    
                        IM2.AttackCurrentWeapon();


                }
                if (Input.GetMouseButtonDown(1))
                {
                    IM2.ZoomCurrentWeapon();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    //mainGunController.StopShooting();
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    IM2.ReloadCurrentWeapon();
                }


                if (Input.GetKeyUp((KeyCode)keyBindings[i]))
                {
                    switch ((KeyBindings)i)
                    {
                        case KeyBindings.Sit:
                            pCon.StandUp();
                            // Sit 키가 떼졌을 때 실행할 코드
                            break;
                        case KeyBindings.Walk:
                            pCon.StopWalk();
                            // Walk 키가 떼졌을 때 실행할 코드
                            break;
                    }
                }
                if (Input.GetKey((KeyCode)keyBindings[i]))
                {
                    switch ((KeyBindings)i)
                    {
                        case KeyBindings.Plant:
                            // Plant 키가 눌려져있을 때 실행할 코드
                            break;
                        case KeyBindings.Interaction:
                            // Interaction 키가 눌려져있을 때 실행할 코드
                            break;
                        case KeyBindings.Walk:
                            pCon.Walk();
                            // Walk 키가 눌려져있을 때 실행할 코드
                            break;

                    }
                }
                if (Input.GetKey(KeyCode.Tab))
                {
                    if (!roundManager.tab.activeSelf)
                    {
                        roundManager.tab.SetActive(true);
                    }
                }
                else if (Input.GetKeyUp(KeyCode.Tab))
                {
                    if (roundManager.tab.activeSelf)
                    {
                        roundManager.tab.SetActive(false);
                    }
                }
            }
        }
        // 모든 키 바인딩에 대해 입력 처리
     
    }





    // 문자열을 KeyCode로 변환하는 함수
    protected override void Start()
    {
        base.Start();
        
        
        KeyBinding();

        for(int i = 0; i < keyBindings.Length; i++)
        {
            //Debug.Log(keyBindings[i]);
        }
    }

    public void KeyBinding()
    {
        // baseKeyDataStringArray를 순회하며 각 데이터의 baseKey 값을 출력하고, 해당 값을 정수로 변환하여 배열에 저장
        int[] keycode = new int[baseKeyDataStringArray.Length];
        for (int i = 0; i < baseKeyDataStringArray.Length; i++)
        {
            var data = baseKeyDataStringArray[i];
            //Debug.Log($"BaseKey: {data.baseKey}");
            keycode[i] = ConvertStringToKeyCodeInt(data.baseKey);
            //Debug.Log(keycode[i]);
        }
        keyBindings[(int)KeyBindings.Skill1] = keycode[0];
        keyBindings[(int)KeyBindings.Skill2] = keycode[1];
        keyBindings[(int)KeyBindings.Ult] = keycode[2];
        keyBindings[(int)KeyBindings.Walk] = keycode[7];
        keyBindings[(int)KeyBindings.Jump] = keycode[8];
        keyBindings[(int)KeyBindings.Sit] = keycode[9];
        keyBindings[(int)KeyBindings.MainWeapon] = keycode[10];
        keyBindings[(int)KeyBindings.SubWeapon] = keycode[11];
        keyBindings[(int)KeyBindings.KnifeWeapon] = keycode[12];
        keyBindings[(int)KeyBindings.Frag] = keycode[13];
        keyBindings[(int)KeyBindings.Magnus] = keycode[14];
        keyBindings[(int)KeyBindings.NextWeapon] = keycode[15];
        keyBindings[(int)KeyBindings.BeforeWeapon] = keycode[16];
        keyBindings[(int)KeyBindings.DropWeapon] = keycode[17];
        keyBindings[(int)KeyBindings.SearchWeapon] = keycode[18];
        keyBindings[(int)KeyBindings.Interaction] = keycode[19];
        keyBindings[(int)KeyBindings.Plant] = keycode[20];
        keyBindings[(int)KeyBindings.Ping] = keycode[21];
        keyBindings[(int)KeyBindings.Store] = keycode[22];
        keyBindings[(int)KeyBindings.Map] = keycode[23];
        keyBindings[(int)KeyBindings.MapHold] = keycode[24];
        keyBindings[(int)KeyBindings.Tip] = keycode[25];
        keyBindings[(int)KeyBindings.Setting] = keycode[26];
    }

    public int ConvertStringToKeyCodeInt(string keyString)
    {
        // keyString에 해당하는 KeyCode 값을 찾습니다.
        KeyCode keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyString);
        // 해당 KeyCode의 정수값을 반환합니다.
        return (int)keyCode;
    }

}
