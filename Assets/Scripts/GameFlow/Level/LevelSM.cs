using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSM : StateMachine
{
    public BubbleLevel level;
    public CoreBubbleRow row;

    public GameObject shootingBubblePrefab;

    public GameObject currShootingBubble;
    public GameObject nextShootingBubble;

    public Vector2 currShootingBubblePos;
    public Vector2 nextShootingBubblePos;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
    }
}
