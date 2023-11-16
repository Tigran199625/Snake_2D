using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PauseWindow : MonoBehaviour
{
    private static PauseWindow instance;

    private void Awake() {

        instance = this;

        transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        transform.GetComponent<RectTransform>().sizeDelta = Vector2.zero;


        transform.Find("resumeButton").GetComponent<Button_UI>().ClickFunc = () => GameHandler.ResumeGame();
        transform.Find("resumeButton").GetComponent<Button_UI>().AddButtonSound();

        transform.Find("mainMenuButton").GetComponent<Button_UI>().ClickFunc = () => Loader.Load(Loader.Scene.MainMenu);
        transform.Find("mainMenuButton").GetComponent<Button_UI>().AddButtonSound();

        Hide();

    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    public static void ShowStatic() {
        instance.Show();
    }

    public static void HideStatic() {
        instance.Hide();
    }

}
