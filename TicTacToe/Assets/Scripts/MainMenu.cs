using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject difficultyPanel; // 引用 DifficultyPanel

    private void Start()
    {
        // 确保难度选择面板初始为隐藏状态
        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(false);
        }
    }

    public void OnVSPlayerClicked()
    {
        StartGame("PVP");
    }

    public void OnVSAIClicked()
    {
        // 显示难度选择面板
        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(true);
        }
    }

    public void OnDifficultySelected(string difficulty)
    {
        GameObject persistentObj = GameObject.FindGameObjectWithTag("PersistantObj");
        if (persistentObj == null)
        {
            persistentObj = new GameObject("PersistentObj");
            persistentObj.AddComponent<PersistanceScript>();
            persistentObj.tag = "PersistantObj";
            DontDestroyOnLoad(persistentObj);
        }

        persistentObj.GetComponent<PersistanceScript>().GameMode = "PVE";
        //persistentObj.GetComponent<PersistanceScript>().AIDifficulty = difficulty;

        SceneManager.LoadScene("GameScene");
    }

    private void StartGame(string gameMode)
    {
        GameObject persistentObj = GameObject.FindGameObjectWithTag("PersistantObj");

        if (persistentObj == null)
        {
            persistentObj = new GameObject("PersistentObj");
            persistentObj.AddComponent<PersistanceScript>();
            persistentObj.tag = "PersistantObj";
            DontDestroyOnLoad(persistentObj);
        }

        persistentObj.GetComponent<PersistanceScript>().GameMode = gameMode;
        SceneManager.LoadScene("GameScene");
    }

}
