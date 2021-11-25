using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : TileObject
{

    [SerializeField]
    private string type;

    private int score;
    public override int AddedValue()
    {
        score = 0;

        ForeachRoadDistanceDo(RoadDistance);

        ForeachAirDistanceDo((ObjectDistance od) => {
            if (od.distance < 3f && od.obj is Water) score += 10;
        });

        return score;
    }

    private void RoadDistance(ObjectDistance rd)
    {
        if(rd.obj is House)
        {
            score += 5;
        }
    }
}
