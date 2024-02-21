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
        _player = other.gameObject.GetComponent<PlayerController>();
        if (_player)
        {
            InvokeRepeating("tryAddDust", 0f, _dustTickRate);
            if (!_player.MaxedOutDust()){
                _player.GetSFX().PlaySFX(PlayerSFXController.SFX.Dust_Collect_Start);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player == _player)
        {
            CancelInvoke("tryAddDust");
            if (!_player.MaxedOutDust()){
                _player.GetSFX().PlaySFX(PlayerSFXController.SFX.Dust_Collect_Stop_Abrupt);
            }
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

            //Check if this dust add has now filled up the player, to stop the sfx
            if(_player.MaxedOutDust()){
                _player.GetSFX().PlaySFX(PlayerSFXController.SFX.Dust_Collect_Stop_Clean);
            }
        }
    }

}
