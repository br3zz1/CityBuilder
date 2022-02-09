using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsEffect : MonoBehaviour
{
    public static PointsEffect Instance;

    private List<Point> points;
    private GameObject pointPrefab;

    void Awake()
    {
        Instance = this;
        points = new List<Point>();
        pointPrefab = Resources.Load<GameObject>("Prefabs/Point");
        for(int i = 0; i < 100; i++)
        {
            Point point = Instantiate(pointPrefab).GetComponent<Point>();
            points.Add(point);
        }
    }

    public void Burst(int points)
    {

    }
}
