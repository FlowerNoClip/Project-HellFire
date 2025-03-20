using UnityEngine;

public class AttackerSkill : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }
}
