using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoadGenerator
{
    public static int townRadius;

    public static bool[,] GenerateRoadLayout(int seed)
    {
        Random.InitState(seed);

        bool[,] layout = new bool[100, 100];
        int numRoads = 1;

        // Center point
        layout[50, 50] = true;

        GenerateMainRoad(Vector2Int.up, ref layout, ref numRoads);
        GenerateMainRoad(Vector2Int.down, ref layout, ref numRoads);
        GenerateMainRoad(Vector2Int.left, ref layout, ref numRoads);
        GenerateMainRoad(Vector2Int.right, ref layout, ref numRoads);

        for (int i = 0; i < 200 + townRadius * townRadius; i++)
        {
            GenerateSmallRoad(ref layout, ref numRoads, 7, RandomRoadPosition(ref layout, ref numRoads), RandomDirection());
        }

        return layout;
    }

    private static Vector2Int RandomDirection()
    {
        int n = Random.Range(0, 4);
        if (n == 0)
        {
            return Vector2Int.up;
        }
        else if (n == 1)
        {
            return Vector2Int.down;
        }
        else if (n == 2)
        {
            return Vector2Int.left;
        }
        return Vector2Int.right;
    }

    private static Vector2Int TurnLeft(Vector2Int orig)
    {
        return new Vector2Int(-orig.y, orig.x);
    }

    private static Vector2Int TurnRight(Vector2Int orig)
    {
        return new Vector2Int(orig.y, -orig.x);
    }

    private static bool GenerateSmallRoad(ref bool[,] layout, ref int numRoads, int depth, Vector2Int position, Vector2Int direction)
    {
        if (depth == -1) return true;
        if (depth < 3 && Random.Range(0,3) == 0) return true;

        position += direction;

        if (layout[position.x, position.y]) return true;

        Vector2Int leftPos = position + TurnLeft(direction);
        Vector2Int dleftPos = position + 2 * TurnLeft(direction);
        Vector2Int rightPos = position + TurnRight(direction);
        Vector2Int drightPos = position + 2 * TurnRight(direction);

        Vector2Int forPos = position + direction;

        Vector2Int fleftPos = position + TurnLeft(direction) + direction;
        Vector2Int frightPos = position + TurnRight(direction) + direction;

        if (layout[leftPos.x, leftPos.y]) return false;
        if (layout[dleftPos.x, dleftPos.y]) return false;
        if (layout[rightPos.x, rightPos.y]) return false;
        if (layout[drightPos.x, drightPos.y]) return false;
        if (!layout[forPos.x, forPos.y])
        {
            if (layout[fleftPos.x, fleftPos.y]) return false;
            if (layout[frightPos.x, frightPos.y]) return false;
        }

        if (GenerateSmallRoad(ref layout, ref numRoads, depth - 1, new Vector2Int(position.x, position.y), direction))
        {
            if (position.x > 50 - townRadius && position.x < 50 + townRadius && position.y > 50 - townRadius && position.y < 50 + townRadius) numRoads++;
            layout[position.x, position.y] = true;
            return true;
        }
        else if (depth < 3)
        {
            return true;
        }

        return false;
    }

    private static Vector2Int RandomRoadPosition(ref bool[,] layout, ref int numRoads)
    {
        int randRoad = Random.Range(0, numRoads);

        for (int y = 50 - townRadius + 1; y < 50 + townRadius; y++)
        {
            for (int x = 50 - townRadius + 1; x < 50 + townRadius; x++)
            {
                if (layout[x, y]) randRoad--;
                if (randRoad == -1)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return new Vector2Int(50, 50);
    }

    private static void GenerateMainRoad(Vector2Int direction, ref bool[,] layout, ref int numRoads)
    {
        Vector2Int position = new Vector2Int(50, 50);
        int turns = Random.Range(4, 8);
        Vector2Int currentDirection = direction;
        for (int i = 0; i < turns; i++)
        {
            int len = 0;
            if (i > 0) len = Random.Range(3, 6);
            else len = Random.Range(7, 11);
            for (int j = 0; j < len; j++)
            {
                position += currentDirection;
                layout[position.x, position.y] = true;
                if (position.x > 50 - townRadius && position.x < 50 + townRadius && position.y > 50 - townRadius && position.y < 50 + townRadius) numRoads++;
            }

            // Direction generating
            if (currentDirection == direction)
            {
                int nDir = Random.Range(0, 3);
                if (nDir == 0)
                {
                    currentDirection = TurnLeft(direction);
                }
                else if (nDir == 1)
                {
                    currentDirection = direction;
                }
                else if (nDir == 2)
                {
                    currentDirection = TurnRight(direction);
                }
            }
            else
            {
                currentDirection = direction;
            }
        }

        while (position.x > 0 && position.x < 99 && position.y > 0 && position.y < 99)
        {
            position += direction;
            layout[position.x, position.y] = true;
            if (position.x > 50 - townRadius && position.x < 50 + townRadius && position.y > 50 - townRadius && position.y < 50 + townRadius) numRoads++;
        }

    }
}
