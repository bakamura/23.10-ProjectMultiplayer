using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer LocalPlayer;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            LocalPlayer = this;

            Debug.Log("Spawned local player");
        }
        else
        {
            HandleMultipleCameras();
            Debug.Log("Spawned remote player");
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        //envia uma mensagem para o server para ele ser destruido
        if (player == Object.InputAuthority)
            Runner.Despawn(Object);
    }
    
    private void HandleMultipleCameras()
    {
        GetComponentInChildren<Camera>().enabled = false;
        GetComponentInChildren<AudioListener>().enabled = false;
    }
}
