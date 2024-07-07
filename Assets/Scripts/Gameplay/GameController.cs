using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public static class GameController
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

    public static void Hide(Transform transform){
        transform.localScale = new Vector3(0, 0, 0);
    }

    public static void Show(Transform transform){
        transform.localScale = new Vector3(1, 1, 1);
    }

    public static void OpenMenu(){ // completely quit game
        // Destroy(canvasObject);
        // DontDestroyOnLoadManager.DestroyAll();
        // SceneManager.LoadScene("SceneWelcome");
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}
