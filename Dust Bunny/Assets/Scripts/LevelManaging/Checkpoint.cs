using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using SpringCleaning.Player;
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
    private GameObject _particleSystemObject;
    private AudioSource _sfx;

    void Awake()
    {
        // _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        _particleSystemObject = GetComponentInChildren<ParticleSystem>().gameObject;
        _sfx = GetComponent<AudioSource>();
    }

    void Start()
    {
        _particleSystemObject?.SetActive(false);
        if (!_useCustomLocation)
        {
            _spawnLocation = transform.position;
        }
        SetState(_state);
    } // end Start(

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IPlayerController controller)) return;

        ResetOtherCheckpoints();
        SetState(CheckpointState.active);
        GameManager.instance.CheckpointLocation = _spawnLocation;
        GameManager.instance.CheckpointDustLevel = controller.CurrentDust;
        ES3AutoSaveMgr.Current.Save();
        _sfx.Play();
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
        _particleSystemObject.SetActive(false);
    } // end ResetCheckpoint

    private void SetState(CheckpointState state)
    {
        _state = state;
        switch (_state)
        {
            case CheckpointState.closed:
                _collider.enabled = true;
                _particleSystemObject.SetActive(false);
                // Nothing for now as this is the defaut state
                break;
            case CheckpointState.open:
                _collider.enabled = false;
                _particleSystemObject.SetActive(false);
                _animator.SetTrigger("Open");
                break;
            case CheckpointState.active:
                _collider.enabled = false;
                _particleSystemObject.SetActive(true);
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
