using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllingBubblesSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject shootingBubblePrefab;

    private GameObject currShootingBubble;
    private GameObject nextShootingBubble;
    private Vector2 currShootingBubblePos;
    private Vector2 nextShootingBubblePos;

    private void Start()
    {
        SetShootingBubblesPositions();
        SpawnShootingBubbles();
        BubbleLevel.Updated += LoadNewBubbles;
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
        currShootingBubble = SpawnBubble(Bubble.GetRandomColor(), currShootingBubblePos);
        nextShootingBubble = SpawnBubble(Bubble.GetRandomColor(), nextShootingBubblePos);
        nextShootingBubble.GetComponent<PlayerInput>().enabled = false;

        //TODO: restructure prefabs
        Destroy(nextShootingBubble.GetComponent<SpringJoint2D>());
    }

    public GameObject SpawnBubble(BubbleColor bubbleColor, Vector2 pos)
    {
        GameObject bubbleObject = Instantiate(shootingBubblePrefab, pos, Quaternion.identity);
        Bubble bubble = bubbleObject.GetComponent<Bubble>();
        bubble.InitColor(bubbleColor);
        bubbleObject.transform.position = pos;
        return bubbleObject;
    }

    private void LoadNewBubbles()
    {
        nextShootingBubble.transform.position = currShootingBubblePos;
        currShootingBubble = nextShootingBubble;
        currShootingBubble.GetComponent<PlayerInput>().enabled = true;

        nextShootingBubble = SpawnBubble(Bubble.GetRandomColor(), nextShootingBubblePos);
        nextShootingBubble.GetComponent<PlayerInput>().enabled = false;
        Destroy(nextShootingBubble.GetComponent<SpringJoint2D>());
    }
}
