using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] public GameObject CrosshairX;
    [SerializeField] GameObject CrosshairY;

    private float Xscale;
    private float Yscale;

    private float scaleMin_X = 0.01f;
    private float scaleMax_X = 0.2f;
    private float scaleMin_Y = 0.01f;
    private float scaleMax_Y = 0.2f;

    RectTransform rectTransformX;
    RectTransform rectTransformY;

    private void Start()
    {
        rectTransformX = CrosshairX.GetComponent<RectTransform>();
        rectTransformY = CrosshairY.GetComponent<RectTransform>();
    }
    public void UpdateScaleX(float sliderValueX)
    {
        Xscale = sliderValueX * (scaleMax_X - scaleMin_X) + scaleMin_X; //항상 값을 0.01~0.2사이에서 진행되도록
        Vector3 newScale = rectTransformX.localScale;
        newScale.x = Xscale;
        rectTransformX.localScale = newScale;
            }
    public void UpdateScaleY(float sliderValueY)
    {
        Yscale = sliderValueY * (scaleMax_Y - scaleMin_Y) + scaleMin_Y;
        Vector3 newScale = rectTransformY.localScale;
        newScale.y = Yscale;
        rectTransformY.localScale = newScale;
    }
}