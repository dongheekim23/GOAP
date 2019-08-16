using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_Opendoor : GoalBase
{
    private GameObject door;
    private GameObject key;

    // TODO : Decide whether to use class constructor or Start()
    public G_Opendoor(GameObject agent, GameObject _door, GameObject _key)
    {
        base.BaseStart(agent);

        this.door = _door;
        this.key = _key;

        // If enemy nearby and enough health, 
        // then set this goal as a current one

        // Add multiple ways to achieve the goal

        // 1. If key is close enough
        //    get a key and open
        // 2. If key is too far
        //    smash the door to open

        List<ActionBase> list1 = new List<ActionBase>();
        list1.Add(new GetKey(agent, key));
        list1.Add(new ApproachAndOpendoor(agent, door));

        List<ActionBase> list2 = new List<ActionBase>();
        list2.Add(new ApproachAndSmashdoor(agent, door));

        List<ActionBase> list3 = new List<ActionBase>();
        list3.Add(new ApproachAndOpendoor(agent, door));

        waysToAchieveGoal.Add(new WayToGoal(list1, new List<Conditions>()));
        waysToAchieveGoal.Add(new WayToGoal(list2, new List<Conditions>()));
        waysToAchieveGoal.Add(new WayToGoal(list3, new List<Conditions>() { Conditions.HAS_KEY }));
    }

    public override string GetName()
    {
        return "G_Opendoor";
    }
}
