using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptycellScript : MonoBehaviour
{
    public int id;
    public GameObject CameraObj;
    private void OnMouseDown()
    {
        CameraObj.GetComponent<GameScript>().Spawn(this.gameObject, id);
    }
}
