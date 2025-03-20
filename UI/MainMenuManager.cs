using System;
using System.Collections;
using FishNet.Component.Spawning;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : NetworkBehaviour 
{
    public static MainMenuManager Instance { get; set; }

    private bool isCursorLocked = false;
    [SerializeField] private GameObject menuScreen, lobbyScreen;
    [SerializeField] private TMP_InputField lobbyInput;
    [SerializeField] private TextMeshProUGUI lobbyTitle, lobbyIDText;
    [SerializeField] private TMP_InputField lobbytitletext;
    [SerializeField] private Button startGameButton;
    public TextMeshProUGUI[] TnicknameText;
    public TextMeshProUGUI[] CTnicknameText;
    public PlayerNetwork playerNetwork;
    public PlayerController playerController;
    public RoundManager roundManager;
    public GameObject menuUI;
    //private void Awake() => Instance = this;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        OpenMainMenu();
        roundManager = RoundManager.Instance;
    }

    public void CreateLobby()
    {
        BootstrapManager.CreateLobby();
    }

    public void OpenMainMenu()
    {
        CloseAllScreens();
        menuScreen.SetActive(true);
    }

    public void OpenLobby()
    {
        CloseAllScreens();
        lobbyScreen.SetActive(true);
    }
    public static void LobbyEntered(string lobbyName, bool isHost)
    {
        Instance.lobbyTitle.text = lobbyName;
        Instance.lobbytitletext.text = BootstrapManager.CurrentLobbyID.ToString();
        Instance.startGameButton.gameObject.SetActive(isHost);
        Instance.lobbyIDText.text = BootstrapManager.CurrentLobbyID.ToString();
        Instance.OpenLobby();
    }

    void CloseAllScreens()
    {
        menuScreen.SetActive(false);
        lobbyScreen.SetActive(false);
    }

    public void JoinLobby()
    {
        CSteamID steamID = new CSteamID(Convert.ToUInt64(lobbyInput.text));

        BootstrapManager.JoinByID(steamID);
    }

    public void LeaveLobby()
    {
        OpenMainMenu();
        StartCoroutine(LeaveLobys());
    }

    public IEnumerator LeaveLobys()
    {
        playerNetwork.LeaveRoom();
        yield return new WaitForSeconds(0.3f);
        BootstrapManager.LeaveLobby();
        yield break;
    }
    [ServerRpc(RequireOwnership = false)]
    public void StartGame()
    {
        // ���� ���� ��ó�� (��: ȭ�� �ݱ� ��)
        CloseAllScreens();

        // ���� ���� ���� ����
        roundManager.RespawnRpc();

        // Ŭ���̾�Ʈ���� ���� ���� �̺�Ʈ ����
        RpcStartGame();
        //roundManager.BuyRound(10);
        
        roundManager.RoundStart(600);

    }

    // ��� Ŭ���̾�Ʈ���� ���� ���� �̺�Ʈ�� �����ϴ� RPC �޼���
    [ObserversRpc]
    void RpcStartGame()
    {
        // Ŭ���̾�Ʈ���� ������ ���� ���� (��: UI �����)
        menuUI.SetActive(false);
        roundManager.info.SetActive(true);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // TnicknameText�� �� ��Ҹ� roundManager.tNameTXT�� ������ �ε����� �Ҵ�
        for (int i = 0; i < TnicknameText.Length; i++)
        {
            if (i < roundManager.tNameTXT.Length)
            {
                roundManager.tNameTXT[i].text = TnicknameText[i].text;
            }
        }

        // CTnicknameText�� �� ��Ҹ� roundManager.ctNameTXT�� ������ �ε����� �Ҵ�
        for (int i = 0; i < CTnicknameText.Length; i++)
        {
            if (i < roundManager.ctNameTXT.Length)
            {
                roundManager.ctNameTXT[i].text = CTnicknameText[i].text;
            }
        }
    }

    public void Button1()
    {
        DBHolder.Instance.GetComponent<PlayerSkillInfo>().selectclass = 1;
        DBHolder.Instance.GetComponent<PlayerSkillInfo>().SelectGetSkill(DBHolder.Instance.GetComponent<PlayerSkillInfo>().selectclass);
        Store.Instance.GetComponentInChildren<StoreData>().SkillStore();
    }    
    public void Button2()
    {
        DBHolder.Instance.GetComponent<PlayerSkillInfo>().selectclass = 2;
        DBHolder.Instance.GetComponent<PlayerSkillInfo>().SelectGetSkill(DBHolder.Instance.GetComponent<PlayerSkillInfo>().selectclass);
        Store.Instance.GetComponentInChildren<StoreData>().SkillStore();
    }    
    public void Button3()
    {
        DBHolder.Instance.GetComponent<PlayerSkillInfo>().selectclass = 3;
        DBHolder.Instance.GetComponent<PlayerSkillInfo>().SelectGetSkill(DBHolder.Instance.GetComponent<PlayerSkillInfo>().selectclass);
        Store.Instance.GetComponentInChildren<StoreData>().SkillStore();
    }    
    public void Button4()
    {
        DBHolder.Instance.GetComponent<PlayerSkillInfo>().selectclass = 4;
        DBHolder.Instance.GetComponent<PlayerSkillInfo>().SelectGetSkill(DBHolder.Instance.GetComponent<PlayerSkillInfo>().selectclass);
        Store.Instance.GetComponentInChildren<StoreData>().SkillStore();
    }   
    public void Button5()
    {
        DBHolder.Instance.GetComponent<PlayerSkillInfo>().selectclass = 5;
        DBHolder.Instance.GetComponent<PlayerSkillInfo>().SelectGetSkill(DBHolder.Instance.GetComponent<PlayerSkillInfo>().selectclass);
        Store.Instance.GetComponentInChildren<StoreData>().SkillStore();
    }

}

