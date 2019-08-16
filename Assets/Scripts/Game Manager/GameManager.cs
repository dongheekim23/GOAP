using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  int policeCount;
  public int robberCount;
  int prisonerCount;

  string currSceneName;

  float elapsedTime;
  float goalTime;

  public bool escapeStarted;
  public bool convictEscaped;

  float screenX;
  float screenY;

  public int patrolIt;

  // Use this for initialization
  void Start()
  {
    currSceneName = SceneManager.GetActiveScene().name;

    policeCount = 0;
    robberCount = 0;
    prisonerCount = 0;

    elapsedTime = 0f;
    goalTime = 20f;

    escapeStarted = false;
    convictEscaped = false;

    screenX = Screen.width;
    screenY = Screen.height;

    patrolIt = 1;
  }

  // Update is called once per frame
  void Update()
  {
    if (escapeStarted == false && currSceneName == "SecondScene")
    {
      if (elapsedTime >= goalTime)
      {
        elapsedTime = 0f;
        escapeStarted = true;
      }

      elapsedTime += Time.deltaTime;
    }

    policeCount = GameObject.FindGameObjectsWithTag("Police").Length;
    robberCount = GameObject.FindGameObjectsWithTag("Robber").Length;
    if (currSceneName == "FirstScene")
      prisonerCount = GameObject.FindGameObjectsWithTag("Prisoner").Length;

    if (Input.GetKeyDown(KeyCode.P))
    {
      if (Time.timeScale == 1)
        Time.timeScale = 0;
      else if (Time.timeScale == 0)
        Time.timeScale = 1;
    }
    //else if (Input.GetKeyDown(KeyCode.C))
    //{
    //  Time.timeScale = 1;
    //}
    else if (Input.GetKeyDown(KeyCode.Escape))
    {
      Application.Quit();
    }
    else if (Input.GetKeyDown(KeyCode.R))
    {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    else if (Input.GetKeyDown(KeyCode.LeftArrow)
      || Input.GetKeyDown(KeyCode.RightArrow))
    {
      if (currSceneName == "FirstScene")
        SceneManager.LoadScene("SecondScene");
      else if (currSceneName == "SecondScene")
        SceneManager.LoadScene("FirstScene");
    }
  }

  void OnGUI()
  {
    GUIStyle guiStyle = new GUIStyle();
    guiStyle.fontSize = 20;

    if (currSceneName == "FirstScene")
    {
      GUI.Label(new Rect(10, 10, 100, 20), "<Cops vs. Criminals>", guiStyle);
      GUI.Label(new Rect(10, 35, 100, 20), "Police Count : " + policeCount.ToString(), guiStyle);
      GUI.Label(new Rect(10, 60, 100, 20), "Criminal Count : " + robberCount.ToString(), guiStyle);
      GUI.Label(new Rect(10, 85, 100, 20), "Prisoner Count : " + prisonerCount.ToString(), guiStyle);
    }
    else
    {
      GUI.Label(new Rect(10, 10, 100, 20), "<Prison Break>", guiStyle);
      GUI.Label(new Rect(10, 35, 100, 20), "Guard Count : " + policeCount.ToString(), guiStyle);
      GUI.Label(new Rect(10, 60, 100, 20), "Prionser Count : " + robberCount.ToString(), guiStyle);
    }

    GUI.Label(new Rect(screenX - 300, 10, 100, 20), "ESC : Quit program", guiStyle);
    GUI.Label(new Rect(screenX - 300, 35, 100, 20), "<-,-> : Switch between scenes", guiStyle);
    GUI.Label(new Rect(screenX - 300, 60, 100, 20), "P : Pause/Play current scene", guiStyle);
    GUI.Label(new Rect(screenX - 300, 85, 100, 20), "R : Restart current scene", guiStyle);
  }
}
