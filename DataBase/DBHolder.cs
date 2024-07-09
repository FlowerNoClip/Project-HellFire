using UnityEngine;

public class DBHolder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static DBHolder Instance { get; private set; }

    // Awake �޼��忡�� �ν��Ͻ��� ����
    private void Awake()
    {
        // �ν��Ͻ��� �̹� �����ϴ��� Ȯ��
        if (Instance != null && Instance != this)
        {
            // �ν��Ͻ��� �̹� �����ϸ� �ߺ��� �ν��Ͻ��� �ı�
            Destroy(gameObject);
        }
        else
        {
            // �ν��Ͻ��� ������ �� �ν��Ͻ��� �����ϰ� �ı����� �ʵ��� ����
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
