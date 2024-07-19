using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerUnit;

public class BuyLandButton : MonoBehaviour
{
    public PlayerUnit playerUnit;
    private Image image;
    public int amount = 50;
    private void Start() {
        image = GetComponent<Image>();
    }
    private void LateUpdate() {
        if (!playerUnit.moneyManager.SufficientMoney(amount)) {
            image.color = new Color(161f / 255f, 161f / 255f, 161f / 255f); 
        }
        else {
            image.color = new Color(176f / 255f, 166f / 255f, 97f / 255f); 
        }
    }
    public void BuyLandClick(){
        IEnumerator ShakeButton() {
            float duration = 0.1f;
            float magnitude = 5f;
            Vector3 originalPosition = transform.localPosition;

            float elapsed = 0f;
            while (elapsed < duration) {
                float x = originalPosition.x + UnityEngine.Random.Range(-1f, 1f) * magnitude;
                float y = originalPosition.y + UnityEngine.Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = new Vector3(x, y, originalPosition.z);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalPosition;
        }
        if (playerUnit.moneyManager.SufficientMoney(amount)&&BuyLand.Instance.currentCellPosition.HasValue){
            if (BuyLand.Instance.BuyLandHere()){
                playerUnit.moneyManager.UseMoney(amount);
            }
        }
        else{
            StartCoroutine(ShakeButton());
        }
    }
}
