using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonPromptTutorial : MonoBehaviour
{
    public enum Type{MOVEMENT, DASH, JUMP, WALLJUMP, INTERACT}

    public Type type;

    private TextMeshPro _text;
    private Animator _anim;
    private IPlayerController _player;
    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponentInChildren<TextMeshPro>();
        _anim = GetComponent<Animator>();
        SetupByType();
    }

    // Update is called once per frame
    void Update()
    {
        FrameInput inputs = UserInput.instance.Gather();
        bool correctInput = false;
        switch (type){
            case Type.MOVEMENT:
                correctInput = correctInput || inputs.Move.magnitude > 0;
                break;
            case Type.DASH:
                correctInput = correctInput || inputs.DashDown;
                break;
            case Type.JUMP:
                correctInput = correctInput || inputs.JumpDown;
                break;
            case Type.WALLJUMP:
                correctInput = correctInput || inputs.JumpDown;
                break;
            case Type.INTERACT:
                correctInput = correctInput || inputs.InteractDown;
                break;
        }

        if (correctInput){
            OnCorrectInput();
        }

        // Move toward the player
        transform.position = Vector3.Lerp(transform.position, (Vector3)_player.State.Position + Vector3.up * 0.1f * _player.CurrentDust, 15f * Time.deltaTime);
    }

    void OnCorrectInput(){
        _anim.SetTrigger("fadeOut");
    }

    public void OnAnimationFinish(){
        Destroy(gameObject);
    }

    public void SetPlayerReference(IPlayerController player){
        _player = player;
    }

    private void SetupByType(){
        switch (type){
            case Type.MOVEMENT:
                _text.text = "<b>Move</b>\n<i>" + UserInput.instance.GetInputNames().MovementKeys + "</i>";
                break;
            case Type.DASH:
                _text.text = "<b>Dash</b>\n<i>" + UserInput.instance.GetInputNames().DashKey + "</i>";
                break;
            case Type.JUMP:
                _text.text = "<b>Jump</b>\n<i>" + UserInput.instance.GetInputNames().JumpKey + "</i>";
                break;
            case Type.WALLJUMP:
                _text.text = "<b>Walljump</b>\n<i>" + UserInput.instance.GetInputNames().JumpKey + "</i>";
                break;
            case Type.INTERACT:
                _text.text = "<b>Interact</b>\n<i>" + UserInput.instance.GetInputNames().InteractKey + "</i>";
                break;
        }
    }
}
