using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] InputActionReference _moveInput;
    [SerializeField] InputActionReference _lookInput;
    [SerializeField] float _movementSpeed;
    [SerializeField] float _rotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.PlayerMove();
        this.PlayerRotate();
    }

    private void PlayerMove()
    {
        characterController.Move(_movementSpeed * Time.deltaTime * GetInputDirection());
    }

    private void PlayerRotate()
    {
        var lookValues = _lookInput.action.ReadValue<Vector2>();
        transform.Rotate(0, lookValues.x * _rotateSpeed * Time.deltaTime , 0);
    }
    private Vector3 GetInputDirection()
    {
        var inputValues = _moveInput.action.ReadValue<Vector2>();
        var direction = transform.forward * inputValues.y + transform.right * inputValues.x;
        return direction.normalized;
    }
}
