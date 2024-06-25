using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TilemapShader : MonoBehaviour
{
    protected Tilemap tilemap; 
    public Material customMaterial; 

    public void ApplyShaderToTile(Vector3Int tilePosition, Sprite tileSprite, string gObjectName)
    {

        // placeholder object with spriterenderer
        GameObject tileObject = new GameObject(gObjectName);
        SpriteRenderer spriteRenderer = tileObject.AddComponent<SpriteRenderer>();

        tileObject.layer = tilemap.gameObject.layer;

        // gameobject to match tile position
        Vector3 worldPosition = tilemap.CellToWorld(tilePosition);
        tileObject.transform.position = worldPosition + tilemap.tileAnchor;

        spriteRenderer.sprite = tileSprite;
        spriteRenderer.material = customMaterial;

    }
}

