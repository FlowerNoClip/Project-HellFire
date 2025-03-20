using FishNet.Object;
using UnityEngine;

public class LegArmHit : NetworkBehaviour, HitInterface
{
    public Player player;
    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public void Hit(int _gunIdx, float _distance, int pierceWallCount)
    {
        if (IsServerInitialized)
        {
            // 서버에서 직접 데미지 처리
            player.OnDamageServer(_gunIdx, _distance, pierceWallCount, 2);
            Debug.Log("Server: Leg hit processed");
        }
        else
        {
            // 클라이언트에서는 서버에 데미지 처리 요청
            HitServerRpc(_gunIdx, _distance, pierceWallCount);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void HitServerRpc(int _gunIdx, float _distance, int pierceWallCount)
    {
        // 서버에서 데미지 처리
        player.OnDamageServer(_gunIdx, _distance, pierceWallCount, 2);
        Debug.Log("Server: Leg hit processed from client request");
    }
}

