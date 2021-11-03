using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour, IUseable
{

    private string cardName;
    private int cost;

    public abstract bool Use();
    public abstract void Tick();
}
