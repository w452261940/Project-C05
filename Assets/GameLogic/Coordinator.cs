using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using SKCell;

public class Coordinator : MonoBehaviour
{

    public Vector3[] startPoints;
    public static bool GameIsPaused;
    public GameObject pauseMenuUI;

    void Start()
    {
        InitializeMap();
    }

    void InitializeMap()
    {
        startPoints = new Vector3[10];
        Transform startPointContainer = GameObject.Find("StartPoints").transform;
        for (int i = 0; i < startPointContainer.childCount; i++)
        {
            startPoints[i] = startPointContainer.GetChild(i).position;
        }
    }

    public void UpdateMenuEvent()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadLevel();
            Debug.Log("1");
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void Update()
    {
        UpdateMenuEvent();
    }

    

}
