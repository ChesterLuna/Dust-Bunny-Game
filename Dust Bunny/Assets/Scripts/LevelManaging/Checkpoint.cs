using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private bool _useCustomLocation;
    [SerializeField] private Vector3 _spawnLocation;
    // [SerializeField] private Sprite _closedSprite;
    // [SerializeField] private Sprite _openSprite;
    // [SerializeField] private Sprite _activatedSprite;
    // private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private CheckpointState _state = CheckpointState.closed;
    private Animator _animator;

    void Awake()
    {
        // _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (!_useCustomLocation)
        {
            _spawnLocation = transform.position;
        }
    } // end Start(

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IPlayerController controller)) return;

        ResetOtherCheckpoints();
        SetState(CheckpointState.active);
        GameManager.instance.CheckpointLocation = _spawnLocation;
        ES3AutoSaveMgr.Current.Save();
    } // end OnTriggerEnter2D

    private void ResetOtherCheckpoints()
    {
        var checkpoints = FindObjectsOfType<Checkpoint>();
        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (checkpoint != this)
            {
                checkpoint.ResetCheckpoint();
            }
        }
    } // end ResetCheckpoint

    public void ResetCheckpoint()
    {
        if (_state != CheckpointState.active) return;
        SetState(CheckpointState.open);
    } // end ResetCheckpoint

    private void SetState(CheckpointState state)
    {
        _state = state;
        switch (_state)
        {
            case CheckpointState.closed:
                _collider.enabled = true;
                // Nothing for now as this is the defaut state
                break;
            case CheckpointState.open:
                _collider.enabled = false;
                _animator.SetTrigger("Open");
                break;
            case CheckpointState.active:
                _collider.enabled = false;
                _animator.SetTrigger("Active");
                break;
        }
    } // end SetState

    public enum CheckpointState
    {
        closed,
        open,
        active
    } // end enum CheckpointState
} // end class Checkpoint
