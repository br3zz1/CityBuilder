using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commercial : TileObject
{

    [SerializeField]
    private MeshRenderer changeColor;

    public override void Init(Tile tile)
    {
        base.Init(tile);
        if(!preview && changeColor != null)
        {
            int rHue = Random.Range(0,6);
            float hue = (rHue * 60f) / 360f;
            changeColor.material.color = Color.HSVToRGB(hue, 1,1);
        }
    }

    public override int AddedValue(bool negative = false)
    {
        base.AddedValue(negative);
        BeginAddValue();
        score = 0;

        ForeachRoadDistanceDo(RoadDistance);

        ForeachNeighbourDo((TileObject t) =>
        {
            if (t is Commercial) AddValue(t, 4);
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
        else if(rd.obj is Warehouse && rd.distance < 8f)
        {
            AddValue(rd.obj, 6);
        }
    }
}
