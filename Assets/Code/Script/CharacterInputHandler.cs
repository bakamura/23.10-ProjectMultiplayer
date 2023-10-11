using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    private Vector2 _movmentDirection;

    // Update is called once per frame
    void Update()
    {
        _movmentDirection.x = Input.GetAxis("Horizontal");
        _movmentDirection.y = Input.GetAxis("Vertical");
    }

    public NetworkInputData GetInputData()
    {
        NetworkInputData temp = new NetworkInputData();

        temp.MovmentDirection = _movmentDirection;

        return temp;
    }
}
