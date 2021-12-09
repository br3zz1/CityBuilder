using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObject : TileObject
{
    private int score;
    public override int AddedValue()
    {
        score = 0;

        ForeachAirDistanceDo((ObjectDistance od) => {
            if (od.distance < 3f && od.obj is Water) score += 4;
            else if (od.distance < 6f && od.obj is House) score += 6;
        });

        ForeachNeighbourDo((TileObject t) => {
            if (t is TreeObject) score += 1;
        });

        return score;
    }
}
