using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : TreeNode
{
    //escapar
    //arribe food
    //flocking
    //movimiento aleatorio

    [SerializeField] TypeAction _myAction;

    public override void Execute(Boid boid)
    {
        //Debug.Log($" Execute {name}");


        switch (_myAction)
        {
            case TypeAction.EvadeHunter:
                boid.StartEvadeHunter();
                break;
            case TypeAction.ArribeFood:
                boid.StartFoodArribe();
                break;
            case TypeAction.DoFlocking:
                boid.StartFlocking();
                break;
            case TypeAction.RandomMovement:
                boid.StartRandomMovement();
                break;
        }
    }
}

public enum TypeAction
{
    EvadeHunter,
    ArribeFood,
    DoFlocking,
    RandomMovement
}
