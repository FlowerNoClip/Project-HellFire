using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Object;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FishNet.Component.Spawning
{

    /// <summary>
    /// Spawns a player object for clients when they connect.
    /// Must be placed on or beneath the NetworkManager object.
    /// </summary>
    [AddComponentMenu("FishNet/Component/PlayerSpawner")]
    public class RoundManager : NetworkBehaviour
    {
        public event Action<NetworkObject> OnSpawned;
        [SerializeField] private NetworkObject _playerPrefab;
        [SerializeField] private bool _addToDefaultScene = true;
        public static List<PlayerNetwork> playerNetworks = new List<PlayerNetwork>();
        public Transform[] TSpawns = new Transform[0];
        public Transform[] CTSpawns = new Transform[0];
        public BootstrapManager bootstrapManager;
        public bool buyTime = true;
        public int maxRound = 20;
        public int curRound = 1;

        public int TKillCount = 0;
        public int CTKillCount = 0;
        public bool isPlay = false;
        public int[] Tlose; // Terrorist 팀의 패배 기록
        public int[] CTlose; // Counter-Terrorist 팀의 패배 기록

        public bool T3ConsecutiveLosses; // Terrorist 팀의 3연속 패배 여부
        public bool CT3ConsecutiveLosses; // Counter-Terrorist 팀의 3연속 패배 여부
        public readonly SyncTimer _timeRemaining = new SyncTimer();
        public TextMeshProUGUI[] killTexts;
        [SerializeField] private TextMeshProUGUI[] timerText;
        [SerializeField] public TextMeshProUGUI[] bulletText;
        public Store store;
        public StoreData storeData;
        public GameObject info;
        public TextMeshProUGUI hpText;
        public TextMeshProUGUI armorText;
        public GameObject tab;
        public Player player;
        [SerializeField] public TextMeshProUGUI[] tNameTXT;
        [SerializeField] public TextMeshProUGUI[] ctNameTXT;
        [SerializeField] public TextMeshProUGUI[] tKillTXT;
        [SerializeField] public TextMeshProUGUI[] tDeathTXT;
        [SerializeField] public TextMeshProUGUI[] ctKillTXT;
        [SerializeField] public TextMeshProUGUI[] ctDeathTXT;
        public static RoundManager Instance { get; private set; }

        public readonly SyncDictionary<string, PlayerStats> leaderboardData = new SyncDictionary<string, PlayerStats>();

        public override void OnStartServer()
        {
            base.OnStartServer();
            leaderboardData.OnChange += OnLeaderboardChanged;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            leaderboardData.OnChange += OnLeaderboardChanged;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            leaderboardData.OnChange -= OnLeaderboardChanged;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            leaderboardData.OnChange -= OnLeaderboardChanged;
        }

        private void Awake()
        {
            Syncs(buyTime, maxRound, curRound, TKillCount, CTKillCount, isPlay);
            info.SetActive(false);
            _timeRemaining.OnChange += _timeRemaining_OnChange;
            _timeRemaining.StartTimer(500);
            bootstrapManager = BootstrapManager.instance;

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            store = Store.Instance;
            storeData = store.GetComponentInChildren<StoreData>();
            storeData.roundManager = this;
        }

        [ObserversRpc]
        public void SyncList(PlayerNetwork pnetwork)
        {
            playerNetworks.Add(pnetwork);
        }
        #region Private.
        /// <summary>
        /// NetworkManager on this object or within this objects parents.
        /// </summary>
        private NetworkManager _networkManager;
        /// <summary>
        /// Next spawns to use.
        /// </summary>
        private int _nextSpawn;

        #endregion
        private void Start()
        {
            InitializeOnce();
            killTexts[0].text = TKillCount.ToString();
            killTexts[1].text = CTKillCount.ToString();

            Tlose = new int[3];
            CTlose = new int[3];

            for (int i = 0; i < Tlose.Length; i++)
            {
                Tlose[i] = 0;
                CTlose[i] = 0;
            }
        }

        private void Update()
        {
            _timeRemaining.Update(Time.deltaTime);


            int totalSeconds = Mathf.CeilToInt(_timeRemaining.Remaining); // 남은 시간을 올림하여 정수 초 단위로 변환
            int minutes = totalSeconds / 60; // 분 단위
            int seconds = totalSeconds % 60; // 초 단위

            // 텍스트 업데이트
            timerText[0].text = minutes.ToString("0"); // 분을 표시하는 텍스트
            timerText[1].text = seconds.ToString("00"); // 초를 표시하는 텍스트

            if (_timeRemaining.Remaining <= 0 || (TKillCount == 50 || CTKillCount == 50))
            {
                if (TKillCount > CTKillCount)
                {
                    Debug.LogError("T win");
                    //bootstrapManager.GoToMenuV("BootstapManger");
                }
                else
                {
                    Debug.LogError("CT win");
                    //bootstrapManager.GoToMenuV("BootstapManger");
                }
            }
        }


        private void OnDestroy()
        {
            if (_networkManager != null)
                _networkManager.SceneManager.OnClientLoadedStartScenes -= SceneManager_OnClientLoadedStartScenes;
        }

        public void InitializeOnce()
        {
            _networkManager = InstanceFinder.NetworkManager;
            if (_networkManager == null)
            {
                Debug.LogWarning($"PlayerSpawner on {gameObject.name} cannot work as NetworkManager wasn't found on this object or within parent objects.");
                return;
            }

            _networkManager.SceneManager.OnClientLoadedStartScenes += SceneManager_OnClientLoadedStartScenes;
        }

        public void DebugLeaderboardData()
        {
            Debug.Log($"Leaderboard Data Count: {leaderboardData.Count}");
            foreach (var kvp in leaderboardData)
            {
                Debug.Log($"Player: {kvp.Key}, Kills: {kvp.Value.kills}, Deaths: {kvp.Value.deaths}");
            }
        }



        private void SceneManager_OnClientLoadedStartScenes(NetworkConnection conn, bool asServer)
        {
            if (!asServer)
                return;
            if (_playerPrefab == null)
            {
                Debug.LogWarning($"Player prefab is empty and cannot be spawned for connection {conn.ClientId}.");
                return;
            }

            Vector3 position;
            Quaternion rotation;
            SetSpawn(_playerPrefab.transform, out position, out rotation);


            NetworkObject nob = _networkManager.GetPooledInstantiated(_playerPrefab, position, rotation, true);
            _networkManager.ServerManager.Spawn(nob, conn);

            //If there are no global scenes 
            if (_addToDefaultScene)
                _networkManager.SceneManager.AddOwnerToDefaultScene(nob);

            OnSpawned?.Invoke(nob);
        }

        private void SetSpawn(Transform prefab, out Vector3 pos, out Quaternion rot)
        {
            //No spawns specified.
            if (TSpawns.Length == 0)
            {
                SetSpawnUsingPrefab(prefab, out pos, out rot);
                return;
            }

            Transform result = TSpawns[_nextSpawn];
            if (result == null)
            {
                SetSpawnUsingPrefab(prefab, out pos, out rot);
            }
            else
            {
                pos = result.position;
                rot = result.rotation;
            }

            //Increase next spawn and reset if needed.
            _nextSpawn++;
            if (_nextSpawn >= TSpawns.Length)
                _nextSpawn = 0;
        }




        private void SetSpawnUsingPrefab(Transform prefab, out Vector3 pos, out Quaternion rot)
        {
            pos = prefab.position;
            rot = prefab.rotation;
        }

        [ServerRpc(RequireOwnership = false)]
        public void RespawnRpc()
        {
            RespawnPlayers();
        }

        [ObserversRpc]
        public void RespawnPlayers()
        {
            for (int i = 0; i < playerNetworks.Count; i++)
            {
                Vector3 spawnPosition;
                Quaternion spawnRotation;

                if (playerNetworks[i].teamCode)
                {
                    spawnPosition = TSpawns[i % TSpawns.Length].position;
                    spawnRotation = TSpawns[i % TSpawns.Length].rotation;
                }
                else
                {
                    spawnPosition = CTSpawns[i % CTSpawns.Length].position;
                    spawnRotation = CTSpawns[i % CTSpawns.Length].rotation;
                }

                playerNetworks[i].transform.position = spawnPosition;
                playerNetworks[i].transform.rotation = spawnRotation;
            }

        }

        public void RoundStart(float _time)
        {

            isPlay = true;
            _timeRemaining.StartTimer(_time, true);
            //buyTime = false;
            Syncs(buyTime, maxRound, curRound, TKillCount, CTKillCount, isPlay);
        }

        public void SyncsPublic()
        {
            Syncs(buyTime, maxRound, curRound, TKillCount, CTKillCount, isPlay);
        }


        [ObserversRpc]
        private void Syncs(bool _buyTime, int _maxRound, int _curRound, int _TwinRound, int _CTwinRound, bool _isPlay)
        {
            buyTime = _buyTime;
            maxRound = _maxRound;
            curRound = _curRound;
            TKillCount = _TwinRound;
            CTKillCount = _CTwinRound;
            isPlay = _isPlay;

            killTexts[0].text = TKillCount.ToString();
            killTexts[1].text = CTKillCount.ToString();
        }

        [ServerRpc(RequireOwnership = false)]
        public void UpdateLeaderboard()
        {
            foreach (var player in playerNetworks)
            {
                Player playerComponent = player.GetComponent<Player>();
                if (playerComponent != null)
                {
                    leaderboardData[player.nickname] = new PlayerStats(playerComponent.myKill.Value, playerComponent.myDeath.Value);
                }
            }

            DebugLeaderboardData();
        }
        [ObserversRpc]
        private void OnLeaderboardChanged(SyncDictionaryOperation op, string key, PlayerStats value, bool asServer)
        {
            Debug.Log($"Leaderboard changed: Operation {op}, Key {key}, Kills {value.kills}, Deaths {value.deaths}, AsServer {asServer}");
            UpdateLeaderboardUI();
        }
        [ObserversRpc]
        private void UpdateLeaderboardUI()
        {
            Debug.Log("Updating Leaderboard UI");
            var allPlayers = leaderboardData.ToList();
            var tPlayers = allPlayers.Where(p => playerNetworks.Any(pn => pn.nickname == p.Key && pn.teamCode)).ToList();
            var ctPlayers = allPlayers.Where(p => playerNetworks.Any(pn => pn.nickname == p.Key && !pn.teamCode)).ToList();

            var sortedTPlayers = tPlayers.OrderByDescending(kvp => kvp.Value.kills).ToList();
            var sortedCTPlayers = ctPlayers.OrderByDescending(kvp => kvp.Value.kills).ToList();

            // T 팀 UI 업데이트
            for (int i = 0; i < tNameTXT.Length; i++)
            {
                if (i < sortedTPlayers.Count)
                {
                    var player = sortedTPlayers[i];
                    if (tNameTXT[i] != null) tNameTXT[i].text = player.Key;
                    if (tKillTXT[i] != null) tKillTXT[i].text = player.Value.kills.ToString();
                    if (tDeathTXT[i] != null) tDeathTXT[i].text = player.Value.deaths.ToString();
                    Debug.Log($"T Player {player.Key}: Kills {player.Value.kills}, Deaths {player.Value.deaths}");
                }
                else
                {
                    if (tNameTXT[i] != null) tNameTXT[i].text = "";
                    if (tKillTXT[i] != null) tKillTXT[i].text = "";
                    if (tDeathTXT[i] != null) tDeathTXT[i].text = "";
                }
            }

            // CT 팀 UI 업데이트
            for (int i = 0; i < ctNameTXT.Length; i++)
            {
                if (i < sortedCTPlayers.Count)
                {
                    var player = sortedCTPlayers[i];
                    if (ctNameTXT[i] != null) ctNameTXT[i].text = player.Key;
                    if (ctKillTXT[i] != null) ctKillTXT[i].text = player.Value.kills.ToString();
                    if (ctDeathTXT[i] != null) ctDeathTXT[i].text = player.Value.deaths.ToString();
                    Debug.Log($"CT Player {player.Key}: Kills {player.Value.kills}, Deaths {player.Value.deaths}");
                }
                else
                {
                    if (ctNameTXT[i] != null) ctNameTXT[i].text = "";
                    if (ctKillTXT[i] != null) ctKillTXT[i].text = "";
                    if (ctDeathTXT[i] != null) ctDeathTXT[i].text = "";
                }
            }
        }




        private void _timeRemaining_OnChange(SyncTimerOperation op, float prev, float next, bool asServer)
        {
            if (op == SyncTimerOperation.Start)
                Debug.Log($"타이머가 {next} 초로 시작되었습니다.");
            else if (op == SyncTimerOperation.Pause)
                Debug.Log($"타이머가 일시 중지되었습니다.");
            else if (op == SyncTimerOperation.PauseUpdated)
                Debug.Log($"타이머가 일시 중지되었으며 남은 시간이 {next} 초로 업데이트되었습니다.");
            else if (op == SyncTimerOperation.Unpause)
                Debug.Log($"타이머가 다시 시작되었습니다.");
            else if (op == SyncTimerOperation.Stop)
                Debug.Log($"타이머가 중지되었으며 더 이상 실행되지 않습니다.");
            else if (op == SyncTimerOperation.StopUpdated)
                Debug.Log($"타이머가 중지되었으며 더 이상 실행되지 않습니다. 타이머는 중지 전 {next} 값으로 중지되었으며 이전 값은 {prev}였습니다.");
            else if (op == SyncTimerOperation.Finished)
                Debug.Log($"타이머가 완료되었습니다!");
            else if (op == SyncTimerOperation.Complete)
                Debug.Log("이번 틱에 대한 모든 타이머 콜백이 완료되었습니다.");
        }


    }
}

