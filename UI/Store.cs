using UnityEngine;

public class Store : MonoBehaviour
{
    public static Store Instance { get; set; }
    private StoreData storeData;
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

    public StoreData returnStoreData()
    {
        storeData = transform.GetComponentInChildren<StoreData>();
        return storeData;
    }
}
