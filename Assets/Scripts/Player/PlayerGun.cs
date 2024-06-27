using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Weapon
{
    GUN,
    SWORD
}

public class PlayerGun : MonoBehaviour
{
    public float range = 5f;
    public static float damage = 30f;

    private Camera cam;

    [HideInInspector] public bool isShooting = false;
    [HideInInspector] public bool isReloading = false;

    Animator animator;
    PlayerMove playerMove;

    public static int clipCapacity = 3;
    public static int totalBullets = 0;
    private static int bulletsInClip;

    public static bool ownedGun = false; // own a gun or not

    public TextMeshProUGUI bulletCount;
    public GameObject bulletInfo;

    public Sprite swordSprite, gunSprite;
    public GameObject weaponHandle;
    private TextMeshProUGUI weaponName;
    private Image weaponImage;
    public Image reloadingIndicator;
    public static Weapon currentWeapon = Weapon.SWORD;

    // Bullet points
    public Transform gunUp;
    public Transform gunDown;
    public Transform gunHorizontal;

    private void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        animator = GetComponent<Animator>();
        playerMove = GetComponent<PlayerMove>();

        bulletsInClip = 0;
        bulletCount.text = bulletsInClip + "/" + totalBullets;

        weaponImage = weaponHandle.transform.GetChild(1).GetComponent<Image>();
        weaponName = weaponHandle.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        if (reloadingIndicator != null)
        {
            reloadingIndicator.fillAmount = 0;
            reloadingIndicator.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchWeapon();
        }

        if (GameController.HomeScene() == false || currentWeapon != Weapon.GUN)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isShooting && !isReloading)
        {
            StopAllCoroutines();
            StartCoroutine(GunCoroutine());
        }
    }

    public void AddBullet()
    {
        totalBullets++;
        if (bulletsInClip < clipCapacity && totalBullets < clipCapacity)
        {
            bulletsInClip = totalBullets;
        }
        bulletCount.text = bulletsInClip + "/" + totalBullets;
    }

    public void Shoot()
    {
        bulletsInClip--;
        bulletCount.text = bulletsInClip + "/" + totalBullets;

        Vector3 spawnPosition;
        Vector2 shootDirection;
        Quaternion rotation;

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

        // Raycast check hit
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Unit unit = hit.transform.GetComponent<Unit>();
            if (unit != null)
            {
                unit.TakeDamage(damage);

                HitFlash hitFlash = hit.transform.GetComponent<HitFlash>();
                if (hitFlash != null)
                {
                    hitFlash.Flash();
                }
            }
        }
    }

    private IEnumerator GunCoroutine()
    {
        if (bulletsInClip <= 0)
        {
            if (totalBullets > 0)
            {
                yield return StartCoroutine(ReloadGunCoroutine());
            }
            else
            {
                Debug.Log("No bullets available!");
                yield break;
            }
        }

        isShooting = true;
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

        animator.SetBool("idle", false);
        animator.Play(animationName);

        Shoot();

        yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName));
        animator.SetBool("idle", true);
        isShooting = false;
    }

    private IEnumerator ReloadGunCoroutine()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        if (reloadingIndicator != null)
        {
            reloadingIndicator.gameObject.SetActive(true);
        }

        float reloadTime = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < reloadTime)
        {
            elapsedTime += Time.deltaTime;
            if (reloadingIndicator != null)
            {
                reloadingIndicator.fillAmount = elapsedTime / reloadTime;
            }
            yield return null;
        }

        if (reloadingIndicator != null)
        {
            reloadingIndicator.fillAmount = 0;
            reloadingIndicator.gameObject.SetActive(false);
        }

        int bulletsToReload = Math.Min(clipCapacity, totalBullets);
        bulletsInClip = bulletsToReload;
        totalBullets -= bulletsToReload;

        bulletCount.text = bulletsInClip + "/" + totalBullets;

        Debug.Log("Reloaded!");

        isReloading = false;
    }

    private void SwitchWeapon()
    {
        if (ownedGun==false){
            return;
        }
        if (currentWeapon == Weapon.GUN)
        {
            currentWeapon = Weapon.SWORD;
            weaponImage.sprite = swordSprite;
            weaponName.text = "Sword";
            bulletInfo.SetActive(false);
        }
        else
        {
            currentWeapon = Weapon.GUN;
            weaponImage.sprite = gunSprite;
            weaponName.text = "Gun";
            bulletInfo.SetActive(true);
        }

        StartCoroutine(ShowWeaponHandle());
    }

    private IEnumerator ShowWeaponHandle()
    {
        weaponHandle.SetActive(true);
        yield return new WaitForSeconds(5); // show for 2 seconds
        weaponHandle.SetActive(false);
    }
}