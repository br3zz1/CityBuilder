using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSwap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler//, IPointerUpHandler
{

    public static CardSwap Instance;

    bool mouseOver;
    bool hoverState;

    public Image panel;
    public SVGImage image;
    public Text text;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        mouseOver = false;
        hoverState = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseOver)
        {
            if (hoverState != ToolController.Instance.Useable is Card)
            {
                if (ToolController.Instance.Useable is Card)
                {
                    if (GameManager.Instance.cardSwaps > 0)
                    {
                        CardEntered();
                    }
                }
                else
                {
                    CardExit();
                }
            }
        }
        text.text = "Card Swap\nAvailable: " + GameManager.Instance.cardSwaps;
    }

    public bool Swap()
    {
        if (GameManager.Instance.cardSwaps > 0 && hoverState)
        {
            Card card = (Card)ToolController.Instance.Useable;
            CardManager.Instance.RemoveCard(card);
            CardManager.Instance.AddRandomCard(card.cardName);
            GameManager.Instance.cardSwaps--;
            return true;
        }
        return false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        CardExit();
    }

    void CardEntered()
    {
        if (hoverState) return;
        LeanTween.value(0, 1, 0.2f).setOnUpdate(AlphaUpdate).setEaseInOutQuad();
        hoverState = true;
    }

    void CardExit()
    {
        if (!hoverState) return;
        LeanTween.value(1, 0, 0.2f).setOnUpdate(AlphaUpdate).setEaseInOutQuad();
        hoverState = false;
    }

    void AlphaUpdate(float value)
    {
        panel.color = Color(panel.color, value / 8f);
        text.color = Color(text.color, value * 3 / 4 + 0.25f);
        image.color = Color(image.color, value * 3 / 4 + 0.25f);
    }

    Color Color(Color c, float alpha)
    {
        c.a = alpha;
        return c;
    }
}
