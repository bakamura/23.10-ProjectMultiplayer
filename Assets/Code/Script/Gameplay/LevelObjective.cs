using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelObjective : NetworkBehaviour {

    [SerializeField] protected string levelToLoadWhenObjectiveComplete;
    public abstract bool CheckObjective();

}
