using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistanceScript : MonoBehaviour
{
    public string GameMode;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

}
