using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class InGameInfoMenu : MonoBehaviour
{

    TextMeshProUGUI _titleUI;
    TextMeshProUGUI _infoUI;
    TextMeshProUGUI _backbuttonUI;
    TextMeshProUGUI _nextbuttonUI;
    PauseMenuOLD _pauseMenu;


    int currentPage = 0;

    void Awake()
    {
        _titleUI = GameObject.Find("InfoCanvas/Title").GetComponent<TextMeshProUGUI>();
        _infoUI = GameObject.Find("Info Menu/InfoText").GetComponent<TextMeshProUGUI>();
        _backbuttonUI = GameObject.Find("Info Menu/Back Button/Text (TMP)").GetComponent<TextMeshProUGUI>();
        _nextbuttonUI = GameObject.Find("Info Menu/Next Button/Text (TMP)").GetComponent<TextMeshProUGUI>();
        _pauseMenu = GameObject.Find("MENU").GetComponent<PauseMenuOLD>();
    } // end Awake

    void OnEnable()
    {
        currentPage = 0;
        SetPage();
    } // end OnEnable

    public void SetPage()
    {
        _titleUI.text = InfoMenu.infoTitles[currentPage];
        _infoUI.text = InfoMenu.infoText[currentPage];
        _nextbuttonUI.text = "Next";
        _backbuttonUI.text = "Back";
    }  // end SetPage

    public void Next()
    {
        if (currentPage < InfoMenu.infoTitles.Length - 1)
        {
            currentPage++;
            SetPage();
        }
        else
        {
            _pauseMenu.SetMenu(PauseMenuOLD.PauseMenuPage.Pause);
        }
    } // end Next

    public void Back()
    {
        if (currentPage > 0)
        {
            currentPage--;
            SetPage();
        }
        else
        {
            _pauseMenu.SetMenu(PauseMenuOLD.PauseMenuPage.Pause);
        }
    } // end Back
} // end PauseMenu
