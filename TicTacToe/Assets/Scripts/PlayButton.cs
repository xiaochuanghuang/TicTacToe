using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public GameObject PersistantObj;
    public GameObject DifficultyPanel;
    public string GameMode;

    public void OnClicked()
    {
        PersistantObj.GetComponent<PersistanceScript>().GameMode = GameMode;

        if (GameMode == "PVE")
        {
            // 显示难度选择面板
            DifficultyPanel.SetActive(true);
        }
        else
        {
            // 如果是 PVP，直接开始游戏
            SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
    }
    public void OnDifficultySelected(string difficulty)
    {
        PersistantObj.GetComponent<PersistanceScript>().AIDifficulty = difficulty;
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
    public void OnBackToMenu()
    {
        DifficultyPanel.SetActive(false);
    }

    }
