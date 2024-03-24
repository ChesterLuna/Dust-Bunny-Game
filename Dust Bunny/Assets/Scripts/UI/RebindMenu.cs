using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RebindMenu : MonoBehaviour
{
    [SerializeField] private Toggle _dashMouseToggle;
    private void OnEnable()
    {
        _dashMouseToggle.isOn = UserInput.instance.UseMouseForDash;
        _dashMouseToggle.onValueChanged.AddListener((value) => UserInput.instance.UseMouseForDash = value);
    } // end OnEnable

    void OnDisable()
    {
        _dashMouseToggle.onValueChanged.RemoveAllListeners();
    } // end OnDisable
} // end RebindMenu

