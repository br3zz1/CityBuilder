using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Card : MonoBehaviour, IUseable, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    [Header("General")]
    [SerializeField]
    private string cardName;
    [SerializeField]
    private int cost;

    public Vector3 desiredPosition;
    public bool hoverable;

    public abstract bool Use();
    public abstract void Tick();

    public virtual void Return()
    {
        ToolController.Instance.Useable = null;
        hoverable = true;
    }

    private bool mouse_over = false;
    private bool mouse_click = false;

    void Update()
    {
        UpdatePosition();
        

        if(mouse_click)
        {
            ToolController.Instance.Useable = this;
            hoverable = false;
        }

        if(Input.GetMouseButtonDown(1))
        {
            Return();
        }

        mouse_click = false;
    }

    void UpdatePosition()
    {
        Vector3 finalDesiredPos = desiredPosition + Vector3.up * 80f;
        if (hoverable)
        {
            if (mouse_over) finalDesiredPos += Vector3.up * 50f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, finalDesiredPos, Time.deltaTime * 10f);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, Input.mousePosition + Vector3.up * 200f, Time.deltaTime * 10f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse_over = true;
        CardManager.Instance.hoveringOver = this;
        CardManager.Instance.UpdateCardPositions();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouse_over = false;
        if (CardManager.Instance.hoveringOver == this) CardManager.Instance.hoveringOver = null;
        CardManager.Instance.UpdateCardPositions();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        mouse_click = true;
    }
}

[Serializable]
public struct CardInspector
{
    public int cardCount;
    public Card card;
}
