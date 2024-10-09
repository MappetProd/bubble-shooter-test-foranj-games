using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShotHandler : MonoBehaviour
{
    private Bubble bubble;
    private PlayerInput playerInput;
    private bool hasHit;

    public static event Action ShotHandled;

    private void Awake()
    {
        hasHit = false;
        bubble = GetComponent<Bubble>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void AddSpringJoint()
    {
        SpringJoint2D joint = gameObject.AddComponent<SpringJoint2D>();
        //Preset preset = AssetDatabase.LoadAssetAtPath<Preset>("Assets/Presets/BubbleSpringJoint.preset");
        //preset.ApplyTo(joint);
        joint.autoConfigureDistance = false;
        joint.distance = 0.005f;
        joint.dampingRatio = 0.8f;
        joint.frequency = 0.5f;
    }

    private void HandleMaxPowerShot(Bubble afftectedBubble)
    {
        BubbleLevel.Instance.ReplaceBubble(afftectedBubble, bubble);

        SpringJoint2D controlledBubbleJoint = gameObject.GetComponent<SpringJoint2D>();
        SpringJoint2D bubbleJoint = afftectedBubble.gameObject.GetComponent<SpringJoint2D>();
        controlledBubbleJoint.connectedAnchor = bubbleJoint.connectedAnchor;

        bubble.neighbours = afftectedBubble.neighbours;
        afftectedBubble.Destroy();
    }

    private void SetPositionNextToCollidedBubble(Collision2D collision)
    {
        SpringJoint2D controlledBubbleJoint = gameObject.GetComponent<SpringJoint2D>();
        
        ContactPoint2D point = collision.contacts[0];
        Vector2 hitBubblePos = collision.gameObject.transform.position;
        
        if (point.point.y < hitBubblePos.y)
            controlledBubbleJoint.connectedAnchor = new Vector2(hitBubblePos.x + 0.25f, hitBubblePos.y - 0.5f);
        else
            controlledBubbleJoint.connectedAnchor = new Vector2(hitBubblePos.x + 0.5f, hitBubblePos.y);
        
    }

    private IEnumerator HideFirstBubble(Queue<Bubble> destroyQueue)
    {
        yield return new WaitForSeconds(0.2f);
        destroyQueue.Dequeue();
        GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.2f);
    }

    private void DestroySameBubbles(Queue<Bubble> destroyQueue)
    {
        while (destroyQueue.Count > 0)
        {
            Bubble bubbleToDestroy = destroyQueue.Dequeue();
            bubbleToDestroy.Destroy();
            //yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bubble") && !hasHit)
        {
            hasHit = true;

            AddSpringJoint();
            if (playerInput.pullPower == playerInput.maxPullPower)
                HandleMaxPowerShot(collision.gameObject.GetComponent<Bubble>());
            else
            {
                SetPositionNextToCollidedBubble(collision);
                bubble.InitNeighbours();
            }

            bubble.DeclareToNeighbours();

            Queue<Bubble> destroyQueue = new Queue<Bubble>();

            HideFirstBubble(destroyQueue);
            destroyQueue = bubble.GetDestroyQueue(ref destroyQueue, bubble.type);

            if (destroyQueue.Count > 2)
            {
                //StartCoroutine(DestroySameBubbles(destroyQueue));
                DestroySameBubbles(destroyQueue);
            }

            ShotHandled.Invoke();
            this.enabled = false;
        }
    }
}
