using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TilemapShader : MonoBehaviour
{
    protected Tilemap tilemap; 
    public Material customMaterial; 

    public void ApplyShaderToTile(PlantedPlant plant, string gObjectName, int layer)
    {
        Vector3Int tilePosition = plant.gridPosition;
        Sprite tileSprite = plant.tiles[plant.maxStage].sprite;
        // placeholder object with spriterenderer
        GameObject tileObject = new GameObject(gObjectName);
        SpriteRenderer spriteRenderer = tileObject.AddComponent<SpriteRenderer>();
        Renderer renderer = tileObject.GetComponent<Renderer>();

        PlantMaxShader plantMax = tileObject.AddComponent<PlantMaxShader>(); // detect player trigger to hide itself
        plantMax.plant = plant;

        renderer.sortingOrder = layer;

        // gameobject to match tile position
        Vector3 worldPosition = tilemap.CellToWorld(tilePosition);
        tileObject.transform.position = worldPosition + tilemap.tileAnchor;

        spriteRenderer.sprite = tileSprite;
        spriteRenderer.material = customMaterial;

        tileObject.SetActive(true);
        PolygonCollider2D collider = tileObject.AddComponent<PolygonCollider2D>();
        collider.isTrigger = true;
    }
}

