using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public GameObject PersistantObj;

    public string GameMode;

    public void OnClicked()
    {
        PersistantObj.GetComponent<PersistanceScript>().GameMode = GameMode;
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        //Debug.Log("PlayButton clicked");
    }
}
