using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BubbleSpawner : MonoBehaviour
{
    [HideInInspector]
    public static BubbleSpawner instance;
    
    private GameObject field;

    private Vector2 startPosLeft = new Vector2(-2.5f, 4f);
    private float bubleDensity = 0.5f;

    // TODO: rename that shit
    private float test = 0.25f;

    [SerializeField]
    private GameObject bubblePrefab;
    [SerializeField]
    private GameObject shootingBubblePrefab;

    Vector2 currShootingBubblePos;
    Vector2 nextShootingBubblePos;

    private int rows = 2;
    private int cols = 8;

    private GameObject currShootingBubble;
    private GameObject nextShootingBubble;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        field = GameObject.Find("BubbleField");

        SetShootingBubblesPositions();
        SpawnShootingBubbles();

        SpawnBubbles();
        InitBubblesNeighbours();

        BallController.onTurnFinished += OnTurnFinished;
    }

    private void SetShootingBubblesPositions()
    {
        GameObject currShootingBubbleExample = GameObject.Find("Current Shooting Bubble Position");
        GameObject nextShootingBubbleExample = GameObject.Find("Next Shooting Bubble Position");

        currShootingBubblePos = currShootingBubbleExample.transform.position;
        nextShootingBubblePos = nextShootingBubbleExample.transform.position;

        //TODO: move to another function?
        currShootingBubbleExample.SetActive(false);
        nextShootingBubbleExample.SetActive(false);
    }

    private void SpawnShootingBubbles()
    {
        currShootingBubble = Instantiate(shootingBubblePrefab, currShootingBubblePos, Quaternion.identity);
        nextShootingBubble = Instantiate(bubblePrefab, nextShootingBubblePos, Quaternion.identity);

        //TODO: restructure prefabs
        Destroy(nextShootingBubble.GetComponent<SpringJoint2D>());
    }

    private void InitBubblesNeighbours()
    {
        Bubble[] bubbles = field.GetComponentsInChildren<Bubble>();

        foreach (Bubble bubble in bubbles)
        {
            bubble.InitNeighbours();
        }
    }

    private void SpawnBubbles()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector2 bubblePos = new Vector2(startPosLeft.x + bubleDensity * j, startPosLeft.y - bubleDensity * i);
                GameObject bubbleObj = Instantiate(bubblePrefab, bubblePos, Quaternion.identity, field.transform);
                SpringJoint2D joint = bubbleObj.GetComponent<SpringJoint2D>();

                bubbleObj.name = $"{@i}, {@j}";
                Bubble bubbleInfo = bubbleObj.GetComponent<Bubble>();
                if (i == 0)
                {
                    bubbleInfo.isCore = true;
                }

                joint.connectedAnchor = bubblePos;
                joint.dampingRatio = 1;
            }

            if (i % 2 == 1)
            {
                startPosLeft.x -= test;
            }
            else
            {
                startPosLeft.x += test;
            }
        }
    }

    public IEnumerator DestroySameBubbles(Queue<Bubble> destroyQueue)
    {
        for (int i = 0; i < destroyQueue.Count; i++) 
        {
            Destroy(destroyQueue.Dequeue().gameObject);
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddBubbleToField(Bubble bubble)
    {
        bubble.transform.SetParent(field.transform);
    }

    private void BubbleServiceFieldsReset()
    {
        Bubble[] bubbles = field.GetComponentsInChildren<Bubble>();
        foreach (Bubble bubble in bubbles)
        {
            bubble.ResetBFSFields();
        }
    }

    private void OnTurnFinished()
    {
        currShootingBubble = nextShootingBubble;
        currShootingBubble.transform.position = currShootingBubblePos;
        currShootingBubble.AddComponent<BallController>();

        nextShootingBubble = Instantiate(bubblePrefab, nextShootingBubblePos, Quaternion.identity);
        Destroy(nextShootingBubble.GetComponent<SpringJoint2D>());

        BubbleServiceFieldsReset();
    }
}
