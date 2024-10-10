using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float maxPullPower;
    public float maxSpreadAngle = 20f;

    [HideInInspector]
    public Vector2 shotDirection;
    [HideInInspector]
    public float pullPower;

    private Vector3 startHoldMousePos;
    private Vector3 currMousePos;
    private Vector3 pullVector;
    private bool isHolding;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isHolding)
        {
            isHolding = true;

            ToggleTrajectoryRendering(true);

            startHoldMousePos = Input.mousePosition;
            startHoldMousePos.z = Camera.main.nearClipPlane;
            startHoldMousePos = Camera.main.ScreenToWorldPoint(startHoldMousePos);

            Debug.Log(startHoldMousePos);
        }

        if (isHolding)
        {
            currMousePos = Input.mousePosition;
            currMousePos.z = Camera.main.nearClipPlane;
            currMousePos = Camera.main.ScreenToWorldPoint(currMousePos);

            pullVector = -(currMousePos - startHoldMousePos);

            pullPower = (currMousePos - startHoldMousePos).magnitude;
            pullPower = Math.Min(pullPower, maxPullPower);

            shotDirection = pullVector.normalized;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (pullPower == maxPullPower)
            {
                float randomAngle = UnityEngine.Random.Range(-maxSpreadAngle, maxSpreadAngle);
                shotDirection = Quaternion.Euler(0f, 0f, randomAngle) * shotDirection;
            } 

            gameObject.GetComponent<BubbleMovement>().enabled = true;
            ToggleTrajectoryRendering(false);
            this.enabled = false;
        }
    }

    private void ToggleTrajectoryRendering(bool value)
    {
        GetComponent<TrajectoryRenderer>().enabled = value;
    }
}
