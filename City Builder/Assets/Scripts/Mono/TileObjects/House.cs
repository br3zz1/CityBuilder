using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : TileObject
{
    public override int AddedValue(bool negative = false)
    {
        base.AddedValue(negative);
        BeginAddValue();
        score = 0;

        ForeachRoadDistanceDo(RoadDistance);

        ForeachAirDistanceDo((ObjectDistance od) => {
            if (od.distance < 3f && od.obj is TreeObject) AddValue(od.obj, 2);
        });

        ForeachNeighbourDo((TileObject t) =>
        {
            if (t is Water) AddValue(t, -10);
        });

        //AddValue(this, 15);

        EndAddValue();
        return score;
    }

    private void RoadDistance(ObjectDistance rd)
    {
        if(rd.obj is House)
        {
            AddValue(rd.obj, 1);
        }
    }
}
