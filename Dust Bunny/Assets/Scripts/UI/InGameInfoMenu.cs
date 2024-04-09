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
    PauseMenu _pauseMenu;


    int currentPage = 0;
    string[] infoText = new string[]
    {
        "\tIn this game, you control the actions of the dust bunny, Spek. You can move using WASD or the arrow keys, and jump with the spacebar. Clicking allows you to dash in the direction of your mouse cursor, and you can interact with certain objects and NPCs by pressing E. These controls can be customized in the settings.",
        "\tScattered across the map, you may find clouds of dust. These clouds will grant Spek dust when you come in contact with them, making Spek bigger, increasing your jump height. Conversely, you may encounter vacuums, which will take away dust when you come in contact with them and cause Spek to shrink, decreasing your jump height. Furthermore, touching a vacuum when Spek is too small will cause a game over.",
        "\tVarious other hazards and objects are scattered across the map. You may encounter the following:\n\t-Levers, which affect the state of the map when interacted with.\n\t-Velcro, which is particularly dangerous to any dust bunny and will cause a game over if touched.\n\t-Fans, which will push Spek in a given direction.\n\t-Vacuums and Swiffers, which will decrease Spek's size if touched, but have blind spots that are safe to touch and may even move across the map."
    };
    string[] infoTitles = new string[]
    {
        "CONTROLS",
        "DUST AND VACUUMS",
        "HAZARDS AND OBJECTS"
        };

    string[] nextButtonText = new string[]
    {
        "Next",
        "Next",
        "Next"
    };

    string[] backButtonText = new string[]
    {
        "Back",
        "Back",
        "Back"
    };

    void Awake()
    {
        _titleUI = GameObject.Find("InfoCanvas/Title").GetComponent<TextMeshProUGUI>();
        _infoUI = GameObject.Find("Info Menu/InfoText").GetComponent<TextMeshProUGUI>();
        _backbuttonUI = GameObject.Find("Info Menu/Back Button/Text (TMP)").GetComponent<TextMeshProUGUI>();
        _nextbuttonUI = GameObject.Find("Info Menu/Next Button/Text (TMP)").GetComponent<TextMeshProUGUI>();
        _pauseMenu = GameObject.Find("MENU").GetComponent<PauseMenu>();
    } // end Awake

    void OnEnable()
    {
        currentPage = 0;
        SetPage();
    } // end OnEnable

    public void SetPage()
    {
        _titleUI.text = infoTitles[currentPage];
        _infoUI.text = infoText[currentPage];
        _nextbuttonUI.text = nextButtonText[currentPage];
        _backbuttonUI.text = backButtonText[currentPage];
    }  // end SetPage

    public void Next()
    {
        if (currentPage < infoText.Length - 1)
        {
            currentPage++;
            SetPage();
        }
        else
        {
            _pauseMenu.SetMenu(PauseMenu.PauseMenuPage.Pause);
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
            _pauseMenu.SetMenu(PauseMenu.PauseMenuPage.Pause);
        }
    } // end Back
} // end PauseMenu
