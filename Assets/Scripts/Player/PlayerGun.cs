using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    public float range = 50f; 
    public float damage = 30f; 
    public ParticleSystem muzzleFlash;

    private Camera cam;

    Animator animator;
    PlayerMove playerMove;

    private void Start() {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        animator = GetComponent<Animator>();
        playerMove = GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.HomeScene()==false){
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("Shoot");
            Shoot();
        }
    }

    void Shoot()
    {
        PlayAnimationGun();
        muzzleFlash.Play();

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {

            // Apply damage if the object has a health component
            Unit unit = hit.transform.GetComponent<Unit>();
            if (unit != null)
            {
                unit.TakeDamage(damage);

                HitFlash hitFlash = hit.transform.GetComponent<HitFlash>();
                hitFlash.Flash();
            }
        }
    }

    private void PlayAnimationGun()
    {

        string animationName;

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

        animator.Play(animationName);

        Debug.Log(animationName);
    }
}