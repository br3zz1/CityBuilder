using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    public List<CardInspector> cardIPs;

    private int totalCardCount;

    [HideInInspector]
    public List<Card> holdingCards;

    [HideInInspector]
    public Card hoveringOver;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        holdingCards = new List<Card>();

        foreach (CardInspector cip in cardIPs)
        {
            totalCardCount += cip.cardCount;
        }
    }

    public void UpdateCardPositions()
    {
        int i = 0;
        Vector3 offset = new Vector3(-70f,0,0);
        if (hoveringOver == null) offset = Vector3.zero;
        foreach (Card card in holdingCards)
        {
            card.GetComponent<RectTransform>().SetSiblingIndex(i);
            if (card == hoveringOver)
            {
                offset.x = -offset.x;
                card.desiredPosition = new Vector3(-(holdingCards.Count / 2f) * 50f + i * 50f + 25f, 0f, 0f);
            }
            else
            {
                card.desiredPosition = new Vector3(-(holdingCards.Count / 2f) * 50f + i * 50f + 25f, 0f, 0f) + offset;
            }
            i++;
        }
    }


    private void AddCard(Card cardPrefab)
    {
        GameObject cardGo = Instantiate(cardPrefab.gameObject, new Vector3(-100f, 0, 0), Quaternion.identity);
        cardGo.transform.SetParent(transform, true);
        cardGo.transform.localScale = Vector3.one;
        Card card = cardGo.GetComponent<Card>();
        holdingCards.Insert(0, card);
        UpdateCardPositions();
    }

    public void AddRandomCard()
    {
        int rCard = Random.Range(0, totalCardCount);

        int i = 0;
        foreach (CardInspector cip in cardIPs)
        {
            for(int j = 0; j < cip.cardCount; j++)
            {
                if (i == rCard)
                {
                    AddCard(cip.card);
                    return;
                }
                i++;
            }
        }
    }

    public void AddCard(string name)
    {
        foreach(CardInspector cip in cardIPs)
        {
            if(cip.card.cardName == name)
            {
                AddCard(cip.card);
                break;
            }
        }
    }

    public void RemoveCard(Card card)
    {
        holdingCards.Remove(card);
        card.Return();
        Destroy(card.gameObject);
        UpdateCardPositions();
    }
}
