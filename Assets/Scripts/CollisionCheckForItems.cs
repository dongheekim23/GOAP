using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheckForItems : MonoBehaviour
{
  GameObject[] policeList;
  GameObject[] robberList;

  float checkDist;

  GameObject textObject;
  TextMesh textMesh;
  Vector3 relativeTextPos;

  // Use this for initialization
  void Start()
  {
    policeList = GameObject.FindGameObjectsWithTag("Police");
    robberList = GameObject.FindGameObjectsWithTag("Robber");

    checkDist = 8f;

    Vector3 objectSize = gameObject.GetComponent<Renderer>().bounds.size;

    textObject = new GameObject("FloatingText");
    relativeTextPos = new Vector3(-1 * objectSize.x, 1, -0.7f * objectSize.z);
    textObject.transform.position = gameObject.transform.position + relativeTextPos;
    textObject.transform.rotation = Quaternion.Euler(90, 0, 0);
    textMesh = textObject.AddComponent<TextMesh>();

    textMesh.color = Color.blue;//new Color(11, 114, 217, 255);
    textMesh.characterSize = 5;
    textMesh.text = gameObject.tag;
  }

  // Update is called once per frame
  void Update()
  {
    foreach(GameObject police in policeList)
    {
      if(police != null)
      {
        if(Vector3.Distance(transform.position, police.transform.position) <= checkDist)
        {
          if(gameObject.tag == "Baton")
            police.GetComponent<AgentCondition>().hasBaton = true;

          else if (gameObject.tag == "Ammo")
            police.GetComponent<AgentCondition>().ammoCount += 8;

          Destroy(gameObject);
          Destroy(textObject);
          return;
        }
      }
    }

    foreach (GameObject robber in robberList)
    {
      if (robber != null)
      {
        if (Vector3.Distance(transform.position, robber.transform.position) <= checkDist)
        {
          if (gameObject.tag == "Baton")
            robber.GetComponent<AgentCondition>().hasBaton = true;

          else if (gameObject.tag == "Ammo")
            robber.GetComponent<AgentCondition>().ammoCount += 8;

          else if (gameObject.tag == "Lockpick")
            robber.GetComponent<AgentCondition>().hasKey = true;

          Destroy(gameObject);
          Destroy(textObject);
          return;
        }
      }
    }
  }
}
