using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    [SerializeField]
    private CardInspector[] cardIPs;

    private List<Card> holdingCards;

    public Card hoveringOver;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        holdingCards = new List<Card>();
        AddCard(0);
        AddCard(0);
        AddCard(0);
        AddCard(0);
        AddCard(0);
        AddCard(0);
        AddCard(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCardPositions()
    {
        int i = 0;
        Vector3 offset = new Vector3(-20f,0,0);
        if (hoveringOver == null) offset = Vector3.zero;
        foreach (Card card in holdingCards)
        {
            if (card == hoveringOver) offset.x = -offset.x;
            card.desiredPosition = new Vector3(-(holdingCards.Count / 2f) * 50f + i * 50f, 0f, 0f);
            i++;
        }
    }


    public void AddCard(int cardIPindex)
    {
        GameObject cardGo = Instantiate(cardIPs[cardIPindex].card.gameObject, new Vector3(0, 200f, 0), Quaternion.identity);
        cardGo.transform.parent = transform;
        Card card = cardGo.GetComponent<Card>();
        holdingCards.Add(card);
        UpdateCardPositions();
    }

    public void RemoveCard(Card card)
    {
        holdingCards.Remove(card);
        Destroy(card);
    }
}
