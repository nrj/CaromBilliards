using System;

// Describes a ball collision

public struct BallCollisionVO
{
    public BallType srcType;
    public BallType dstType;
    public int srcId;
    public int dstId;
    public float magnitude;
}
