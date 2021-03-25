//Used usually when you have a login page and you always have to play from it
//It changes scene, plays, and on play finished, it returns to the last scene you were into

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class PlayFromScene : MonoBehaviour
{
    private static bool shouldPlay = false;

    static EditorScripts()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    [MenuItem("casigame/Play Game")]
    static void DoSomething()
    {
        string lastScene = EditorSceneManager.GetActiveScene().name;
        SessionState.SetString("lastScene", lastScene);
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        shouldPlay = true;
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
    }

    private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        if (!shouldPlay) return;
        shouldPlay = false;

        EditorApplication.EnterPlaymode();
    }
    private static void OnPlayModeChanged(PlayModeStateChange obj)
    {
        if(obj == PlayModeStateChange.EnteredEditMode)
        {
            string lastScene = SessionState.GetString("lastScene", "");
            if (lastScene != "")
            {
                SessionState.EraseString("lastScene");
                EditorSceneManager.OpenScene($"Assets/Scenes/{lastScene}.unity");
            }
        }
    }
}


