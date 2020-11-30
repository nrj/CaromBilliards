using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameTurnVO
{
    public UnityEngine.Vector3 shotVector;  // zero, if shot not yet taken
    public List<(int, Vector3)> ballStartPositions;
    public List<BallType> ballsHit;

    public GameTurnVO(List<Ball> balls)
    {
        shotVector = Vector3.zero;
        ballStartPositions = balls.Select(b => (b.uid, b.transform.position)).ToList();
        ballsHit = new List<BallType>();
    }
}