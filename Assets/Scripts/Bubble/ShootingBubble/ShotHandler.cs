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
            BubbleLevel.Instance.RemoveBubble(bubbleToDestroy);
            //yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bubble") && !hasHit)
        {
            hasHit = true;

            AddSpringJoint();
            Bubble collidedBubble = collision.gameObject.GetComponent<Bubble>();

            if (playerInput.pullPower == playerInput.maxPullPower)
                BubbleLevel.Instance.ReplaceBubble(collidedBubble, this.bubble);
            else
                BubbleLevel.Instance.AddBubble(collision, this.bubble);

            Queue<Bubble> destroyQueue = new Queue<Bubble>();
            HideFirstBubble(destroyQueue);
            bubble.GetDestroyQueue(ref destroyQueue, bubble.type);

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
