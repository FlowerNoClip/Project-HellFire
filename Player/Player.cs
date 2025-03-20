using FishNet.Component.Spawning;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public int hp = 100;
    public int armor = 0;
    private DBHolder dbHolder;
    private DistanceDamageTable distanceDamageTable;
    private RoundManager roundManager;
    private PlayerNetwork playerNetwork;
    private PlayerColliderManger playerColliderManger;

    [SerializeField] private PlayerAnimation pAnim;
    [SerializeField] private SkinnedMeshRenderer[] playerSkinnedMeshRenderer;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] public TextMeshProUGUI armorText;

    public readonly SyncVar<int> myKill = new SyncVar<int>();
    public readonly SyncVar<int> myDeath = new SyncVar<int>();

    public bool isDead = false;
    [Header("Movement")]
    [SerializeField] private float walkACC;
    [SerializeField] private float runACC;
    [SerializeField] private float maxWalkSPD;
    [SerializeField] private float maxRunSPD;
    [SerializeField] private float currentSpeed;
    [Header("Jump")]
    [SerializeField] private float maxJumpHeight;
    [SerializeField] private float jumpCooldown;
    [Header("Crouch")]
    [SerializeField] private float crouchWalkACC;
    [SerializeField] private float maxCrouchWalkSPD;
    [SerializeField] private float crouchCooldown;
    [Header("MouseSen")]
    [SerializeField] private float senX;
    [SerializeField] private float senY;
    public float WalkACC { get { return walkACC; } set { walkACC = value; } }
    public float RunACC { get { return runACC; } set { runACC = value; } }
    public float MaxWalkSPD { get { return maxWalkSPD; } set { maxWalkSPD = value; } }
    public float MaxRunSPD { get { return maxRunSPD; } set { maxRunSPD = value; } }
    public float CurrentSpeed { get { return currentSpeed; } set { currentSpeed = value; } }
    public float MaxJumpHeight { get { return maxJumpHeight; } set { maxJumpHeight = value; } }
    public float JumpCooldown { get { return jumpCooldown; } set { jumpCooldown = value; } }
    public float CrouchWalkACC { get { return crouchWalkACC; } set { crouchWalkACC = value; } }
    public float MaxCrouchWalkSPD { get { return maxCrouchWalkSPD; } set { maxCrouchWalkSPD = value; } }
    public float CrouchCooldown { get { return crouchCooldown; } set { crouchCooldown = value; } }
    public float SenX { get { return senX; } set { } }
    public float SenY { get { return senY; } set { } }
    private void Awake()
    {
        dbHolder = DBHolder.Instance;
        roundManager = RoundManager.Instance;
        distanceDamageTable = dbHolder.GetComponent<DistanceDamageTable>();
        playerNetwork = GetComponent<PlayerNetwork>();
        playerColliderManger = GetComponent<PlayerColliderManger>();
        healthText = roundManager.hpText;
        armorText = roundManager.armorText;
        roundManager.player = this;
    }
    private void Start()
    {
        if (distanceDamageTable == null)
        {
            Debug.Log("널값 들어옴");
        }
        for (int i = 0; i < distanceDamageTable.DistanceDamageInfoArray.Length; i++)
        {
            //Debug.Log(distanceDamageTable.DistanceDamageInfoArray[i].index);
        }
        healthText.text = hp.ToString();
    }
    [ServerRpc(RequireOwnership = false)]
    public void OnDamageServer(int _gunIdx, float _distance, int pierceWallCount, int _bodyType)
    {
        if (hp <= 0) return; // 이미 죽은 상태라면 처리하지 않음

        int hitDamage = CalculateDamage(_gunIdx, _distance, _bodyType);
        if (pierceWallCount > 0)
        {
            hitDamage /= 2;
        }

        hp -= hitDamage;
        
        // 클라이언트에 데미지 정보 전달
        OnDamageClients(hitDamage, hp);

        if (hp <= 0)
        {
            // 여기서 LastAttacker를 사용하는 대신, 
            // 공격자 정보를 네트워크를 통해 전달받아야 합니다.
            // 예를 들어, 공격자의 NetworkObject.ObjectId를 파라미터로 받을 수 있습니다.
            DeadServer();
        }
    }
    [ObserversRpc]
    private void OnDamageClients(int damage, int newHp)
    {
        hp = newHp;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateKillDeathServer(int kills, int deaths)
    {
        myKill.Value = kills;
        myDeath.Value = deaths;
        UpdateKillDeathClients(kills, deaths);
        roundManager.UpdateLeaderboard();
    }
    [ObserversRpc]
    private void UpdateKillDeathClients(int kills, int deaths)
    {
        myKill.Value = kills;
        myDeath.Value = deaths;
        Debug.Log($"Updated kills for {playerNetwork.nickname}: {kills}");
    }

    private int CalculateDamage(int _gunIdx, float _distance, int _bodyType)
    {
        var damageInfo = distanceDamageTable.DistanceDamageInfoArray
            .FirstOrDefault(info => _gunIdx == info.index &&
                                    _distance >= info.min_Distance &&
                                    _distance < info.max_Distance);

        switch (_bodyType)
        {
            case 0: return damageInfo.head_dmg;
            case 1: return damageInfo.body_dmg;
            case 2: return damageInfo.leg_dmg;
            default:
                Debug.LogWarning("Invalid body type specified.");
                return 0;
        }
    }

[ServerRpc(RequireOwnership = false)]
private void IncreaseKillServer()
{
    myKill.Value++;
    UpdateKillDeathClients(myKill.Value, myDeath.Value);
    RoundManager.Instance.UpdateLeaderboard();
}

    [ObserversRpc]
    // 사망 수 증가 메서드
    public void IncreaseDeath()
    {
        myDeath.Value++;
        UpdateKillDeathServer(myKill.Value, myDeath.Value);
    }


    [ServerRpc(RequireOwnership = false)]
    private void SendDamageServer(int _hitDamage, string _nickname)
    {
        SendDamage(_hitDamage, playerNetwork.nickname);
    }
    [ObserversRpc]
    private void SendDamage(int _hitDamage, string _nickname)
    {

        // 먼저 방어력을 소모합니다.
        if (armor > 0)
        {
            int remainingDamage = _hitDamage - armor;
            armor -= _hitDamage;
            if (armor < 0)
            {
                armor = 0;
            }

            // 방어력을 모두 소모하고 남은 데미지를 체력에 적용합니다.
            if (remainingDamage > 0)
            {
                hp -= remainingDamage;
            }
        }
        else
        {
            // 방어력이 없을 경우, 데미지를 체력에 바로 적용합니다.
            hp -= _hitDamage;
        }

        // 체력 및 방어력 UI 업데이트
        if (playerController.playerCamera != null)
        {
            healthText.text = hp.ToString();
            armorText.text = armor.ToString(); // 방어력 텍스트도 업데이트
        }

        // 체력이 0 이하가 되면
        if (hp <= 0)
        {
            Debug.Log("Player died. Checking for killer...");
            if (playerNetwork.LastAttacker != null)
            {
                Debug.Log($"Last attacker found: {playerNetwork.LastAttacker.nickname}");
                Player killer = playerNetwork.LastAttacker.GetComponent<Player>();
                if (killer != null)
                {
                    Debug.Log($"Calling IncreaseKill for {killer.playerNetwork.nickname}");
                    killer.IncreaseKillServer();
                }
                else
                {
                    Debug.Log("Killer Player component not found");
                }
            }
            else
            {
                Debug.Log("No last attacker found");
            }
            DeadServer();
        }
    }


[ServerRpc(RequireOwnership = false)]
private void DeadServer()
{
    DeadClients();
    // 추가적인 서버 측 처리 (예: 리스폰 타이머 시작 등)
}

[ObserversRpc]
private void DeadClients()
{
    // Deads 메서드의 내용을 여기로 옮김
    playerColliderManger.DeActivveColliderAll();
    pAnim.SetBodyWeightZero();
    pAnim.SetDead();
    KillCountUp(playerNetwork.teamCode);
    IncreaseDeath();
    isDead = true;
    StartCoroutine(Revive());
}
    public IEnumerator Revive()
    {

        yield return new WaitForSeconds(4f);
        ReVives();
        yield return null;
    }
    [ObserversRpc]
    public void ReVives()
    {
        isDead = false;
        hp = 100;
        if (playerController.playerCamera != null)
        {
            healthText.text = hp.ToString();
        }
        Respawn(playerNetwork.teamCode);


        playerSkinnedMeshRenderer[0].enabled = true;
        playerSkinnedMeshRenderer[1].enabled = true;
        playerColliderManger.ActiveColliderAll();
        pAnim.SetBodyWeightOne();
        pAnim.SetIdle();
    }
    [ObserversRpc]
    public void Respawn(bool _tempCode)
    {
        Debug.Log("Respawn 함수 호출됨");
        if (_tempCode)
        {
            Debug.Log("플레이어 팀 코드: 테러리스트");

            if (roundManager.TSpawns.Length > 0) // 스폰 포인트가 있는지 확인
            {
                Debug.Log("TSpawns 배열 길이: " + roundManager.TSpawns.Length);

                // TSpawns 배열에서 랜덤한 인덱스를 선택
                int randomIndex = UnityEngine.Random.Range(0, roundManager.TSpawns.Length);
                Debug.Log("선택된 랜덤 인덱스: " + randomIndex);

                // 선택된 스폰 포인트의 Transform을 가져옴
                Transform randomSpawn = roundManager.TSpawns[randomIndex];
                Debug.Log("선택된 스폰 포인트: " + randomSpawn.position);

                // 플레이어의 위치를 선택된 스폰 포인트의 위치로 설정
                playerNetwork.gameObject.transform.position = randomSpawn.position;
                Debug.Log("플레이어 위치 설정됨: " + playerNetwork.transform.position);
            }
            else
            {
                Debug.LogWarning("roundManager.TSpawns에 사용할 수 있는 스폰 포인트가 없습니다.");
            }
        }
        else
        {
            Debug.Log("플레이어 팀 코드: 카운터 테러리스트");

            if (roundManager.CTSpawns.Length > 0) // 스폰 포인트가 있는지 확인
            {
                Debug.Log("CTSpawns 배열 길이: " + roundManager.CTSpawns.Length);

                // CTSpawns 배열에서 랜덤한 인덱스를 선택
                int randomIndex = UnityEngine.Random.Range(0, roundManager.CTSpawns.Length);
                Debug.Log("선택된 랜덤 인덱스: " + randomIndex);

                // 선택된 스폰 포인트의 Transform을 가져옴
                Transform randomSpawn = roundManager.CTSpawns[randomIndex];
                Debug.Log("선택된 스폰 포인트: " + randomSpawn.position);

                // 플레이어의 위치를 선택된 스폰 포인트의 위치로 설정
                playerNetwork.gameObject.transform.position = randomSpawn.position;
                Debug.Log("플레이어 위치 설정됨: " + playerNetwork.transform.position);

            }
            else
            {
                Debug.LogWarning("roundManager.CTSpawns에 사용할 수 있는 스폰 포인트가 없습니다.");
            }
        }
    }

    private void KillCountUp(bool _tempCode)
    {
        if (!_tempCode && !isDead)
        {
            roundManager.TKillCount += 1;
            roundManager.SyncsPublic();
        }
        else if (_tempCode && !isDead)
        {
            roundManager.CTKillCount += 1;
            roundManager.SyncsPublic();
        }
    }

}