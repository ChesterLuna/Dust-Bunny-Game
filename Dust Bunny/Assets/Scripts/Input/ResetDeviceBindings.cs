using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ResetDeviceBindings : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputAction;
    [SerializeField] private string _targetControlScheme;
    private RebindMenu rebindMenu;

    public void ResetAllBindings()
    {
        rebindMenu = gameObject.GetComponent<RebindMenu>();
        rebindMenu.DashMouseToggle.isOn = true;
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
        rebindMenu = GetComponent<RebindMenu>();
        rebindMenu.DashMouseToggle.isOn = true;
        foreach (InputActionMap map in _inputAction.actionMaps)
        {
            foreach (InputAction action in map.actions)
            {
                action.RemoveBindingOverride(InputBinding.MaskByGroup(_targetControlScheme));
            }
        }
    }
}
