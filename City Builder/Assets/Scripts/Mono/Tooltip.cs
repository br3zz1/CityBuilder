using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{

    public Text header;
    public Text content;

    public int characterLimit;

    private bool stopFollowing;

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            TooltipSystem.Instance.Hide();
            stopFollowing = true;
        }
        if(!stopFollowing)
        {
            Vector2 mousePos = Input.mousePosition;

            float pivotX = mousePos.x / Screen.width;
            float pivotY = mousePos.y / Screen.height;

            GetComponent<RectTransform>().pivot = new Vector2(pivotX, pivotY);

            transform.position = Input.mousePosition + new Vector3(10f, 10f);
        }
    }

    public void ChangeText(string headerText, string contentText)
    {
        stopFollowing = false;

        header.enabled = headerText.Length > 0;
        content.enabled = contentText.Length > 0;
        header.text = headerText;
        content.text = contentText;

        GetComponent<Image>().color = new Color(0.4f,0.4f,0.4f);

        GetComponent<LayoutElement>().enabled = (header.text.Length > characterLimit || content.text.Length > characterLimit) ? true : false;

        transform.position = Input.mousePosition + new Vector3(10f, 10f);
    }

    public void ChangeText(string headerText, string contentText, Color color)
    {
        stopFollowing = false;

        header.enabled = headerText.Length > 0;
        content.enabled = contentText.Length > 0;
        header.text = headerText;
        content.text = contentText;

        GetComponent<Image>().color = color;

        GetComponent<LayoutElement>().enabled = (header.text.Length > characterLimit || content.text.Length > characterLimit) ? true : false;

        transform.position = Input.mousePosition + new Vector3(10f, 10f);
    }
}
