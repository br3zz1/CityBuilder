using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warehouse : TileObject
{
    
    public override int AddedValue(bool negative = false)
    {
        base.AddedValue(negative);
        BeginAddValue();
        score = 0;

        ForeachRoadDistanceDo(RoadDistance);

        ForeachNeighbourDo((TileObject t) =>
        {
            if (t is Warehouse) AddValue(t, 4);
        });

        //AddValue(this, 15);

        EndAddValue();
        return score;
    }

    private void RoadDistance(ObjectDistance rd)
    {
        if (rd.obj is Commercial && rd.distance < 8f)
        {
            AddValue(rd.obj, 6);
        }
    }
}
