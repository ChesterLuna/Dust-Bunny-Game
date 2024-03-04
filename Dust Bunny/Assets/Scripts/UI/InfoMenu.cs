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
        "In this game, you control a dust bunny named Spek.\n\nUse WASD or arrow keys to move, SPACE to jump, and E to interact. SHIFT triggers a dash, aimed in the direction of the mouse cursor.",
        "Dust clouds are scattered throughout the map. Contact with a cloud will cause Spek to grow, decreasing their movement speed but increasing jump height.\n\nCertain hazards can take away dust and cause Spek to shrink, increasing movement speed but decreasing jump height. Be careful! Losing dust when Spek is too small is a fatal mistake!",
        "Spek may encounter various other objects on their journey, such as:\n-Levers, which affect the state of the map when interacted with.\n-Velcro, which is especially dangerous to any dust bunny who has the misfortune of touching it.\n-Fans, which will push Spek in the direction of their air currents.\n-Vacuums and Swiffers remove dust if Spek gets too close. Some of these hazards move around! But they might just miss a spot when their back is turned..."
    };
    string[] infoTitles = new string[]
    {
        "CONTROLS",
        "DUST",
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
        UISFXManager.PlaySFX(UISFXManager.SFX.NAVIGATE);
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
        UISFXManager.PlaySFX(UISFXManager.SFX.NAVIGATE);
    } // end Back
} // end InfoMenu
