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
        // 게임 시작 전처리 (예: 화면 닫기 등)
        CloseAllScreens();

        // 게임 시작 로직 실행
        roundManager.RespawnRpc();

        // 클라이언트에게 게임 시작 이벤트 전송
        RpcStartGame();
        //roundManager.BuyRound(10);
        
        roundManager.RoundStart(600);

    }

    // 모든 클라이언트에게 게임 시작 이벤트를 전송하는 RPC 메서드
    [ObserversRpc]
    void RpcStartGame()
    {
        // 클라이언트에서 실행할 동작 설정 (예: UI 숨기기)
        menuUI.SetActive(false);
        roundManager.info.SetActive(true);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // TnicknameText의 각 요소를 roundManager.tNameTXT의 동일한 인덱스에 할당
        for (int i = 0; i < TnicknameText.Length; i++)
        {
            if (i < roundManager.tNameTXT.Length)
            {
                roundManager.tNameTXT[i].text = TnicknameText[i].text;
            }
        }

        // CTnicknameText의 각 요소를 roundManager.ctNameTXT의 동일한 인덱스에 할당
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

