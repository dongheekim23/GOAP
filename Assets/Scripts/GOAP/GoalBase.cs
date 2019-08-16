using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GoalBase// : MonoBehaviour
{
  // Assign multiple sets of actions for each goal
  // ex) 1. “Approach” => “Melee”
  //     2. “Get ammo” => “Approach” => “Shoot”
  protected List<WayToGoal> waysToAchieveGoal;
  protected bool isGoalAchieved;
  protected AgentCondition agentCondition;
  protected GameObject agent;

  protected void BaseStart(GameObject agent)
  {
    this.agent = agent;
    waysToAchieveGoal = new List<WayToGoal>();
    //isGoalAchieved = false;
    agentCondition = agent.GetComponent<AgentCondition>();
  }

  // Use this for initialization
  void Start()
  {
    
  }
  
  // Check if the goal has been achieved
  public bool IsGoalAchieved()
  {
    return isGoalAchieved;
  }

  // Get the list of actions with the minimum total cost based on calculation
  public List<ActionBase> GetActionListWithLowestCost()
  {
    int itWithMinCost = 0;
    float minCost = float.MaxValue;

    // Go through each list stored in waysToAchieveGoal
    // and compute the total cost
    for (int i = 0; i < waysToAchieveGoal.Count; ++i)
    {
      // If the agent condition does not meet the precondition
      // of the action sequence, then skip the sequence
      if (waysToAchieveGoal[i].CheckPreconditions(agentCondition) == false)
        continue;

      float costPerList = 0f;

      foreach (ActionBase action in waysToAchieveGoal[i].actionSequence)
      {
        costPerList += action.ComputeCost();
      }

      if (costPerList < minCost)
      {
        itWithMinCost = i;
        minCost = costPerList;
      }
    }

    // Return the list with minimum cost
    return waysToAchieveGoal[itWithMinCost].actionSequence;
  }

  public abstract string GetName();
}
