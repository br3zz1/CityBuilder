using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance;

    public Tooltip tooltip;

    private LTDescr ltd;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void Show(string header, string content, float duration = 0)
    {
        if (ltd != null)
        {
            LeanTween.cancel(ltd.id);
            ltd = null;
        }

        tooltip.gameObject.SetActive(true);
        tooltip.ChangeText(header, content);

        LeanTween.scale(tooltip.gameObject, Vector3.one, 0.2f).setEaseOutBack();

        if(duration > 0)
        {
            ltd = LeanTween.delayedCall(duration, () => { Hide(); });
        }
    }

    public void Show(string header, string content, Color color, float duration = 0)
    {
        Show(header, content, duration);
        tooltip.ChangeText(header, content, color);
    }

    public void Hide(bool complete = false)
    {
        if(!complete) LeanTween.scale(tooltip.gameObject, Vector3.zero, 0.2f).setEaseInBack().setOnComplete(() => { Hide(true); });
        else tooltip.gameObject.SetActive(false);
    }
}
