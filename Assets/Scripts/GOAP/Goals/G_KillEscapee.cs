using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_KillEscapee : GoalBase
{
  GameObject detectedEnemy;

  // TODO : Decide whether to use class constructor or Start()
  public G_KillEscapee(GameObject agent, GameObject detectedEnemy)
  {
    base.BaseStart(agent);
    
    this.detectedEnemy = detectedEnemy;

    List<ActionBase> list3 = new List<ActionBase>();
    list3.Add(new ApproachAndShoot(agent, detectedEnemy));
        
    waysToAchieveGoal.Add(new WayToGoal(list3, new List<Conditions>() { Conditions.HAS_AMMO }));
  }

  public override string GetName()
  {
    return "G_KillEscapee";
  }
}
