using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlindnessEffect : MonoBehaviour
{
    [SerializeField] private Image img;

    private Animator anim;
    private int width,height;
    public static BlindnessEffect activeInstance;
    private void Start()
    {
        activeInstance = this;
        anim = GetComponent<Animator>();
        width = Screen.width;
        height = Screen.height;
    }

    public void GoBlind()
    {
        StartCoroutine(goBlind());
    }

    private IEnumerator goBlind()
    {
        yield return new WaitForEndOfFrame();
        Texture2D tex = new Texture2D(width,height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, width,height), 0, 0);
        tex.Apply();

        img.sprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100);

        anim.SetTrigger("Go Blind");
    }
}
