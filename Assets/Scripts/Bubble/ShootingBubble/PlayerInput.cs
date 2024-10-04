using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float maxPullPower;

    [HideInInspector]
    public Vector2 shotDirection;
    [HideInInspector]
    public float pullPower;

    private Vector3 startHoldMousePos;
    private bool isHolding;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isHolding)
        {
            isHolding = true;

            startHoldMousePos = Input.mousePosition;
            startHoldMousePos.z = Camera.main.nearClipPlane;
            startHoldMousePos = Camera.main.ScreenToWorldPoint(startHoldMousePos);

            Debug.Log(startHoldMousePos);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector3 currMousePos = Input.mousePosition;
            currMousePos.z = Camera.main.nearClipPlane;
            currMousePos = Camera.main.ScreenToWorldPoint(currMousePos);

            shotDirection = -(currMousePos - startHoldMousePos);

            pullPower = Math.Min((currMousePos - startHoldMousePos).magnitude, maxPullPower);

            gameObject.GetComponent<BubbleMovement>().enabled = true;
            this.enabled = false;
        }
    }
}
