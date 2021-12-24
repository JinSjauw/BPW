using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    public void ChangeScene(string name) 
    {
        Debug.Log("Log button pressed");
        LevelManager.Instance.LoadScene(name);
    }
}
