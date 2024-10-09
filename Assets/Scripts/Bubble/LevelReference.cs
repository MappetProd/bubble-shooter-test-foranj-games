using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public struct LevelReference
{
    public List<List<Bubble>> reference;

    public CoreRow coreRow;

    public LevelReference(List<List<Bubble>> ok, CoreRow core)
    {
        this.reference = ok;
        this.coreRow = core;
    }
    public void Replace(Bubble destroyedBubble, Bubble newBubble)
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
    }

    public void Remove(Bubble destroyedBubble)
    {
        foreach (List<Bubble> row in reference)
        {
            bool result = row.Remove(destroyedBubble);
            if (result)
                break;
        }
    }

    public void AddRow(List<Bubble> row)
    {
        reference.Add(row);
    }

    public void SetCoreRow()
    {
        coreRow.CoreBubbles = reference[0];
        coreRow.startAmount = reference[0].Count;
    }

    public List<Bubble> GetNotAttachedToCoreBubbles()
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
    }
}
