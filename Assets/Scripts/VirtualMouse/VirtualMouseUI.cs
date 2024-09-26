using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;

public class VirtualMouseUI : MonoBehaviour
{

    [SerializeField] private RectTransform canvasRectTransform;
    private VirtualMouseInput virtualMouseInput;
    private void Awake()
    {
        virtualMouseInput = GetComponent<VirtualMouseInput>();
        //InputSystem.onActionChange += OnInputActionChange;
        InputState.Change(virtualMouseInput.virtualMouse.position, new Vector2(Screen.width / 2, Screen.height / 2));
    }

    // private void OnInputActionChange(object arg1, InputActionChange change)
    // {
    //     if(change == InputActionChange.ActionPerformed && arg1 is InputAction){
    //         InputAction inputAction = arg1 as InputAction;
    //         if(inputAction.activeControl.device.displayName == "VirtualMouse"){
    //             return;
    //         }
    //         if(inputAction.activeControl.device is Gamepad){
    //             if()
    //         }
    //     }
    // }

    private void Update()
    {
        transform.localScale = Vector3.one * (1f / canvasRectTransform.localScale.x);
    }
    private void LateUpdate()
    {
        Vector2 virtualMousePosition = virtualMouseInput.virtualMouse.position.value;
        virtualMousePosition.x = Mathf.Clamp(virtualMousePosition.x, 0f, Screen.width);
        virtualMousePosition.y = Mathf.Clamp(virtualMousePosition.y, 0f, Screen.height);
        InputState.Change(virtualMouseInput.virtualMouse.position, virtualMousePosition);
    }
}
