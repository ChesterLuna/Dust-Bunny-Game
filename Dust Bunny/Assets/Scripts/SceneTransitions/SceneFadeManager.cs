using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Sirenix.OdinInspector;

public class SceneFadeManager : MonoBehaviour
{
    public static SceneFadeManager Instance;
    [SerializeField, Required, ChildGameObjectsOnly] private Image _fadeImage;
    [Range(0.1f, 10f)][SerializeField] private float _fadeOutSpeed = 5f;
    [Range(0.1f, 10f)][SerializeField] private float _fadeInSpeed = 5f;
    [SerializeField] private Color _fadeColor = Color.black;
    public bool IsFadingOut { get; private set; }
    public bool IsFadingIn { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _fadeColor.a = 0;
            _fadeImage.color = _fadeColor;
        }
        else
        {
            Debug.LogWarning("There are multiple SceneFadeManagers in the scene. Deleting the newest one.");
            Destroy(this);
        }
    }

    private void Update()
    {
        if (IsFadingOut)
        {
            _fadeColor.a = Mathf.MoveTowards(_fadeColor.a, 1, Time.deltaTime * _fadeOutSpeed);
            _fadeImage.color = _fadeColor;
            if (_fadeColor.a == 1)
            {
                IsFadingOut = false;
            }
        }

        if (IsFadingIn)
        {
            _fadeColor.a = Mathf.MoveTowards(_fadeColor.a, 0, Time.deltaTime * _fadeInSpeed);
            _fadeImage.color = _fadeColor;
            if (_fadeColor.a == 0)
            {
                IsFadingIn = false;
            }
        }
    }

    public void StartFadeOut()
    {
        _fadeImage.color = _fadeColor;
        IsFadingOut = true;
    }

    public void StartFadeIn()
    {
        _fadeImage.color = _fadeColor;
        IsFadingIn = true;
    }

}
