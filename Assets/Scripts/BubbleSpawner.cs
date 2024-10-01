using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BubbleSpawner : MonoBehaviour
{
    public static BubbleSpawner instance;
    private GameObject field;

    private Vector2 startPosLeft = new Vector2(-2.5f, 4f);
    private float bubleDensity = 0.5f;
    private float test = 0.25f;

    [SerializeField]
    private GameObject bubblePrefab;

    private int rows = 2;
    private int cols = 8;

    // Start is called before the first frame update

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

        SpawnBubbles();
        InitBubblesNeighbours();
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
                Bubble bubbleInfo = bubbleObj.AddComponent<Bubble>();
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
}
