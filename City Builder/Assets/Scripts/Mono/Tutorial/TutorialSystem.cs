using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSystem : MonoBehaviour
{

    public static TutorialSystem Instance;

    [SerializeField]
    private Text tutorialText;
    [SerializeField]
    private GameObject panel;

    private int tutorialState;

    [SerializeField]
    private bool resetTutorial;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {   
        if (resetTutorial) PlayerPrefs.SetInt("tutorialDone", 0);

        if (PlayerPrefs.GetInt("tutorialDone", 0) == 0)
        {
            tutorialState = 0;
            PlayerPrefs.SetInt("tutorialDone", 1);

            Show("Use WASD or arrow keys to move");
        }
        else
        {
            tutorialState = -1;
        }
    }

    private void Show(string text)
    {
        LeanTween.moveLocalY(panel, 0, 0.4f).setEaseOutCubic();
        tutorialText.text = text;
    }

    private void Hide(Action complete)
    {
        LeanTween.moveLocalY(panel, 150, 0.4f).setEaseInCubic().setOnComplete(complete);
    }

    public void Skip()
    {
        Hide(() => { });
        PlayerPrefs.SetInt("tutorialDone", 1);
        tutorialState = -1;
    }

    public void Moved()
    {
        if (tutorialState == 0)
        {
            tutorialState = 1;
            Hide(() => { Show("Use the right mouse button to look around"); });
        }
    }

    public void Rotated()
    {
        if (tutorialState == 1)
        {
            tutorialState = 2;
            Hide(() => { Show("Drag and drop a card onto the map to build a house. Right click to cancel"); });
        }
    }

    public void PlacedDown()
    {
        if (tutorialState == 2)
        {
            tutorialState = 3;
            Hide(() => { Show("Drag and drop a card into the card swap area to swap a card"); });
        }
    }

    public void Swapped()
    {
        if (tutorialState == 3)
        {
            tutorialState = 4;
            Hide(() => { Show("Reach the next milestone to get more cards"); });
        }
    }

    public void Milestone()
    {
        if (tutorialState == 4)
        {
            tutorialState = -1;
            Hide(() => { });
        }
    }

}