using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class SlimeGenerate : Generate
{

    public static int slimes = 0;

    static bool hasSpawned = false, conditionHandled = false;

    public static SlimeGenerate instance;
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

        objectTag = "Slime";
        number = 4;
    }



    void FixedUpdate()
    {
        if (TimeManage.instance.IsDay()==false){
            return;
        }
        if (TimeManage.instance.currentMinute != nextMinuteRefill)
        {
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
                    conditionHandled = true;
                }
                return;
            }

            hasSpawned = true;
            nextMinuteRefill+= intervalBetweenSpawns;
            if (nextMinuteRefill>=60){
                nextMinuteRefill -= 60;
            }
            slimes += number;
            Spawn(number);

        }
    }

    

}
