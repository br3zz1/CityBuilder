using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : TileObject
{
    private int score;
    public override int AddedValue()
    {
        score = 0;

        ForeachRoadDistanceDo(RoadDistance);

        ForeachAirDistanceDo((ObjectDistance od) => {
            if (od.distance < 6f && od.obj is TreeObject) score += 6;
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
