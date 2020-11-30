using System.Collections.Generic;

public class GameVO
{
    public List<GameTurnVO> turns = new List<GameTurnVO>();
    public int totalPoints = 0;
    public int timeInSeconds = 0;
    public bool isOver = false;
}