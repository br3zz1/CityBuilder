using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueText : MonoBehaviour
{

    public TileObject parent;

    // Update is called once per frame
    void Update()
    {
        if (parent == null) Debug.LogError("Parent null");
        else
        {
            Vector3 heading = parent.transform.position - Camera.main.transform.position;
            if (Vector3.Dot(Camera.main.transform.forward, heading) > 0)
            {
                transform.localScale = Vector3.one;
                transform.position = Camera.main.WorldToScreenPoint(parent.transform.position);
            }
            else
            {
                transform.localScale = Vector3.zero;
            }
        }
    }
}
