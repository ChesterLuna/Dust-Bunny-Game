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
    [SerializeField] bool _showIndicator = true;
    public bool showIndicator => _showIndicator;

    // Start is called before the first frame update
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        foreach (GameObject target in _targetGameObjects)
        {
            _targets.Add(target.GetComponentInChildren<ISwitchable>());
        }

    } // end Awake

    void Start()
    {
        if (_isOn)
        {
            TurnOn();
        }
        else
        {
            TurnOff();
        }
    } // end Start

    void TurnOn()
    {
        _isOn = true;
        foreach (ISwitchable target in _targets)
        {
            target.Enable();
        }
        _spriteRenderer.sprite = _onSprite;
    }

    void TurnOff()
    {
        _isOn = false;
        foreach (ISwitchable target in _targets)
        {
            target.Disable();
        }
        _spriteRenderer.sprite = _offSprite;
    } // end TurnOff

    public void Interact()
    {
        _isOn = !_isOn;
        if (_isOn)
        {
            TurnOn();
        }
        else
        {
            TurnOff();
        }
    } // end Interact
} // end Switch
