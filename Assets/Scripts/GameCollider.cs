using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameCollider : MonoBehaviour
{
    Camera camera;
    void Awake()
    {
        camera = Camera.main;
        
        Vector2[] boundaries = GetWorldBoundaries();
        CreateCollider(boundaries);
    }

    private Vector2[] GetWorldBoundaries()
    {
        if (camera == null || !camera.orthographic)
        {
            return new Vector2[0];
        }

        Vector3 bottomLeft = camera.ScreenToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        Vector3 topLeft = camera.ScreenToWorldPoint(new Vector3(0, camera.pixelHeight, camera.nearClipPlane));
        Vector3 bottomRight = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, 0, camera.nearClipPlane));
        Vector3 topRight = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, camera.pixelHeight, camera.nearClipPlane));

        Vector2[] result = new Vector2[]{bottomLeft, topLeft, topRight, bottomRight, bottomLeft};
        return result;
    }

    private void CreateCollider(Vector2[] boundaries)
    {
        // TODO: naming
        EdgeCollider2D collider = gameObject.AddComponent<EdgeCollider2D>();
        collider.points = boundaries;
    }
}
