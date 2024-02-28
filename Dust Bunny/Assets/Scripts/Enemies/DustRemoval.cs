using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DustRemoval : MonoBehaviour
{
    [Tooltip("The maximum amount of dust this object can take before it is destroyed. Set to -1 to make it indestructible")]
    [SerializeField] float _maxDustToTake = 100;
    [SerializeField] float _dustTickAmount = 10;
    float _amountOfDust = 0;
    PlayerController _player;
    [SerializeField] float _dustTickRate = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        _player = other.gameObject.GetComponent<PlayerController>();
        if (_player)
        {
            InvokeRepeating("tryRemoveDust", 0f, _dustTickRate);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player == _player)
        {
            _player = null;
            CancelInvoke("tryRemoveDust");
            _player = null;
        }
    }

    void tryRemoveDust()
    {
        if (_player == null) return; // Safety check
        _amountOfDust += _dustTickAmount;
        if (_amountOfDust > _maxDustToTake && _maxDustToTake != -1)
        {
            Destroy(gameObject);
        }
        _player?.RemoveDust(_dustTickAmount);

    }


}
