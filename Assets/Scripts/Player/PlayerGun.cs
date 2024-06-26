using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    public float range = 5f; 
    public static float damage = 30f; 

    private Camera cam;

    [HideInInspector] public bool isShooting = false;

    Animator animator;
    PlayerMove playerMove;

    // References to muzzle flash points
    public Transform gunUp;
    public Transform gunDown;
    public Transform gunHorizontal;

    private void Start() {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        animator = GetComponent<Animator>();
        playerMove = GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.HomeScene() == false){
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StopAllCoroutines();
            StartCoroutine(GunCoroutine());
        }
    }

    public void Shoot()
    {

        Vector3 spawnPosition;
        Vector2 shootDirection;
        Quaternion rotation;


        // spawn bullet
        switch (playerMove.orientation)
        {
            case Orientation.UP:
                spawnPosition = gunUp.position;
                shootDirection = Vector2.up;
                rotation = Quaternion.Euler(0f, 0f, 90f); 
                break;
            case Orientation.DOWN:
                spawnPosition = gunDown.position;
                shootDirection = Vector2.down;
                rotation = Quaternion.Euler(0f, 0f, -90f);
                break;
            case Orientation.LEFT:
                spawnPosition = gunHorizontal.position;
                shootDirection = Vector2.left;
                rotation = Quaternion.Euler(0f, 0f, 180f); 
                break;
            default:
                spawnPosition = gunHorizontal.position;
                shootDirection = Vector2.right;
                rotation = Quaternion.identity;
                break;
        }

        GameObject bulletInstance = ObjectPooling.SpawnFromPool("Bullet", spawnPosition);
        bulletInstance.transform.rotation = rotation;
        bulletInstance.SetActive(true);

        Bullet bullet = bulletInstance.GetComponent<Bullet>();
        bullet.maxRange = range;

        bullet.Shoot(shootDirection);

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            // damage
            Unit unit = hit.transform.GetComponent<Unit>();
            if (unit != null)
            {
                unit.TakeDamage(damage);

                HitFlash hitFlash = hit.transform.GetComponent<HitFlash>();
                hitFlash.Flash();
            }
        }
    }

    private IEnumerator GunCoroutine()
    {
        isShooting = true;
        string animationName;

        // Set the muzzle flash position based on orientation
        switch (playerMove.orientation)
        {
            case Orientation.UP:
                animationName = "PlayerGunUp";
                break;
            case Orientation.DOWN:
                animationName = "PlayerGunDown";
                break;
            default:
                animationName = "PlayerGunHorizontal";
                break;
        }

        animator.SetBool("idle", false);
        animator.Play(animationName);


        yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName));
        animator.SetBool("idle", true);
        isShooting = false;
    }
}