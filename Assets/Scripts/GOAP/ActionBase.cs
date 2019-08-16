using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionBase// : MonoBehaviour
{
  // To check action status
  public enum ActionStatus
  {
    ON_START,
    ON_UPDATE,
    ON_EXIT
  }

  protected ActionStatus currStatus;
  protected float cost;
  protected GameObject agent;
  protected TextMesh textMesh;

  protected void BaseStart()
  {
    currStatus = ActionStatus.ON_START;
    cost = 0f;
    agent = null;
  }

  // Use this for initialization
  void Start()
  {
    
  }
  
  public void Run(ref Vector3 steeringVec)
  {
    if (currStatus == ActionStatus.ON_START)
      OnStart(ref steeringVec);
    else if(currStatus == ActionStatus.ON_UPDATE)
      OnUpdate(ref steeringVec);
    //else if(currStatus == ActionStatus.ON_EXIT)
    //  OnExit(ref steeringVec);
  }

  // Return -1 if the action is not achievable
  // with the current condition of the agent
  public abstract float ComputeCost();
  // Initialize values (called only once)
  public abstract void OnStart(ref Vector3 steeringVec);
  // Actual action
  public abstract void OnUpdate(ref Vector3 steeringVec);
  // Reset values (called only once)
  //public abstract void OnExit(ref Vector3 steeringVec);
  public abstract string GetName();
  public ActionStatus GetStatus()
  {
    return currStatus;
  }
}
