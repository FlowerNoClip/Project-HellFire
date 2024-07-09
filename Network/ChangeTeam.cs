using UnityEngine;
using UnityEngine.UI;

public class ChangeTeam : MonoBehaviour
{
    public static ChangeTeam Instance;
    public Button[] buttons;

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
}
