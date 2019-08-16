using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Return : GoalBase
{

  public G_Return(GameObject agent)
  {
    base.BaseStart(agent);

    List<ActionBase> list1 = new List<ActionBase>();
    list1.Add(new A_Idle(agent));

    List<ActionBase> list2 = new List<ActionBase>();
    list2.Add(new A_GoBackToStation(agent));

    waysToAchieveGoal.Add(new WayToGoal(list1, new List<Conditions>()));
    waysToAchieveGoal.Add(new WayToGoal(list2, new List<Conditions>()));
  }

  public override string GetName()
  {
    return "G_DealWithPolice";
  }
}
