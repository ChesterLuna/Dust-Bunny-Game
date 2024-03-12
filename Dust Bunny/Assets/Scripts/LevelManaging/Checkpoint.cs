using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    [SerializeField] private bool _useCustomLocation;
    [SerializeField] private Vector3 _spawnLocation;

    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _activatedSprite;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }

    void Start()
    {
        if (!_useCustomLocation)
        {
            _spawnLocation = transform.position;
        }
    } // end Start(

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ResetOtherCheckpoints();
            _spriteRenderer.sprite = _activatedSprite;
            _collider.enabled = false;
            GameManager.instance.CheckpointLocation = _spawnLocation;
            ES3AutoSaveMgr.Current.Save();
        }
    } // end OnTriggerEnter2D

    private void ResetOtherCheckpoints()
    {
        var checkpoints = FindObjectsOfType<Checkpoint>();
        foreach (Checkpoint checkpoint in checkpoints)
        {
            checkpoint.ResetCheckpoint();
        }
    } // end ResetCheckpoint

    public void ResetCheckpoint()
    {
        _collider.enabled = true;
        _spriteRenderer.sprite = _normalSprite;
    } // end ResetCheckpoint

} // end class Checkpoint
