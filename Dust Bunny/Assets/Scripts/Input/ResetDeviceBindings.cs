using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetDeviceBindings : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputAction;
    [SerializeField] private string _targetControlScheme;

    public void ResetAllBindings()
    {
        foreach (InputActionMap map in _inputAction.actionMaps)
        {
            foreach (InputAction action in map.actions)
            {
                action.RemoveAllBindingOverrides();
            }
        }
    }

    public void ResetControlSchemeBindings()
    {
        foreach (InputActionMap map in _inputAction.actionMaps)
        {
            foreach (InputAction action in map.actions)
            {
                action.RemoveBindingOverride(InputBinding.MaskByGroup(_targetControlScheme));
            }
        }
    }
}
