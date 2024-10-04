using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleMovement : MonoBehaviour
{
    [SerializeField]
    private float bubbleSpeed;

    private Vector2 shotDirection;
    
    private PlayerInput playerInput;
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 velocity = playerInput.shotDirection * bubbleSpeed * playerInput.pullPower;
        Vector3 displacement = velocity * Time.deltaTime;
        transform.position += displacement;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            playerInput.shotDirection = Vector2.Reflect(playerInput.shotDirection, collision.contacts[0].normal);
        }
        else if (collision.gameObject.CompareTag("Bubble"))
        {
            this.enabled = false;
        }

    }
}
