using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    public Tilemap tilemap;

    public float smoothSpeed = 0.125f;  
    public Vector3 offset;  // offset so voi vi tri player

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

        // tim boundary
        minBoundary = new Vector2(minWorld.x, minWorld.y);
        maxBoundary = new Vector2(maxWorld.x, maxWorld.y);

        // tim chieu dai va chieu cao cua camera
        cameraHeight = 2f * mainCamera.orthographicSize; 
        cameraWidth = cameraHeight * mainCamera.aspect;
    }

    void LateUpdate()
    {
        if (player == null){
            return;
        }
        float clampedX = Mathf.Clamp(player.position.x + offset.x, minBoundary.x + cameraWidth/2 - offset.x, maxBoundary.x - cameraWidth/2 + offset.x);
        float clampedY = Mathf.Clamp(player.position.y + offset.y, minBoundary.y + cameraHeight/2 - offset.y, maxBoundary.y - cameraHeight/2 + offset.y);
        // Debug.Log(clampedX);
        // Debug.Log(clampedY);
        Vector3 clampedPosition = new Vector3(clampedX, clampedY, transform.position.z); // clamp de khong qua gioi han
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
