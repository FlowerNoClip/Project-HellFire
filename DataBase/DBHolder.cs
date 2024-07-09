using UnityEngine;

public class DBHolder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static DBHolder Instance { get; private set; }

    // Awake 메서드에서 인스턴스를 설정
    private void Awake()
    {
        // 인스턴스가 이미 존재하는지 확인
        if (Instance != null && Instance != this)
        {
            // 인스턴스가 이미 존재하면 중복된 인스턴스를 파괴
            Destroy(gameObject);
        }
        else
        {
            // 인스턴스가 없으면 이 인스턴스를 설정하고 파괴되지 않도록 설정
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
