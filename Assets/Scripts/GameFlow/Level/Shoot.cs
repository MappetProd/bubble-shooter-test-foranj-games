using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : BaseState
{
    private LevelSM _sm;

    public Shoot(string name, LevelSM stateMachine) : base("Shoot", stateMachine)
    {
        this.name = name;
        _sm = stateMachine;
    }

    public override void Enter() 
    {
        PrepareCurrentShootingBubble();
        PrepareNextShootingBubble();
    }


    private void PrepareNextShootingBubble()
    {
        _sm.nextShootingBubble = SpawnBubble(Bubble.GetRandomColor(), _sm.nextShootingBubblePos);
        _sm.nextShootingBubble.GetComponent<PlayerInput>().enabled = false;
        GameObject.Destroy(_sm.nextShootingBubble.GetComponent<SpringJoint2D>());
    }

    private void PrepareCurrentShootingBubble()
    {
        _sm.nextShootingBubble.transform.position = _sm.currShootingBubblePos;
        _sm.currShootingBubble = _sm.nextShootingBubble;
        _sm.currShootingBubble.GetComponent<PlayerInput>().enabled = true;
    }

    private GameObject SpawnBubble(BubbleColor bubbleColor, Vector2 pos)
    {
        GameObject bubbleObject = GameObject.Instantiate(_sm.shootingBubblePrefab, pos, Quaternion.identity);
        Bubble bubble = bubbleObject.GetComponent<Bubble>();
        bubble.InitColor(bubbleColor);
        bubbleObject.transform.position = pos;
        return bubbleObject;
    }

    public virtual void Exit() { }
}
