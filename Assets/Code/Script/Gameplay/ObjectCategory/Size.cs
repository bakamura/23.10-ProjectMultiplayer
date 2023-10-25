using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Size : NetworkBehaviour { 

    public enum SizeType {
        S,
        M,
        B
    }
    [SerializeField] private SizeType _sizeType;


    // Access
    public SizeType Type { get {  return _sizeType; } }

    public void ChangeSize(bool isGrowing) {

    }

}
