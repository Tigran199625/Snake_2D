using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public static class Score {

    public static event EventHandler OnHighscoreChanged;

    private static int score;
    public static void InitializeStatic() {
        OnHighscoreChanged = null;
        score = 0;
    }

    public static int GetScore() {
        return score;
    }

    public static void AddScore() {
        score += 100;
    }

    public static int GetHighScore() {
        return PlayerPrefs.GetInt("highscore", 0);
    }

    public static bool TrySetNewHighScore() {
        return TrySetNewHighScore(score);
    }

    public static bool TrySetNewHighScore(int score) {
        int highscore = GetHighScore();
        if(score > highscore) {
            PlayerPrefs.SetInt("highscore", score);
            PlayerPrefs.Save();
            OnHighscoreChanged?.Invoke(null, EventArgs.Empty);
            return true;
        } else {
            return false;
        }

    }

}
