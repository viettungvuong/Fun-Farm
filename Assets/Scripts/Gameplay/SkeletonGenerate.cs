using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class SkeletonGenerate : Generate
{
    public static int skeletons;

    static bool hasSpawned, conditionHandled;
    public static SkeletonGenerate instance;
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
        nextMinuteRefill = 40;
        skeletons = 0;
        hasSpawned = false;
        conditionHandled = false; // bỏ vào start để reset lại giá trị static
        if (PlayerUnit.playerMode==PlayerMode.CREATIVE){
            enabled = false;
            return;
        }

        objectTag = "Skeleton";
        number = 2;
    }



    void FixedUpdate()
    {
        if (PlayerUnit.playerMode==PlayerMode.CREATIVE){
            enabled = false;
            return;
        }

        if (TimeManage.instance.IsDay()==true||skeletons>0){ // not spawn when skeletons still there
            return;
        }


        if (TimeManage.instance.currentMinute != nextMinuteRefill)
        {
            base.remainingTimeText.text = timeString(nextMinuteRefill);

            hasSpawned = false; // allow spawning once per min (this is resetting)
            conditionHandled = false;// allow check home scene and add 1 min only do once per min
        }

        if (TimeManage.instance.currentMinute==nextMinuteRefill&&!hasSpawned){
            if (GameController.HomeScene()==false){ 
                if (!conditionHandled){
                    nextMinuteRefill += 1;
                    if (nextMinuteRefill>=60){
                        nextMinuteRefill -= 60;
                    }
                    base.remainingTimeText.text = timeString(nextMinuteRefill);
                    conditionHandled = true;
                    return;
                } 
            }
            hasSpawned = true;
            nextMinuteRefill+= intervalBetweenSpawns;
            if (nextMinuteRefill>=60){
                nextMinuteRefill -= 60;
            }
            Debug.Log(nextMinuteRefill);
            base.remainingTimeText.text = timeString(nextMinuteRefill);
            skeletons += number;
            Spawn(number);

        }
    }
}
