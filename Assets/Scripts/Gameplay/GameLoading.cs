using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoading : MonoBehaviour
{
    PlayerUnit FetchPlayer(string json){
        return JsonUtility.FromJson<PlayerUnit>(json);
    }

    TimeManage FetchTime(string json){
        return JsonUtility.FromJson<TimeManage>(json);
    }

    PlantManager FetchPlants(string json){
        return JsonUtility.FromJson<PlantManager>(json);
    }
}
