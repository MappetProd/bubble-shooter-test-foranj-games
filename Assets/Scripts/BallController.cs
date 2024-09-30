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
    private float shotSpeed = 0f;

    private Vector3 startHoldMousePos = Vector3.zero;
    private Vector2 shotDirection = Vector2.zero;
    private float shotPower = 0f;

    private bool isHolding = false;
    private bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
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
        shotDirection = Vector2.Reflect(shotDirection, collision.contacts[0].normal);
    }

    private void OnMouseOver()
    {
        
    }
}
