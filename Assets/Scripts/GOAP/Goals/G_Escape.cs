using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Escape : GoalBase
{
    private GameObject goal;

    // TODO : Decide whether to use class constructor or Start()
    public G_Escape(GameObject agent, GameObject _goal)
    {
        base.BaseStart(agent);

        this.goal = _goal;

        // If enemy nearby and enough health, 
        // then set this goal as a current one

        // Add multiple ways to achieve the goal

        // 1. If key is close enough
        //    get a key and open
        // 2. If key is too far
        //    smash the door to open

        List<ActionBase> list1 = new List<ActionBase>();
        list1.Add(new A_Escape(agent, goal));

        waysToAchieveGoal.Add(new WayToGoal(list1, new List<Conditions>()));
    }

    public override string GetName()
    {
        return "G_Escape";
    }
}
