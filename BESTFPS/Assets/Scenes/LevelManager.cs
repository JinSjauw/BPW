using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private GameObject loaderObject;
    [SerializeField] private Image progressBarObject;


    private void Awake()
    {
        if(Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }
    }
    public async void LoadScene(string sceneName) 
    {
        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

      /*  loaderObject.SetActive(true);

        do
        {
            progressBarObject.fillAmount = scene.progress;
        } while (scene.progress < 0.9f)*/;

        scene.allowSceneActivation = true;
        //loaderObject.SetActive(false);
    }
}
