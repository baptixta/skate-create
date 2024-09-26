using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;

public class VirtualMouseUI : MonoBehaviour
{

    [SerializeField] private RectTransform canvasRectTransform;
    private VirtualMouseInput virtualMouseInput;

    public enum GameDevice
    {
        KeyboardMouse,
        Gamepad
    }

    private GameDevice activeGameDevice;


    private void Start()
    {
        virtualMouseInput = GetComponent<VirtualMouseInput>();
        InputSystem.onActionChange += OnInputActionChange;
        ResetMouseToCenter();
    }

    private void OnInputActionChange(object arg1, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed && arg1 is InputAction)
        {
            InputAction inputAction = arg1 as InputAction;
            if (inputAction.activeControl.device.displayName == "VirtualMouse")
            {
                return;
            }
            if (inputAction.activeControl.device is Gamepad)
            {
                if (activeGameDevice != GameDevice.Gamepad)
                {
                    ChangeActiveGameDevice(GameDevice.Gamepad);
                }
            }
            else
            {
                if (activeGameDevice != GameDevice.KeyboardMouse)
                {
                    ChangeActiveGameDevice(GameDevice.KeyboardMouse);
                }
            }
        }
    }

    private void ChangeActiveGameDevice(GameDevice activeGameDevice)
    {
        this.activeGameDevice = activeGameDevice;
        Cursor.visible = activeGameDevice == GameDevice.KeyboardMouse;
        //UpdateVisibility();
    }

    void UpdateVisibility()
    {
        if (activeGameDevice == GameDevice.Gamepad)
        {
            ResetMouseToCenter();
            transform.GetComponentInChildren<Image>().enabled = true;
        }
        else
        {
            transform.GetComponentInChildren<Image>().enabled = false;
        }
    }

    private void ResetMouseToCenter()
    {
        InputState.Change(virtualMouseInput.virtualMouse.position, new Vector2(Screen.width / 2, Screen.height / 2));
    }

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
