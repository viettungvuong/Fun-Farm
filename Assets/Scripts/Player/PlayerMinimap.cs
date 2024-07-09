using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMinimap : MonoBehaviour
{
    public GameObject minimap;
    private Camera mainCamera;
    private Vector3 left, right;
    private bool isMoved = false; // To track if the minimap has been moved
    private void Start() {
 
        SceneManager.sceneLoaded += OnSceneLoaded;

        left = new Vector3(79.4f, 29.7f);
        right = new Vector3(513f, 29.7f);

        InitializeCamera();
    }


    private void OnDestroy()
    {
 
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
 
        InitializeCamera();
    }

    private void InitializeCamera(){
        if (GameController.HomeScene()){
            mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        }
    }

    private void Update()
    {
        if (GameController.HomeScene())
        {
            minimap.transform.localScale = new Vector3(1, 1, 1);
            CheckPlayerMinimapOverlap();
        }
        else
        {
            minimap.transform.localScale = new Vector3(0, 0, 0);
        }


    }

    private void CheckPlayerMinimapOverlap()
    {
        // convert player position to screen point
        Vector3 playerScreenPoint = mainCamera.WorldToScreenPoint(transform.position);
        Ray ray = mainCamera.ScreenPointToRay(playerScreenPoint);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // raycast hit from player to mnimap
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider != null && hit.collider.gameObject == minimap)
            {
                if (!isMoved)
                {
                    MoveMinimap();
                    isMoved = true;
                }
            }
            else
            {
                if (isMoved)
                {
                    ResetMinimap();
                    isMoved = false;
                }
            }
        }
    }

    private void MoveMinimap()
    {
        minimap.transform.localPosition = right;
    }

    private void ResetMinimap()
    {
        minimap.transform.localPosition = left;
    }
}