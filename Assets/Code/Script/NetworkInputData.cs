using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
//quando estiver enviando dados a documenta��o do photon recomenda utilizar apenas bytes para melhor performance, mas n � obrigat�rio
public struct NetworkInputData : INetworkInput
{
    public Vector2 MovmentDirection;
    public float RotationYAxis;
    public NetworkBool IsJumpPressed;
}
