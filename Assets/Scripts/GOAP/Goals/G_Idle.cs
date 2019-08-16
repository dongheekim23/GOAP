using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Idle : GoalBase
{
  public G_Idle(GameObject agent, float timer = 5f)
  {
    base.BaseStart(agent);

    waysToAchieveGoal.Add(new WayToGoal(
      new List<ActionBase>() { new A_Idle(agent, timer) },
      new List<Conditions>()));
  }

  // Update is called once per frame
  void Update()
  {

  }

  public override string GetName()
  {
    return "G_Idle";
  }
}
