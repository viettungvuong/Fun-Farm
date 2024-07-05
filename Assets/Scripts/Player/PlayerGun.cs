using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Weapon
{
    GUN,
    SWORD
}

public class PlayerGun : MonoBehaviour
{
    public float range = 5f;
    public int damage = 20;

    [HideInInspector] public bool isShooting = false;
    [HideInInspector] public bool isReloading = false;

    private Animator animator;
    private PlayerMove playerMove;

    public int clipCapacity = 3;
    public int totalBullets=0;
    [SerializeField] private int bulletsInClip=0;

    public bool ownedGun = false; // own a gun or not

    public TextMeshProUGUI bulletCount;
    public GameObject bulletInfo;

    public Sprite swordSprite, gunSprite;
    public GameObject weaponShow;
    private TextMeshProUGUI weaponName;
    private Image weaponImage;
    public Image reloadingIndicator;
    public Weapon currentWeapon = Weapon.SWORD;

    // Bullet points
    public Transform gunUp;
    public Transform gunDown;
    public Transform gunHorizontal;

    public PlayerGunData Serialize()
    {
        PlayerGunData data = new PlayerGunData();
        data.totalBullets = totalBullets;
        data.bulletsInClip = bulletsInClip;
        data.ownedGun = ownedGun;
        data.currentWeapon = currentWeapon;
        return data;
    }

    public void Reload(PlayerGunData playerGunData){
        totalBullets = playerGunData.totalBullets;
        bulletsInClip = playerGunData.bulletsInClip;
        ownedGun = playerGunData.ownedGun;
        currentWeapon = playerGunData.currentWeapon;
    }


    private void Start()
    {
        if (PlayerUnit.playerMode==PlayerMode.CREATIVE){
            enabled = false;
            return;
        }
        animator = GetComponent<Animator>();
        playerMove = GetComponent<PlayerMove>();

        if (bulletCount != null) bulletCount.text = bulletsInClip + "/" + totalBullets;

        // Debug.Log(weaponShow);

        weaponImage = weaponShow.transform.GetChild(1).GetComponent<Image>();
        weaponName = weaponShow.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

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

        if (Input.GetKey(KeyCode.Space) && !isShooting && !isReloading)
        {
            Debug.Log("Press shoot");
            StopAllCoroutines();
            StartCoroutine(GunCoroutine());
        }
    }

    public void AddBullet()
    {
        totalBullets++;
        if (bulletsInClip < clipCapacity && totalBullets <= clipCapacity)
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
        bullet.damage = damage;
        bullet.Shoot(shootDirection);


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

        yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName));
        if (isShooting==false){
            Shoot();
        }

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

        StartCoroutine(ShowWeaponShow());
    }

    private IEnumerator ShowWeaponShow()
    {
        weaponShow.SetActive(true);
        yield return new WaitForSeconds(2); // show for 2 seconds
        weaponShow.SetActive(false);
    }
}