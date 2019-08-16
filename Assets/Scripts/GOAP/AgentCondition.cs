using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentCondition : MonoBehaviour
{
  public int  ammoCount;
  public bool hasBaton;
  public bool hasKey;
  public bool hasPotion;

  public GameObject capturer;

  // Use this for initialization
  void Awake()
  {
    hasPotion = false;

    // For gun and melee weapon
    ammoCount = 0;
    hasBaton = false;

    // Robber only
    hasKey = false;
    capturer = null;
  }

  // Update is called once per frame
  void Update()
  {

  }
}
