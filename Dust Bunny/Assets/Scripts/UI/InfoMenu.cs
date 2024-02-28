using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InfoMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [SerializeField] TextMeshProUGUI _titleUI;
    [SerializeField] TextMeshProUGUI _infoUI;

    [SerializeField] TextMeshProUGUI _backbuttonUI;
    [SerializeField] TextMeshProUGUI _nextbuttonUI;

    int currentPage = 0;
    string[] infoText = new string[]
    {
        "\tIn this game, you control the actions of the dust bunny, Spek. You can move using WASD or the arrow keys, and jump with the spacebar. Pressing shift allows you to dash in the direction of your mouse cursor, and you can interact with certain objects and NPCs by pressing E.",
        "\tScattered across the map, you may find clouds of dust. These clouds will grant Spek dust when you come in contact with them, making Spek bigger, decreasing your movement speed but increasing your jump height. Conversely, you may encounter vacuums, which will take away dust when you come in contact with them and cause Spek to shrink, increasing your movement speed but decreasing your jump height. Furthermore, touching a vacuum when Spek is too small will cause a game over.",
        "\tVarious other hazards and objects are scattered across the map. Currently, you may encounter the following:\n\t-Levers, which affect the state of the map when interacted with.\n\t-Velcro, which is particularly dangerous to any dust bunny and will cause a game over if touched.\n\t-Fans, which will push Spek in a given direction.\n\t-Vacuums and Swiffers, which will decrease Spek's size if touched, but have blind spots that are safe to touch and may even move across the map."
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
        "Main Menu"
    };

    string[] backButtonText = new string[]
    {
        "Main Menu",
        "Back",
        "Back"
    };

    void Start()
    {
        SetPage();
    } // end Start

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
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
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
            UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
        }
    } // end Back
} // end InfoMenu
