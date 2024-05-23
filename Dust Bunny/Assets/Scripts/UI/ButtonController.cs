using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpringCleaning.UI
{
    // This script is used to control the sound of buttons, it should eventually be added to every single button to avoid having to manually add the sound to each button.
    // It currently only works with buttons but should be expanded to work with other interactable UI elements.
    public class ButtonController : MonoBehaviour
    {
        private Button _button;
        private Toggle _toggle;

        [SerializeField] private SoundType _soundType = SoundType.Navigate; // [Positive, Negative, Navigate]

        private void Awake()
        {
            _button = GetComponent<Button>();
            _toggle = GetComponent<Toggle>();
        }

        private void OnEnable()
        {
            UpdateType(_soundType);
        }

        private void OnDisable()
        {
            ClearType();
        }

        public void UpdateType(SoundType soundType)
        {
            ClearType();

            _soundType = soundType;

            switch (_soundType)
            {
                case SoundType.Positive:
                    if (_button != null) _button.onClick.AddListener(PositiveSound);
                    if (_toggle != null) _toggle.onValueChanged.AddListener(PositiveSound);
                    break;
                case SoundType.Negative:
                    if (_button != null) _button.onClick.AddListener(NegativeSound);
                    if (_toggle != null) _toggle.onValueChanged.AddListener(NegativeSound);
                    break;
                case SoundType.Navigate:
                    if (_button != null) _button.onClick.AddListener(NavigateSound);
                    if (_toggle != null) _toggle.onValueChanged.AddListener(NavigateSound);
                    break;
                default:
                    break;
            }
        }

        private void ClearType()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(PositiveSound);
                _button.onClick.RemoveListener(NegativeSound);
                _button.onClick.RemoveListener(NavigateSound);
            }
            if (_toggle != null)
            {
                _toggle.onValueChanged.RemoveListener(PositiveSound);
                _toggle.onValueChanged.RemoveListener(NegativeSound);
                _toggle.onValueChanged.RemoveListener(NavigateSound);
            }
        }

        #region Simple Sound Methods
        private void PositiveSound()
        {
            UISFXManager.PlaySFX(UISFXManager.SFX.POSITIVE);
        }

        private void NegativeSound()
        {
            UISFXManager.PlaySFX(UISFXManager.SFX.NEGATIVE);
        }

        private void NavigateSound()
        {
            UISFXManager.PlaySFX(UISFXManager.SFX.NAVIGATE);
        }
        #endregion

        #region Bool Sound Methods
        private void PositiveSound(bool value)
        {
            UISFXManager.PlaySFX(UISFXManager.SFX.POSITIVE);
        }

        private void NegativeSound(bool value)
        {
            UISFXManager.PlaySFX(UISFXManager.SFX.NEGATIVE);
        }

        private void NavigateSound(bool value)
        {
            UISFXManager.PlaySFX(UISFXManager.SFX.NAVIGATE);
        }
        #endregion
    }
    public enum SoundType
    {
        Positive,
        Negative,
        Navigate
    }
}