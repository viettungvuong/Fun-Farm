using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    public Tilemap tilemap;

    public float smoothSpeed = 0.125f;  
    public Vector3 offset;  // offset relative to the player position

    Vector2 minBoundary; 
    Vector2 maxBoundary;

    float cameraHeight;
    float cameraWidth;

    private Camera mainCamera;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = GetComponent<Camera>();

        BoundsInt bounds = tilemap.cellBounds;

        // Convert the bounds to world space
        Vector3Int min = bounds.min;
        Vector3Int max = bounds.max;

        Vector3 minWorld = tilemap.CellToWorld(min);
        Vector3 maxWorld = tilemap.CellToWorld(max);

        // Find boundaries
        minBoundary = new Vector2(minWorld.x, minWorld.y);
        maxBoundary = new Vector2(maxWorld.x, maxWorld.y);

        // Find camera height and width
        cameraHeight = 2f * mainCamera.orthographicSize; 
        cameraWidth = cameraHeight * mainCamera.aspect;
    }

    void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        float clampedX = Mathf.Clamp(player.position.x + offset.x*1.3f, minBoundary.x + cameraWidth / 2, maxBoundary.x - cameraWidth / 2);
        float clampedY = Mathf.Clamp(player.position.y + offset.y*1.3f, minBoundary.y + cameraHeight / 2, maxBoundary.y - cameraHeight / 2);

        Vector3 clampedPosition = new Vector3(clampedX, clampedY, transform.position.z); // keep within boundaries
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}