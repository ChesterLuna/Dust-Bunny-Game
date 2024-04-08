using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;



public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject[] _targetGameObjects;
    [SerializeField] List<ISwitchable> _targets = new();
    [SerializeField] bool _isOn = false;
    [SerializeField] Sprite _onSprite;
    [SerializeField] Sprite _offSprite;
    SpriteRenderer _spriteRenderer;
    public bool ShowIndicator { get; private set; } = true;

    private AudioSource _turnOnSFX;
    private AudioSource _turnOffSFX;

    // Start is called before the first frame update
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        foreach (GameObject target in _targetGameObjects)
        {
            _targets.Add(target.GetComponentInChildren<ISwitchable>());
        }

        _turnOnSFX = GetComponents<AudioSource>()[0];
        _turnOffSFX = GetComponents<AudioSource>()[1];

    } // end Awake

    void Start()
    {
        if (_isOn)
        {
            TurnOn(false);
        }
        else
        {
            TurnOff(false); 
        }
    } // end Start

    void TurnOn(bool shouldPlaySFX)
    {
        _isOn = true;
        foreach (ISwitchable target in _targets)
        {
            target.Enable();
        }
        _spriteRenderer.sprite = _onSprite;

        if (shouldPlaySFX) _turnOnSFX.Play();
    }

    void TurnOff(bool shouldPlaySFX)
    {
        _isOn = false;
        foreach (ISwitchable target in _targets)
        {
            target.Disable();
        }
        _spriteRenderer.sprite = _offSprite;

        if (shouldPlaySFX) _turnOffSFX.Play();
    } // end TurnOff

    public void Interact()
    {
        _isOn = !_isOn;
        if (_isOn)
        {
            TurnOn(true);
        }
        else
        {
            TurnOff(true);
        }
    } // end Interact
} // end Switch
