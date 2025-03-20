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
            Debug.Log("�ΰ� ����");
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
        if (hp <= 0) return; // �̹� ���� ���¶�� ó������ ����

        int hitDamage = CalculateDamage(_gunIdx, _distance, _bodyType);
        if (pierceWallCount > 0)
        {
            hitDamage /= 2;
        }

        hp -= hitDamage;
        
        // Ŭ���̾�Ʈ�� ������ ���� ����
        OnDamageClients(hitDamage, hp);

        if (hp <= 0)
        {
            // ���⼭ LastAttacker�� ����ϴ� ���, 
            // ������ ������ ��Ʈ��ũ�� ���� ���޹޾ƾ� �մϴ�.
            // ���� ���, �������� NetworkObject.ObjectId�� �Ķ���ͷ� ���� �� �ֽ��ϴ�.
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
    // ��� �� ���� �޼���
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

        // ���� ������ �Ҹ��մϴ�.
        if (armor > 0)
        {
            int remainingDamage = _hitDamage - armor;
            armor -= _hitDamage;
            if (armor < 0)
            {
                armor = 0;
            }

            // ������ ��� �Ҹ��ϰ� ���� �������� ü�¿� �����մϴ�.
            if (remainingDamage > 0)
            {
                hp -= remainingDamage;
            }
        }
        else
        {
            // ������ ���� ���, �������� ü�¿� �ٷ� �����մϴ�.
            hp -= _hitDamage;
        }

        // ü�� �� ���� UI ������Ʈ
        if (playerController.playerCamera != null)
        {
            healthText.text = hp.ToString();
            armorText.text = armor.ToString(); // ���� �ؽ�Ʈ�� ������Ʈ
        }

        // ü���� 0 ���ϰ� �Ǹ�
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
    // �߰����� ���� �� ó�� (��: ������ Ÿ�̸� ���� ��)
}

[ObserversRpc]
private void DeadClients()
{
    // Deads �޼����� ������ ����� �ű�
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
        Debug.Log("Respawn �Լ� ȣ���");
        if (_tempCode)
        {
            Debug.Log("�÷��̾� �� �ڵ�: �׷�����Ʈ");

            if (roundManager.TSpawns.Length > 0) // ���� ����Ʈ�� �ִ��� Ȯ��
            {
                Debug.Log("TSpawns �迭 ����: " + roundManager.TSpawns.Length);

                // TSpawns �迭���� ������ �ε����� ����
                int randomIndex = UnityEngine.Random.Range(0, roundManager.TSpawns.Length);
                Debug.Log("���õ� ���� �ε���: " + randomIndex);

                // ���õ� ���� ����Ʈ�� Transform�� ������
                Transform randomSpawn = roundManager.TSpawns[randomIndex];
                Debug.Log("���õ� ���� ����Ʈ: " + randomSpawn.position);

                // �÷��̾��� ��ġ�� ���õ� ���� ����Ʈ�� ��ġ�� ����
                playerNetwork.gameObject.transform.position = randomSpawn.position;
                Debug.Log("�÷��̾� ��ġ ������: " + playerNetwork.transform.position);
            }
            else
            {
                Debug.LogWarning("roundManager.TSpawns�� ����� �� �ִ� ���� ����Ʈ�� �����ϴ�.");
            }
        }
        else
        {
            Debug.Log("�÷��̾� �� �ڵ�: ī���� �׷�����Ʈ");

            if (roundManager.CTSpawns.Length > 0) // ���� ����Ʈ�� �ִ��� Ȯ��
            {
                Debug.Log("CTSpawns �迭 ����: " + roundManager.CTSpawns.Length);

                // CTSpawns �迭���� ������ �ε����� ����
                int randomIndex = UnityEngine.Random.Range(0, roundManager.CTSpawns.Length);
                Debug.Log("���õ� ���� �ε���: " + randomIndex);

                // ���õ� ���� ����Ʈ�� Transform�� ������
                Transform randomSpawn = roundManager.CTSpawns[randomIndex];
                Debug.Log("���õ� ���� ����Ʈ: " + randomSpawn.position);

                // �÷��̾��� ��ġ�� ���õ� ���� ����Ʈ�� ��ġ�� ����
                playerNetwork.gameObject.transform.position = randomSpawn.position;
                Debug.Log("�÷��̾� ��ġ ������: " + playerNetwork.transform.position);

            }
            else
            {
                Debug.LogWarning("roundManager.CTSpawns�� ����� �� �ִ� ���� ����Ʈ�� �����ϴ�.");
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