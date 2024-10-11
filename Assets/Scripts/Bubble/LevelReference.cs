using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public struct LevelReference
{
    public List<Bubble> allBubbles;

    public CoreRow coreRow;

    public LevelReference(List<Bubble> allBubbles, CoreRow core)
    {
        this.allBubbles = allBubbles;
        this.coreRow = core;
    }
    /*public void Replace(Bubble destroyedBubble, Bubble newBubble)
    {
        foreach (List<Bubble> row in reference)
        {
            int destroyedBubbleIndex = row.FindIndex(bubble => bubble.id == destroyedBubble.id);
            if (destroyedBubbleIndex == -1)
                continue;
            else
            {
                row.Remove(destroyedBubble);
                row.Insert(destroyedBubbleIndex, newBubble);
                break;
            }
        }
    }*/

    public void Replace(Bubble destroyedBubble, Bubble newBubble)
    {
        int destroyedBubbleIndex = allBubbles.FindIndex(bubble => bubble.id == destroyedBubble.id);
        allBubbles.Remove(destroyedBubble);
        allBubbles.Insert(destroyedBubbleIndex, newBubble);

        if (coreRow.CoreBubbles.FindIndex(bubble => bubble.id == destroyedBubble.id) != -1)
        {
            allBubbles.Remove(destroyedBubble);
            allBubbles.Insert(destroyedBubbleIndex, newBubble);
        }
    }

    /*public void Remove(Bubble destroyedBubble)
    {
        foreach (List<Bubble> row in reference)
        {
            bool result = row.Remove(destroyedBubble);
            if (result)
                break;
        }
    }*/

    public void Remove(Bubble destroyedBubble)
    {
        allBubbles.Remove(destroyedBubble);
        coreRow.CoreBubbles.Remove(destroyedBubble);
    }

    /*public void AddRow(List<Bubble> row)
    {
        reference.Add(row);
    }*/

    public void Add(Bubble newBubble)
    {
        allBubbles.Add(newBubble);
    }

    public void SetCoreRow(List<Bubble> _coreRow)
    {
        coreRow.CoreBubbles = _coreRow;
        coreRow.startAmount = _coreRow.Count;
    }

    /*public List<Bubble> GetNotAttachedToCoreBubbles()
    {
        List<Bubble> result = new List<Bubble>();
        foreach (List<Bubble> row in reference)
        {
            foreach (Bubble bubble in row)
            {
                if (!bubble.hasWayToCore)
                {
                    result.Add(bubble);
                }
            }
        }
        return result;
    }*/

    public List<Bubble> GetNotAttachedToCoreBubbles()
    {
        List<Bubble> result = new List<Bubble>();
        foreach (Bubble bubble in allBubbles)
        {
            if (!bubble.hasWayToCore)
                result.Add(bubble);
        }
        return result;
    }
}
