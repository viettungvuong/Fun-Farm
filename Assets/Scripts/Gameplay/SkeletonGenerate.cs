using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class SkeletonGenerate : Generate
{
    public static int skeletons = 0;

    static bool hasSpawned = false, conditionHandled = false;
    public static SkeletonGenerate instance;
    static int intervalBetweenSpawns = 60, nextMinuteRefill = 40;

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

        objectTag = "Skeleton";
        number = 2;
    }



    void FixedUpdate()
    {
        if (TimeManage.instance.IsDay()==true){
            return;
        }


        if (TimeManage.instance.currentMinute != nextMinuteRefill)
        {
            int minsDiff;
            if (nextMinuteRefill>TimeManage.instance.currentMinute){
                minsDiff = nextMinuteRefill - TimeManage.instance.currentMinute;
            }
            else{
                minsDiff = nextMinuteRefill+60 - TimeManage.instance.currentMinute;
            }
            int hours = minsDiff / 60;
            int mins = minsDiff - hours * 60;
            string minsString = (mins < 10) ? "0" + mins : mins.ToString();
            base.remainingTimeText.text = hours + ":" + minsString;

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
                    conditionHandled = true;
                    return;
                } 
            }
            hasSpawned = true;
            nextMinuteRefill+= intervalBetweenSpawns;
            if (nextMinuteRefill>=60){
                nextMinuteRefill -= 60;
            }
            skeletons += number;
            Spawn(number);

        }
    }
}
