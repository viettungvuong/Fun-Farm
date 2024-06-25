using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TilemapShader : MonoBehaviour
{
    protected Tilemap tilemap; 
    public Material customMaterial; 

    public void ApplyShaderToTile(Vector3Int tilePosition, Sprite tileSprite, string gObjectName, int layer)
    {

        // placeholder object with spriterenderer
        GameObject tileObject = new GameObject(gObjectName);
        SpriteRenderer spriteRenderer = tileObject.AddComponent<SpriteRenderer>();
        Renderer renderer = tileObject.GetComponent<Renderer>();

        PolygonCollider2D collider = tileObject.AddComponent<PolygonCollider2D>();
        collider.isTrigger = true;

        tileObject.AddComponent<PlantMaxShader>(); // detect player to hide itself

        renderer.sortingOrder = layer;

        // gameobject to match tile position
        Vector3 worldPosition = tilemap.CellToWorld(tilePosition);
        tileObject.transform.position = worldPosition + tilemap.tileAnchor;

        spriteRenderer.sprite = tileSprite;
        spriteRenderer.material = customMaterial;

        GlowPulse glowPulse = tileObject.AddComponent<GlowPulse>();
        glowPulse.glowMaterial = customMaterial;
        glowPulse.pulseSpeed = 0.5f;
        glowPulse.minGlowIntensity = 0.1f;
        glowPulse.maxGlowIntensity = 0.5f;
        glowPulse.moveAmount = 0;

    }
}

