using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterMovmentHandler : NetworkBehaviour
{
    private NetworkCharacterControllerPrototypeCustom _playerMovment;

    private void Awake()
    {
        _playerMovment = GetComponent<NetworkCharacterControllerPrototypeCustom>();
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData inputData))
        {
            Vector3 movDirection = transform.forward * inputData.MovmentDirection.y + transform.right * inputData.MovmentDirection.x;
            _playerMovment.Move(movDirection.normalized);
        }
    }

}
