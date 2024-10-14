using TMPro;
using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    [SerializeField] private float xLimit = 960;
    [SerializeField] private float yLimit = 540;//分别是1920*1080 的一半

    [SerializeField] private float xOffset = 150;
    [SerializeField] private float yOffset = 150;

    public virtual void AdjustPosition()  //   根据分辨率设置xy 限制，然后再根据鼠标的位置调整 tooltip出现的位置
    {
        Vector2 mousePosition = Input.mousePosition;
        float newXOffset = 0;
        float newYOffset = 0;
        if (mousePosition.x > xLimit)
            newXOffset = -xOffset;
        else
            newXOffset = xOffset;


        if (mousePosition.y > yLimit)
            newYOffset = -yOffset;
        else
            newYOffset = yOffset;
        transform.position = new Vector2(mousePosition.x + newXOffset, mousePosition.y + newYOffset);
    }

    public void AdjustFontSize(TextMeshProUGUI _text) //调整字体  但是感觉不太好影响整体
    {
        if (_text.text.Length > 12)
            _text.fontSize = _text.fontSize * .8f;
    }
}
