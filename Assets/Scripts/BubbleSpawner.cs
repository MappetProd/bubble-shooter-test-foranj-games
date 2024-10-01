using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    private Vector2 startPosLeft = new Vector2(-2.5f, 4f);
    private float bubleDensity = 0.5f;
    private float test = 0.25f;

    [SerializeField]
    private GameObject bubblePrefab;

    private int rows = 2;
    private int cols = 8;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector2 bubblePos = new Vector2(startPosLeft.x + bubleDensity * j, startPosLeft.y - bubleDensity * i);
                GameObject bubble = Instantiate(bubblePrefab, bubblePos, Quaternion.identity);
                SpringJoint2D joint = bubble.GetComponent<SpringJoint2D>();
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
