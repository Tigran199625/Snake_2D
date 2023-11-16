using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{

    public enum Scene {
        GameScene,
        Loading,
        MainMenu
    }

    private static Action loaderCallbackAction;
    public static void Load(Scene scene) {

        // Set the loader callback action to load the target scene
        loaderCallbackAction = () => {
            SceneManager.LoadScene(scene.ToString());
        };

        // Load the loading scene
        SceneManager.LoadScene(Scene.Loading.ToString());
    } 

    public static void LoaderCallback() {
        // Triggered after the first Update which lets the screen refresh
        // Execute the loader callback action which will load the target scene
        if(loaderCallbackAction != null) {
            loaderCallbackAction();
            loaderCallbackAction = null;
        }
    }
}
