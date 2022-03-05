using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Card : MonoBehaviour, IUseable, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    public string cardName;

    public GameObject front;
    public GameObject back;
    public Text description;

    [HideInInspector]
    public Vector3 desiredPosition;
    [HideInInspector]
    public bool hoverable;

    public abstract bool Use();
    public abstract void Tick();

    public virtual void Return()
    {
        ToolController.Instance.Useable = null;
        ViewFrontSide();
        hoverable = true;
    }

    private bool mouse_over = false;
    private bool mouse_click = false;

    private bool back_side;

    void Update()
    {
        UpdatePosition();

        if(!GameManager.Instance.paused)
        {
            if (mouse_click)
            {
                ToolController.Instance.Useable = this;
                hoverable = false;
            }

            if (Input.GetMouseButtonDown(1))
            {
                Return();
            }

            if ((object)ToolController.Instance.Useable == this && Input.GetKeyDown(KeyCode.F))
            {
                if(back_side)
                {
                    ViewFrontSide();
                }
                else
                {
                    ViewBackSide();
                }
                
            }

            mouse_click = false;
        }
    }

    protected virtual void ViewBackSide()
    {
        if (back_side) return;
        back_side = true;
        LeanTween.rotateY(gameObject, 90, 0.2f).setEaseInCubic().setOnComplete(() => {
            front.SetActive(false);
            back.SetActive(true);
            back.transform.localScale = new Vector3(-1,1,1);
            LeanTween.rotateY(gameObject, 180, 0.2f).setEaseOutCubic();
        });
    }

    protected virtual void ViewFrontSide()
    {
        if (!back_side) return;
        back_side = false;
        LeanTween.rotateY(gameObject, 90, 0.2f).setEaseInCubic().setOnComplete(() => {
            front.SetActive(true);
            back.SetActive(false);
            LeanTween.rotateY(gameObject, 0, 0.2f).setEaseOutCubic();
        });
    }

    void UpdatePosition()
    {
        Vector3 finalDesiredPos = desiredPosition + Vector3.up * 40f;
        if (hoverable)
        {
            if (mouse_over) finalDesiredPos += Vector3.up * 28f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, finalDesiredPos, Time.unscaledDeltaTime * 10f);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, Input.mousePosition + Vector3.up * (Screen.height / 4f), Time.unscaledDeltaTime * 10f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance.paused) return;
        mouse_over = true;
        if (hoverable)
        {
            CardManager.Instance.hoveringOver = this;
            CardManager.Instance.UpdateCardPositions();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouse_over = false;
        if (CardManager.Instance.hoveringOver == this) CardManager.Instance.hoveringOver = null;
        CardManager.Instance.UpdateCardPositions();
    }

    public void OnPointerDown(PointerEventData eventData)
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
