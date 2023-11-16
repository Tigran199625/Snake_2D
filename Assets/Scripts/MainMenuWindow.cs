using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuWindow : MonoBehaviour
{

    private enum Sub {
        Main,
        HowToPlay,
    }

    private void Awake() {

        // Set positions to default ` zero, in case if we move them.
        transform.Find("mainSub").GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        transform.Find("howToPlaySub").GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        // Get buttons and config them.
        transform.Find("mainSub").Find("playButton").GetComponent<Button_UI>().ClickFunc = () => Loader.Load(Loader.Scene.GameScene);
        transform.Find("mainSub").Find("playButton").GetComponent<Button_UI>().AddButtonSound();

        transform.Find("mainSub").Find("quitButton").GetComponent<Button_UI>().ClickFunc = () => Application.Quit();
        transform.Find("mainSub").Find("quitButton").GetComponent<Button_UI>().AddButtonSound();

        transform.Find("mainSub").Find("howToPlayButton").GetComponent<Button_UI>().ClickFunc = () => ShowSub(Sub.HowToPlay);
        transform.Find("mainSub").Find("howToPlayButton").GetComponent<Button_UI>().AddButtonSound();

        transform.Find("howToPlaySub").Find("backButton").GetComponent<Button_UI>().ClickFunc = () => ShowSub(Sub.Main);
        transform.Find("howToPlaySub").Find("backButton").GetComponent<Button_UI>().AddButtonSound();

        // Show main menu window
        ShowSub(Sub.Main);
    }

    private void ShowSub(Sub sub) {
        transform.Find("mainSub").gameObject.SetActive(false);
        transform.Find("howToPlaySub").gameObject.SetActive(false);

        switch(sub) {
            case Sub.Main:
                transform.Find("mainSub").gameObject.SetActive(true);
                break;
            case Sub.HowToPlay:
                transform.Find("howToPlaySub").gameObject.SetActive(true);
                break;
        }
    }

}
