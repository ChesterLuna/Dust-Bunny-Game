using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField] Switchable[] _targets;
    [SerializeField] bool _isOn = false;
    [SerializeField] Sprite _onSprite;
    [SerializeField] Sprite _offSprite;
    SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        if (_isOn)
        {
            turnOn();
        }
        else
        {
            turnOff();
        }
    }

    void turnOn()
    {
        _isOn = true;
        foreach (Switchable target in _targets)
        {
            target.enable();
        }
        _spriteRenderer.sprite = _onSprite;
    }

    void turnOff()
    {
        _isOn = false;
        foreach (Switchable target in _targets)
        {
            target.disable();
        }
        _spriteRenderer.sprite = _offSprite;
    }

    public void Interact()
    {
        _isOn = !_isOn;
        if (_isOn)
        {
            turnOn();
        }
        else
        {
            turnOff();
        }
    }
}
