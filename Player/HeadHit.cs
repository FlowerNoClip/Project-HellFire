using FishNet.Demo.AdditiveScenes;
using FishNet.Object;
using UnityEngine;

public class HeadHit : NetworkBehaviour,HitInterface
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
            // �������� ���� ������ ó��
            player.OnDamageServer(_gunIdx, _distance, pierceWallCount, 0);
            Debug.Log("Server: Head hit processed");
        }
        else
        {
            // Ŭ���̾�Ʈ������ ������ ������ ó�� ��û
            HitServerRpc(_gunIdx, _distance, pierceWallCount);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void HitServerRpc(int _gunIdx, float _distance, int pierceWallCount)
    {
        // �������� ������ ó��
        player.OnDamageServer(_gunIdx, _distance, pierceWallCount, 0);
        Debug.Log("Server: Head hit processed from client request");
    }
}