using System.Collections.Generic;
using UnityEngine;

public enum GameRound : byte
{
    None, 
    StationaryBaskets,
    DistanceBaskets,
    MovingBaskets,
    RevolvingBaskets
}

[System.Serializable]
public class GameRoundData
{
    public GameRound round;
    public int roundScore;
    public int pointsPerBasket;
    public float distanceFromParent;
    public Vector3 parentPosition;
    public List<Vector3> basketLocalPositions;
}
