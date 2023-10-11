using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    private Vector2 _movmentDirection;
    private Vector2 _mousePosition;
    private CharacterMovmentHandler _movmentHandler;
    private bool _isJumping;

    private void Awake()
    {
        _movmentHandler = GetComponent<CharacterMovmentHandler>();
    }

    void Update()
    {
        _mousePosition.x = Input.GetAxis("Mouse X");
        _mousePosition.y = Input.GetAxis("Mouse Y") * -1;


        _movmentDirection.x = Input.GetAxis("Horizontal");
        _movmentDirection.y = Input.GetAxis("Vertical");

        _movmentHandler.SetViewInputVector(_mousePosition);
        _isJumping = Input.GetButtonDown("Jump");
    }

    public NetworkInputData GetInputData()
    {
        NetworkInputData temp = new NetworkInputData();

        temp.RotationYAxis = _mousePosition.x;

        temp.MovmentDirection = _movmentDirection;

        temp.IsJumpPressed = _isJumping;

        return temp;
    }
}
