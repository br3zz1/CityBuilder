using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commercial : TileObject
{
    public override int AddedValue(bool negative = false)
    {
        base.AddedValue(negative);
        BeginAddValue();
        score = 0;

        ForeachRoadDistanceDo(RoadDistance);

        ForeachNeighbourDo((TileObject t) =>
        {
            if (t is Commercial) AddValue(t, 10);
        });

        //AddValue(this, 15);

        EndAddValue();
        return score;
    }

    private void RoadDistance(ObjectDistance rd)
    {
        if (rd.obj is House && rd.distance < 5f)
        {
            AddValue(rd.obj, 2);
        }
    }
}
