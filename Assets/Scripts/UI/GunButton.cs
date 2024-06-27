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
    private Image background;
    private TextMeshProUGUI buyText;


    void Start(){
        player = GameObject.FindGameObjectWithTag("Player");

        playerUnit = player.GetComponent<PlayerUnit>();
        playerGun = player.GetComponent<PlayerGun>();

        background = GetComponent<Image>();

        buyText = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        buyText.text = price.ToString();
    }

    private void LateUpdate() {
        if (!playerUnit.SufficientMoney(price)||(gun&&PlayerGun.ownedGun)||(!gun&&!PlayerGun.ownedGun)) {
            background.color = new Color(161f / 255f, 161f / 255f, 161f / 255f); 
        }
        else {
            background.color = new Color(60f / 255f, 109f / 255f, 93f / 255f); 
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


        
        if (!playerUnit.SufficientMoney(price)||(gun&&PlayerGun.ownedGun)||(!gun&&!PlayerGun.ownedGun)) {
            // shake button
            StartCoroutine(ShakeButton());
            return;
        }
        playerUnit.UseMoney(price);

        if (gun){
            PlayerGun.ownedGun = true;
            for (int i=0; i<12; i++){
                playerGun.AddBullet();
            }
        }
        else{ // for bullets
            playerGun.AddBullet();
        }

    }
}
