using Fusion;
using UnityEngine;

public struct DataPackInput : INetworkInput {

    public Vector2 Movement;
    public NetworkBool Jump;
    public NetworkBool Action1;
    public NetworkBool Action2;
    public NetworkBool Action3;

}
