using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_DealWithRobber : GoalBase
{
  GameObject detectedEnemy;

  // TODO : Decide whether to use class constructor or Start()
  public G_DealWithRobber(GameObject agent, GameObject detectedEnemy)
  {
    base.BaseStart(agent);
    
    this.detectedEnemy = detectedEnemy;

    // If enemy nearby and enough health, 
    // then set this goal as a current one

    // Add multiple ways to achieve the goal

    // 1. If robber has no weapon at all
    //    Arrest him and take him to prison
    // 2. If no ammo && enemy far away
    //    Get ammo -> Repeat(Approach enemy -> Shoot)
    // 3. If has ammo && enemy far away
    //    Repeat(Approach enemy -> Shoot)
    // 4. If enemy close
    //    Repeat(Approach enemy -> Melee)

    List<ActionBase> list1 = new List<ActionBase>();
    list1.Add(new A_ArrestAndBringToPrison(agent, detectedEnemy));

    List<ActionBase> list2 = new List<ActionBase>();
    list2.Add(new GetAmmo(agent));
    list2.Add(new ApproachAndShoot(agent, detectedEnemy));

    List<ActionBase> list3 = new List<ActionBase>();
    list3.Add(new ApproachAndShoot(agent, detectedEnemy));

    List<ActionBase> list4 = new List<ActionBase>();
    list4.Add(new ApproachAndMelee(agent, detectedEnemy));

    waysToAchieveGoal.Add(new WayToGoal(list1, new List<Conditions>()));
    //waysToAchieveGoal.Add(new WayToGoal(list2, new List<Conditions>()));
    waysToAchieveGoal.Add(new WayToGoal(list3, new List<Conditions>() { Conditions.HAS_AMMO }));
    waysToAchieveGoal.Add(new WayToGoal(list4, new List<Conditions>() { Conditions.HAS_BATON }));
  }

  public override string GetName()
  {
    return "G_DealWithRobber";
  }
}
