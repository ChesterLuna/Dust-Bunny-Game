using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DustCloud : MonoBehaviour
{
    [SerializeField] float _maxDustToGive = 100;
    [SerializeField] float _dustTickAmount = 10;
    float _amountOfDust;
    PlayerController _player;
    [SerializeField] float _dustTickRate = 0.3f;


    void Start()
    {
        _amountOfDust = _maxDustToGive;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        _player = other.gameObject.GetComponent<PlayerController>();
        if (_player)
        {
            InvokeRepeating("tryAddDust", 0f, _dustTickRate);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player == _player)
        {
            CancelInvoke("tryAddDust");
        }
    }

    void tryAddDust()
    {
        if (!_player.MaxedOutDust())
        {
            _amountOfDust -= _dustTickAmount;
            if (_amountOfDust < 0 && _maxDustToGive != -1)
            {
                Destroy(gameObject);
            }
            _player.AddDust(_dustTickAmount);
        }
    }

}
