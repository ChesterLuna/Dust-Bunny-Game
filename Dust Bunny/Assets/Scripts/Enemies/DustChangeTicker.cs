using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DustChangeTicker : MonoBehaviour
{
    [Tooltip("The maximum amount of dust this object can give/take before it is destroyed. Set to -1 to make it infinite.")]
    [SerializeField] float _maxDustToExchange = 100;
    [SerializeField] float _dustTickAmount = 10;
    float _amountOfDust = 0;
    IPlayerController _player;
    [SerializeField] float _dustTickRate = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        other.gameObject.TryGetComponent(out _player);
        if (_player != null)
        {
            InvokeRepeating(nameof(TryRemoveDust), 0f, _dustTickRate);
        }
    } // end OnTriggerEnter2D

    private void OnTriggerExit2D(Collider2D other)
    {
        other.gameObject.TryGetComponent(out IPlayerController player);
        if (player == _player)
        {
            CancelInvoke(nameof(TryRemoveDust));
            _player = null;
        }
    } // end OnTriggerExit2D

    void TryRemoveDust()
    {
        if (_player == null) return; // Safety check
        _amountOfDust += _dustTickAmount;
        if (_amountOfDust > _maxDustToExchange && _maxDustToExchange != -1)
        {
            gameObject.SetActive(false);
        }
        _player?.ChangeDust(_dustTickAmount);
    } // end TryRemoveDust
} // end class DustRemoval