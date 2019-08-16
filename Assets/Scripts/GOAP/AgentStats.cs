using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStats : MonoBehaviour
{
  public float health;
  public float maxForce;
  public float maxSpeed;
  public float meleeDamage;

  public bool steelDoor;

  // Use this for initialization
  void Awake()
  {
    //health = 100f;
    //maxForce = 10f;
    //maxSpeed = 15f;
    //meleeDamage = 0f;
    //For Door
    steelDoor = false;
  }

  // Update is called once per frame
  void Update()
  {

  }
}
