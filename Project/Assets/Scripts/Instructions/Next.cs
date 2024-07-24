using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Next : MonoBehaviour
{
    private TextMeshProUGUI text;
    public PlayerMove playerMove;
    public GameObject[] instructions;
    int iteration = 0;
    private int numInstructions;

    private void Start() {
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        numInstructions = instructions.Count();
    }
    public void NextInstruction() {
        if (iteration>0){
            instructions[iteration-1].SetActive(false);
        }
        if (iteration >= numInstructions-1){ // final instruction
            StartCoroutine(MovePlayerVillage());
        }
        else{
            text.text = "Next";
        }
        instructions[iteration].SetActive(true);
        iteration++;
    }

    private IEnumerator MovePlayerVillage()
    {
        this.transform.localScale = new Vector3(0, 0, 0); // hide button
        Vector3 pointA = new Vector3(-7.29f, -3.92f);
        yield return StartCoroutine(playerMove.AutomaticMove(pointA));
        yield return new WaitForSeconds(0.5f); 
        Vector3 pointB = new Vector3(-17.12f, -3.92f);
        yield return StartCoroutine(playerMove.AutomaticMove(pointB));
        instructions[iteration - 1].transform.GetChild(0).gameObject.SetActive(true); // show Village entry instructions
        yield return new WaitForSeconds(2f);
        PlayerPrefs.SetInt("GameOpened", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("SceneWelcome"); // open welcome
    }
}
