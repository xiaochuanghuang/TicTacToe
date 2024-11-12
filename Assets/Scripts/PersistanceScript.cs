using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistanceScript : MonoBehaviour
{
    public string GameMode;
    public string AIDifficulty;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

}
