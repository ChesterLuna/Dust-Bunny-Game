using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpringCleaning.Player;
public class RebindMenu : MonoBehaviour
{
    public Toggle DashMouseToggle;
    private void OnEnable()
    {
        DashMouseToggle.isOn = UserInput.Instance.UseMouseForDash;
        DashMouseToggle.onValueChanged.AddListener((value) =>
        {
            UserInput.Instance.SetMouseForDash(value);
        });
    } // end OnEnable

    void OnDisable()
    {
        DashMouseToggle.onValueChanged.RemoveAllListeners();
    } // end OnDisable
} // end RebindMenu

