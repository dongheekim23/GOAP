using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Patrol : GoalBase
{
  public G_Patrol(GameObject agent)
  {
    base.BaseStart(agent);
    
    // TODO : Implement and add "Move to random position" action
    waysToAchieveGoal.Add(new WayToGoal(
      new List<ActionBase>() { new A_Patrol(agent) },
      new List<Conditions>()));
  }

  // Update is called once per frame
  void Update()
  {

  }

  public override string GetName()
  {
    return "G_Patrol";
  }
}
