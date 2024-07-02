using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{


    public static float GetAnimationLength(Animator animator, string animationName)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (var clip in clips)
        {
            if (clip.name == animationName)
            {
                return clip.length;
            }
        }
        return 0f;
    }

    public static bool HomeScene(){
        return SceneManager.GetActiveScene().name == "SceneHome";
    }

    public static double SerializeDateTime(DateTime dateTime){
        return (dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds; 
    }

    public static DateTime DeserializeDateTime(double timeStamp){
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timeStamp); 
    }

    public static void Hide(Transform transform){
        transform.localScale = new Vector3(0, 0, 0);
    }

    public static void Show(Transform transform){
        transform.localScale = new Vector3(1, 1, 1);
    }

}
