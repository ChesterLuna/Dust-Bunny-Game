using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DustChangeTicker : MonoBehaviour
{
    [Tooltip("The maximum amount of dust this object can give/take before it is destroyed (Absolute Value). Set to -1 to make it infinite.")]
    [SerializeField] float _maxDustToExchange = 100;
    [Tooltip("The amount of dust to give/take each tick, can be positive or negative")]
    [SerializeField] float _dustTickAmount = 10;
    float _amountOfDust = 0;
    IPlayerController _player;

    [Tooltip("How often the dust will be given/taken (in seconds).")]
    [SerializeField] float _dustTickRate = 1f;
    [Tooltip("How long the object will be inactive after reaching the max dust.")]
    [SerializeField] float _cooldownTime = 5f;
    Collider2D _collider;
    SpriteRenderer _spriteRenderer;
    [SerializeField] Sprite _small;
    [SerializeField] Sprite _mid;
    [SerializeField] Sprite _large;

    private IEnumerator coroutine;


    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_dustTickAmount == 0)
        {
            Debug.LogError("Dust tick amount is 0, this will cause the dust to never change");
        }
        UpdateSprite();
    } // end Awake

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.gameObject.TryGetComponent(out _player);
        if (_player != null)
        {
            coroutine = TryChangeDust();
            StartCoroutine(coroutine);
        }
    } // end OnTriggerEnter2D

    private void OnTriggerExit2D(Collider2D other)
    {
        other.gameObject.TryGetComponent(out IPlayerController player);
        if (player == _player)
        {
            if (coroutine != null) StopCoroutine(coroutine);
            _player = null;
        }
    } // end OnTriggerExit2D

    IEnumerator TryChangeDust()
    {
        if (_player == null) yield break; // Safety check
        while (true)
        {
            _amountOfDust += _dustTickAmount;
            if (Mathf.Abs(_amountOfDust) > _maxDustToExchange && _maxDustToExchange != -1)
            {
                break;
            }
            else
            {
                _player?.ChangeDust(_dustTickAmount, true);
                UpdateSprite();
                if ((Mathf.Abs(_amountOfDust) == _maxDustToExchange && _maxDustToExchange != -1) || _dustTickRate == -1)
                {
                    break;
                }
                yield return new WaitForSeconds(_dustTickRate);
            }
        }
        Disable();
        yield break;
    } // end TryChangeDust

    public void Disable()
    {
        SetActive(false);
        StartCoroutine(EnableAfterCooldown());
    } // end Disable

    IEnumerator EnableAfterCooldown()
    {
        yield return new WaitForSeconds(_cooldownTime);
        SetActive(true);
        yield break;
    } // end EnableAfterCooldown

    public void SetActive(bool active)
    {
        _amountOfDust = 0;
        _collider.enabled = active;
        if (_spriteRenderer != null) _spriteRenderer.enabled = active;
        UpdateSprite();
    } // end SetActive

    private void UpdateSprite()
    {
        if (_spriteRenderer == null) return;
        if (_maxDustToExchange == -1)
        {
            _spriteRenderer.sprite = _large;
        }
        else
        {
            float percentage = Mathf.Abs(_amountOfDust) / _maxDustToExchange * 100;
            if (percentage < 33)
            {
                _spriteRenderer.sprite = _large;
            }
            else if (percentage >= 33 && percentage < 66)
            {
                _spriteRenderer.sprite = _mid;
            }
            else
            {
                _spriteRenderer.sprite = _small;
            }
        }
    } // end SetAnimation
} // end class DustRemoval