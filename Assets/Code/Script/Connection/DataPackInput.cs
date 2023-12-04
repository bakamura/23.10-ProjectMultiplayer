using Fusion;
using UnityEngine;

namespace ProjectMultiplayer.Connection
{
    public struct DataPackInput : INetworkInput
    {
        public float CameraYAngle;
        public Vector2 Movement;
        public NetworkBool Jump;
        public NetworkBool Action1;
        public NetworkBool Action2;
        public NetworkBool Action3;

    }
}