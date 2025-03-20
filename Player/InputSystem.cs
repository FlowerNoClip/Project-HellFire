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
    private int[] keyBindings = new int[Enum.GetNames(typeof(KeyBindings)).Length]; // �������� ���̸�ŭ �迭 ����

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
                    Debug.Log($"{(KeyBindings)i} Ű�� ���Ƚ��ϴ�.");

                    // �� Ű ���ε��� ���� ���� ó��
                    switch ((KeyBindings)i)
                    {
                        case KeyBindings.Skill1:
                            IM2.SetSkill1();
                            break;
                        case KeyBindings.Skill2:
                            IM2.SetSkill2();
                            // Skill2 Ű�� ������ �� ������ �ڵ�
                            break;
                        case KeyBindings.Ult:
                            // Ult Ű�� ������ �� ������ �ڵ�
                            break;
                        case KeyBindings.Jump:
                            pCon.Jump();
                            // Jump Ű�� ������ �� ������ �ڵ�
                            break;
                        case KeyBindings.Sit:
                            pCon.Crouch();
                            // Sit Ű�� ������ �� ������ �ڵ�
                            break;
                        case KeyBindings.MainWeapon:
                            // MainWeapon Ű�� ������ �� ������ �ڵ�
                            IM2.SetMainGun();

                            break;
                        case KeyBindings.SubWeapon:
                            // SubWeapon Ű�� ������ �� ������ �ڵ�
                            IM2.SetSubGun();
                            break;
                        case KeyBindings.KnifeWeapon:
                            // KnifeWeapon Ű�� ������ �� ������ �ڵ�
                            IM2.SetKnife();
                            break;
                        case KeyBindings.Frag:
                            // Frag Ű�� ������ �� ������ �ڵ�
                            IM2.SetGrenade();
                            break;
                        case KeyBindings.Magnus:
                            // Magnus Ű�� ������ �� ������ �ڵ�
                            break;
                        case KeyBindings.NextWeapon:
                            // NextWeapon Ű�� ������ �� ������ �ڵ�
                            break;
                        case KeyBindings.BeforeWeapon:
                            // BeforeWeapon Ű�� ������ �� ������ �ڵ�
                            break;
                        case KeyBindings.DropWeapon:
                            // DropWeapon Ű�� ������ �� ������ �ڵ�
                            break;
                        case KeyBindings.SearchWeapon:
                            // SearchWeapon Ű�� ������ �� ������ �ڵ�
                            break;
                        case KeyBindings.Interaction:
                            // Interaction Ű�� ������ �� ������ �ڵ�
                            break;
                        case KeyBindings.Ping:
                            // Ping Ű�� ������ �� ������ �ڵ�
                            break;
                        case KeyBindings.Store:
                            // Store Ű�� ������ �� ������ �ڵ�
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
                            // Tip Ű�� ������ �� ������ �ڵ�
                            break;
                        case KeyBindings.Setting:
                            // Setting Ű�� ������ �� ������ �ڵ�
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
                            // Sit Ű�� ������ �� ������ �ڵ�
                            break;
                        case KeyBindings.Walk:
                            pCon.StopWalk();
                            // Walk Ű�� ������ �� ������ �ڵ�
                            break;
                    }
                }
                if (Input.GetKey((KeyCode)keyBindings[i]))
                {
                    switch ((KeyBindings)i)
                    {
                        case KeyBindings.Plant:
                            // Plant Ű�� ���������� �� ������ �ڵ�
                            break;
                        case KeyBindings.Interaction:
                            // Interaction Ű�� ���������� �� ������ �ڵ�
                            break;
                        case KeyBindings.Walk:
                            pCon.Walk();
                            // Walk Ű�� ���������� �� ������ �ڵ�
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
        // ��� Ű ���ε��� ���� �Է� ó��
     
    }





    // ���ڿ��� KeyCode�� ��ȯ�ϴ� �Լ�
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
        // baseKeyDataStringArray�� ��ȸ�ϸ� �� �������� baseKey ���� ����ϰ�, �ش� ���� ������ ��ȯ�Ͽ� �迭�� ����
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
        // keyString�� �ش��ϴ� KeyCode ���� ã���ϴ�.
        KeyCode keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyString);
        // �ش� KeyCode�� �������� ��ȯ�մϴ�.
        return (int)keyCode;
    }

}
