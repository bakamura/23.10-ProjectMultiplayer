using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterMovmentHandler : NetworkBehaviour
{
    private NetworkCharacterControllerPrototypeCustom _playerMovment;
    [SerializeField] private Camera _camera;
    private Vector2 _viewInput;
    private float _currentCamRotation;

    private void Awake()
    {
        _playerMovment = GetComponent<NetworkCharacterControllerPrototypeCustom>();
    }

    private void Update()
    {
        _currentCamRotation += Time.deltaTime * _viewInput.y;
        //_currentCamRotation = Mathf.Clamp(_currentCamRotation, -90f, 90f);

        _camera.transform.localRotation = Quaternion.Euler(0, _currentCamRotation, 0);
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData inputData))
        {
            Vector3 movDirection = transform.forward * inputData.MovmentDirection.y + transform.right * inputData.MovmentDirection.x;
            _playerMovment.Move(movDirection.normalized);
        }
    }

    public void SetViewInputVector(Vector2 value)
    {
        _viewInput = value;
    }

}
