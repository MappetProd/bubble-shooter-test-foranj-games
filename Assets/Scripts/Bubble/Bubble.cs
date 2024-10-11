using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Converter;

public class Bubble : MonoBehaviour
{
    [HideInInspector]
    public Guid id;

    [HideInInspector]
    public List<Bubble> neighbours;

    [HideInInspector]
    public BubbleColor type;

    [HideInInspector]
    public bool hasWayToCore;

    [HideInInspector]
    public bool isVisitedDestroyBFS;

    public int points;

    private SpriteRenderer sp;

    public static Action<int> BubbleDestroyed;

    void Awake()
    {
        id = Guid.NewGuid();
        sp = GetComponent<SpriteRenderer>();

        neighbours = new List<Bubble>();
        hasWayToCore = true;
    }

    public static BubbleColor GetRandomColor()
    {
        List<BubbleColor> values = Enum.GetValues(typeof(BubbleColor)).OfType<BubbleColor>().ToList();
        values.Remove(BubbleColor.RANDOM);
        values.Remove(BubbleColor.VOID);
        BubbleColor randomColor = (BubbleColor)values[UnityEngine.Random.Range(0, values.Count)];
        return randomColor;
    }

    public void InitColor(BubbleColor bubbleColor)
    {
        type = bubbleColor;
        switch (type)
        {
            case BubbleColor.RED:
                sp.color = Color.red;
                break;
            case BubbleColor.GREEN:
                sp.color = Color.green;
                break;
            case BubbleColor.BLUE:
                sp.color = Color.blue;
                break;
            default:
                throw new Exception($"There is no such BubbleColor as {nameof(bubbleColor)}");
        }
    }

    public void InitNeighbours()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Overlap");

        int defaultLayerMask = LayerMask.GetMask("Default");
        float bubbleRadius = GetComponent<CircleCollider2D>().radius;

        Collider2D[] overlapColliders = Physics2D.OverlapCircleAll(transform.position, bubbleRadius, defaultLayerMask);
        foreach (Collider2D collider in overlapColliders)
        {
            neighbours.Add(collider.gameObject.GetComponent<Bubble>());
        }

        gameObject.layer = 0;
    }

    /*private void OnDrawGizmos()
    {
        float bubbleRadius = GetComponent<CircleCollider2D>().radius;
        Gizmos.DrawWireSphere(transform.position, bubbleRadius);
    }*/

    public void GetDestroyQueue(ref Queue<Bubble> destroyQueue, BubbleColor type)
    {
        isVisitedDestroyBFS = true;
        if (type == this.type)
        {
            destroyQueue.Enqueue(this);
            foreach (Bubble neighbour in neighbours)
            {
                if (!neighbour.isVisitedDestroyBFS)
                {
                    neighbour.GetDestroyQueue(ref destroyQueue, type);
                }
            }
        }
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
        isVisitedDestroyBFS = false;
        hasWayToCore = false;
    }

    public void SetSpringJointPositionByBubble(Bubble bubble)
    {
        SpringJoint2D controlledBubbleJoint = gameObject.GetComponent<SpringJoint2D>();
        SpringJoint2D collidedBubbleJoint = bubble.gameObject.GetComponent<SpringJoint2D>();
        controlledBubbleJoint.connectedAnchor = collidedBubbleJoint.connectedAnchor;
    }

    public void SetSpringJointPositionByCollision(Collision2D collision)
    {
        SpringJoint2D controlledBubbleJoint = gameObject.GetComponent<SpringJoint2D>();

        ContactPoint2D point = collision.contacts[0];
        Vector2 hitBubblePos = collision.gameObject.transform.position;

        if (point.point.y < hitBubblePos.y)
            controlledBubbleJoint.connectedAnchor = new Vector2(hitBubblePos.x + 0.25f, hitBubblePos.y - 0.5f);
        else
            controlledBubbleJoint.connectedAnchor = new Vector2(hitBubblePos.x + 0.5f, hitBubblePos.y);

    }
}
