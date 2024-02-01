using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField] Switchable[] targets;
    [SerializeField] bool isOn = false;
    [SerializeField] Sprite onSprite;
    [SerializeField] Sprite offSprite;
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (isOn)
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
        isOn = true;
        foreach (Switchable target in targets)
        {
            target.enable();
        }
        spriteRenderer.sprite = onSprite;
    }

    void turnOff()
    {
        isOn = false;
        foreach (Switchable target in targets)
        {
            target.disable();
        }
        spriteRenderer.sprite = offSprite;
    }

    public void Interact()
    {
        isOn = !isOn;
        if (isOn)
        {
            turnOn();
        }
        else
        {
            turnOff();
        }
    }
}
