using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public enum BubbleType
    {
        RED, GREEN, BLUE
    }

    public List<Bubble> neighbours;
    public BubbleType type;
    public bool isCore;

    public bool hasWayToCore;
    public bool isVisited;

    private SpriteRenderer sp;

    void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        neighbours = new List<Bubble>();
        isCore = false;
        hasWayToCore = true;
        Array values = Enum.GetValues(typeof(BubbleType));
        type = (BubbleType)values.GetValue(UnityEngine.Random.Range(0, values.Length));

        switch (type)
        {
            case BubbleType.RED:
                sp.color = Color.red;
                break;
            case BubbleType.GREEN:
                sp.color = Color.green;
                break;
            case BubbleType.BLUE:
                sp.color = Color.blue;
                break;
            default:
                sp.color = Color.white;
                break;
        }
    }

    public void InitNeighbours()
    {
        // TODO: remove hardcode (1f = 0.5f (bubble radius) * 2)
        gameObject.layer = 6;
        Collider2D[] overlapColliders = Physics2D.OverlapCircleAll(transform.position, 0.5f, LayerMask.GetMask("Default"));
        foreach (Collider2D collider in overlapColliders)
        {
            neighbours.Add(collider.gameObject.GetComponent<Bubble>());
        }
        gameObject.layer = 0;
    }

    public Queue<Bubble> GetDestroyQueue(ref Queue<Bubble> destroyQueue, BubbleType type)
    {
        isVisited = true;
        if (type == this.type)
        {
            destroyQueue.Enqueue(this);
            foreach (Bubble neighbour in neighbours)
            {
                if (!neighbour.isVisited)
                {
                    neighbour.GetDestroyQueue(ref destroyQueue, type);
                }
            }
        }
        
        return destroyQueue;
    }

    public void DeclareToNeighbours()
    {
        foreach (Bubble neighbour in neighbours)
        {
            neighbour.neighbours.Add(this);
        }
    }

    public void DeclareRemoveToNeighbours()
    {
        foreach (Bubble neighbour in neighbours)
        {
            neighbour.neighbours.Remove(this);
        }
    }

    public void ResetBFSFields()
    {
        isVisited = false;
    }
}
