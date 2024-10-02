using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField]
    private float maxPowerDistance = 1f;

    [SerializeField]
    private float maxBallSpeed = 5f;

    private float shotSpeed;

    private Vector3 startHoldMousePos;
    private Vector2 shotDirection;
    private float shotPower;

    private bool isHolding;
    private bool isMoving;
    private bool hasHitBubble;

    public static event Action onTurnFinished;

    private void Awake()
    {
        shotSpeed = 0f;
        shotPower = 0f;

        startHoldMousePos = Vector3.zero;
        shotDirection = Vector2.zero;

        isHolding = false;
        isMoving = false;
        hasHitBubble = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving && !hasHitBubble)
        {
            Vector3 velocity = shotDirection * shotSpeed;
            Vector3 displacement = velocity * Time.deltaTime;

            /*if (!World.instance.gameField.Contains(transform.position + displacement))
            {
                shotDirection = Vector2.Reflect();
                Debug.Log(shotDirection);
                return;
            }*/

            transform.position += displacement;
            return;
        }

        if (Input.GetMouseButtonDown(0) && !isHolding)
        {
            startHoldMousePos = Input.mousePosition;
            startHoldMousePos.z = Camera.main.nearClipPlane;
            startHoldMousePos = Camera.main.ScreenToWorldPoint(startHoldMousePos);

            Debug.Log(startHoldMousePos);
            isHolding = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector3 currMousePos = Input.mousePosition;
            currMousePos.z = Camera.main.nearClipPlane;
            currMousePos = Camera.main.ScreenToWorldPoint(currMousePos);
            Debug.Log(currMousePos);

            shotDirection = -(currMousePos - startHoldMousePos);
            Debug.Log(shotDirection);

            float holdDistance = (currMousePos - startHoldMousePos).magnitude;
            shotPower = holdDistance < maxPowerDistance ? holdDistance : maxPowerDistance;
            shotSpeed = maxBallSpeed * shotPower;

            isHolding = false;
            isMoving = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            shotDirection = Vector2.Reflect(shotDirection, collision.contacts[0].normal);
        }
        else if (collision.gameObject.CompareTag("Bubble") && hasHitBubble != true)
        {
            hasHitBubble = true;
            SpringJoint2D joint = gameObject.AddComponent<SpringJoint2D>();
            joint.dampingRatio = 0.85f;
            joint.autoConfigureDistance = false;
            joint.distance = 0.05f;

            if (shotPower == maxPowerDistance)
            {
                SpringJoint2D bubbleJoint = collision.gameObject.GetComponent<SpringJoint2D>();
                joint.connectedAnchor = bubbleJoint.connectedAnchor;
                // todo: set active = false

                Bubble destroyedBubble = collision.gameObject.GetComponent<Bubble>();
                Bubble newBubble = gameObject.GetComponent<Bubble>();
                newBubble.neighbours = destroyedBubble.neighbours;
                BubbleSpawner.instance.AddBubbleToField(newBubble);

                destroyedBubble.DeclareRemoveToNeighbours();
                Destroy(collision.gameObject);
                
                newBubble.DeclareToNeighbours();

                Queue<Bubble> destroyQueue = new Queue<Bubble>();
                newBubble.GetDestroyQueue(ref destroyQueue, newBubble.type);

                if (destroyQueue.Count > 2)
                {
                    StartCoroutine(DestroySameBubbles(destroyQueue));
                    //DestroySameBubbles(destroyQueue);
                }
                
            }
            else
            {
                ContactPoint2D point = collision.contacts[0];
                Vector2 hitBubblePos = collision.gameObject.transform.position;
                if (point.point.y < hitBubblePos.y)
                {
                    joint.connectedAnchor = new Vector2(hitBubblePos.x + 0.25f, hitBubblePos.y - 0.5f);
                }
                else
                {
                    joint.connectedAnchor = new Vector2(hitBubblePos.x + 0.5f, hitBubblePos.y);
                }
            }
            onTurnFinished.Invoke();
        }

    }

    public IEnumerator DestroySameBubbles(Queue<Bubble> destroyQueue)
    {
        yield return new WaitForSeconds(0.2f);
        destroyQueue.Dequeue();
        GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.2f);

        while (destroyQueue.Count > 0)
        {
            Destroy(destroyQueue.Dequeue().gameObject);
            yield return new WaitForSeconds(0.2f);
        }
        Destroy(gameObject);
    }
}
