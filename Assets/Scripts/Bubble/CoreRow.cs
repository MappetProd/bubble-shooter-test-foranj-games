using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CoreRow
{
    public List<Bubble> CoreBubbles;
    public int startAmount;

    private void MarkNeighboursAsPathToCore(Bubble bubble)
    {
        bubble.hasWayToCore = true;
        foreach (Bubble neighbour in bubble.neighbours)
        {
            if (!neighbour.hasWayToCore)
                MarkNeighboursAsPathToCore(neighbour);
        }
    }

    public bool IsCoreRowDestroyed()
    {
        if (CoreBubbles.Count * 100 / startAmount >= 30)
            return true;
        else
            return false;
    }

    public void MarkBubblesThatHaveWayToCore()
    {
        foreach (Bubble coreBubble in CoreBubbles)
        {
            MarkNeighboursAsPathToCore(coreBubble);
        }
    }
}
