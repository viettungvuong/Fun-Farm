using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class SlimeGenerate : Generate
{

    public static int slimes;

    static bool hasSpawned, conditionHandled;

    public static SlimeGenerate instance;
    static int intervalBetweenSpawns = 60; 
    public static int nextMinuteRefill;

    protected override void Awake() {
        base.Awake();

        if (instance==null){
            instance = this;
        }
        else{
            Destroy(this);
        }
    }

    protected override void Start()
    {
        base.Start();
        Debug.Log("Start");
        nextMinuteRefill = 30;
        slimes = 0;
        hasSpawned = false;
        conditionHandled = false; // bỏ vào start để reset lại giá trị

        if (PlayerUnit.playerMode==PlayerMode.CREATIVE){
            enabled = false;
            return;
        }

        objectTag = "Slime";
        number = 3;
    }



    void FixedUpdate()
    {
        if (PlayerUnit.playerMode==PlayerMode.CREATIVE){
            enabled = false;
            return;
        }
        if (TimeManage.instance.IsDay()){
            enemyIndicatorIcon.sprite = slimeSprite;
        }
        else{
            enemyIndicatorIcon.sprite = skeletonSprite;
            return;
        }

        if (TimeManage.instance.currentMinute != nextMinuteRefill)
        {
            base.remainingTimeText.text = timeString(nextMinuteRefill);

            hasSpawned = false; // allow spawning once per min
            conditionHandled = false;// allow check home scene and add 1 min only do once per min
        }


        if (TimeManage.instance.currentMinute==nextMinuteRefill&&!hasSpawned){
            if (GameController.HomeScene()==false||PlantManager.instance.GetNumberOfPlants()==0){ // no plants then not spawn
                if (!conditionHandled)
                {
                    nextMinuteRefill += 1;
                    if (nextMinuteRefill >= 60)
                    {
                        nextMinuteRefill -= 60;
                    }
                    base.remainingTimeText.text = timeString(nextMinuteRefill);
                    conditionHandled = true;
                }
                return;
            }

            hasSpawned = true;
            nextMinuteRefill+= intervalBetweenSpawns;
            if (nextMinuteRefill>=60){
                nextMinuteRefill -= 60;
            }
            base.remainingTimeText.text = timeString(nextMinuteRefill);
            slimes += number;
            Spawn(number);

        }
    }

    

}
