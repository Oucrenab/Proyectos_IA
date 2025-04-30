using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionNode : TreeNode
{
    [SerializeField] TreeNode _trueNode;
    [SerializeField] TreeNode _falseNode;
    [SerializeField] TypeQuestion _myQuestion;

    public override void Execute(Boid boid)
    {
        //Debug.Log($" Execute {name}");

        switch (_myQuestion)
        {
            case TypeQuestion.HunterClose:
                //Evade
                if(boid.CheckForHunter())
                    _trueNode.Execute(boid);
                else
                    _falseNode.Execute(boid);

                break;
            case TypeQuestion.FoodClose:
                //Arribe

                if (boid.CheckForFood())
                    _trueNode.Execute(boid);
                else
                    _falseNode.Execute(boid);

                break;
            case TypeQuestion.BoidClose:
                //Moverse con el grupo

                if (boid.CheckForBoids())
                    _trueNode.Execute(boid);
                else
                    _falseNode.Execute(boid);
                break;

        }
    }


}

public enum TypeQuestion
{
    HunterClose,
    FoodClose,
    BoidClose
}
