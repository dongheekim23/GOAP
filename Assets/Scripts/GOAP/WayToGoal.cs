using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Conditions
{
  HAS_AMMO,
  HAS_BATON,
  HAS_KEY,
  HAS_POTION,
}

public class WayToGoal
{
  public List<ActionBase> actionSequence;
  
  // There is no precondition if its size is 0
  List<Conditions> preconditionList;

  public WayToGoal(List<ActionBase> actionSequence, List<Conditions> preconditionList)
  {
    this.actionSequence = actionSequence;

    /*
     
    0 : has ammo
    1 : has baton
    2 : has key
    3 : has potion
     
    */

    this.preconditionList = preconditionList;
  }

  public bool CheckPreconditions(List<Conditions> agentConditionList)
  {
    for (int i = 0; i < preconditionList.Count; ++i)
    {
      if (agentConditionList.Contains(preconditionList[i]) == false)
        return false;
    }

    return true;
  }

  public bool CheckPreconditions(AgentCondition agentCondition)
  {
    for (int i = 0; i < preconditionList.Count; ++i)
    {
      if(preconditionList[i] == Conditions.HAS_AMMO)
      {
        if (agentCondition.ammoCount <= 0)
          return false;
      }
      else if(preconditionList[i] == Conditions.HAS_BATON)
      {
        if (agentCondition.hasBaton == false)
          return false;
      }
      else if (preconditionList[i] == Conditions.HAS_KEY)
      {
        if (agentCondition.hasKey == false)
          return false;
      }
      else if (preconditionList[i] == Conditions.HAS_POTION)
      {
        if (agentCondition.hasPotion == false)
          return false;
      }
    }

    return true;
  }
}
