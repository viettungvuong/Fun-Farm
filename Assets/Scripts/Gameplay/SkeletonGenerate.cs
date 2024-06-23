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
    static int intervalBetweenSpawns = 60, nextMinuteRefill = 15;
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

        intervalBetweenSpawns = 60;
        nextMinuteRefill = 15;
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
            hasSpawned = false; // allow spawning once per min
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

            Spawn(number);
            skeletons += number;
        }
    }
}
