using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GunButton : MonoBehaviour
{
    private GameObject player;
    public int price;
    public bool gun;
    private PlayerUnit playerUnit;
    private PlayerGun playerGun;
    // public int price;
    private Image background, priceBg;
    private TextMeshProUGUI buyText;

    public GameObject bulletPanel;

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player");

        playerUnit = player.GetComponent<PlayerUnit>();
        playerGun = player.GetComponent<PlayerGun>();

        background = GetComponent<Image>();
        priceBg = transform.GetChild(0).GetComponent<Image>();

        buyText = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        buyText.text = price.ToString();
    }

    private void LateUpdate() {
        if (!playerUnit.SufficientMoney(price)||(gun&&PlayerGun.ownedGun)||(!gun&&!PlayerGun.ownedGun)) {
            background.color = new Color(161f / 255f, 161f / 255f, 161f / 255f); 
            priceBg.color = new Color(144f / 255f, 144f / 255f, 144f / 255f); 
        }
        else {
            background.color = new Color(60f / 255f, 109f / 255f, 93f / 255f); 
            priceBg.color = new Color(144f / 255f, 169f / 255f, 212f / 255f); 
        }
    }


    public void ChooseGun(){
        IEnumerator ShakeButton() {
            float duration = 0.1f;
            float magnitude = 5f;
            Vector3 originalPosition = transform.localPosition;

            float elapsed = 0f;
            while (elapsed < duration) {
                float x = originalPosition.x + Random.Range(-1f, 1f) * magnitude;
                float y = originalPosition.y + Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = new Vector3(x, y, originalPosition.z);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalPosition;
        }

        IEnumerator ShowBulletPanelCoroutine(){
            bulletPanel.SetActive(true);
            playerGun.AddBullet();
            yield return new WaitForSeconds(5);
            bulletPanel.SetActive(false);
            
        }
        
        if (!playerUnit.SufficientMoney(price)||(gun&&PlayerGun.ownedGun)||(!gun&&!PlayerGun.ownedGun)) {
            // shake button
            StartCoroutine(ShakeButton());
            return;
        }
        playerUnit.UseMoney(price);
        if (gun){
            PlayerGun.ownedGun = true;
        }
        else{ // for bullets
            StartCoroutine(ShowBulletPanelCoroutine());
        }
    }
}
