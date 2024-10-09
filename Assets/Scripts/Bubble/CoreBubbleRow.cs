using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CoreBubbleRow : MonoBehaviour
{
    private static CoreBubbleRow _instance;
    public static CoreBubbleRow Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("CoreRow").AddComponent<CoreBubbleRow>();
            }
            return _instance;
        }
    }

    private int startAmount;
    public List<Bubble> CoreBubbles { get; private set; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this);

        CoreBubbles = new List<Bubble>();
    }


    private void MarkNeighboursAsPathToCore(Bubble bubble)
    {
        bubble.hasWayToCore = true;
        foreach (Bubble neighbour in bubble.neighbours)
        {
            if (!neighbour.hasWayToCore)
                MarkNeighboursAsPathToCore(neighbour);
        }
    }

    public void ReplaceCoreBubble(Bubble destroyedCoreBubble, Bubble newCoreBubble)
    {
        int destroyedBubbleIndex = CoreBubbles.FindIndex(bubble => bubble.id == destroyedCoreBubble.id);
        CoreBubbles.Remove(destroyedCoreBubble);
        CoreBubbles.Insert(destroyedBubbleIndex, newCoreBubble);
    }

    public void RemoveCoreBubble(Bubble coreBubble)
    {
        CoreBubbles.Remove(coreBubble);
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
