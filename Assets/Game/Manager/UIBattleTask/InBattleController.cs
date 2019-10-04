using System;
using UnityEngine;

public class InBattleController : MonoBehaviour
{
    public void OnJoyStickMove()
    {
        UsingJoystrick?.Invoke();
    }

    public void OnMainWeapon()
    {
        OnMainWeaponBtn?.Invoke();
    }

    public void UpMainWeapon()
    {
        UpMainWeaponBtn?.Invoke();
    }

    public void OnSubWeapon()
    {
        OnSubWeaponBtn?.Invoke();
    }

    public void UpSubWeapon()
    {
        UpSubWeaponBtn?.Invoke();
    }

    public void OnSwitchAudio()
    {
        OnSwitchAudioButton?.Invoke();
    }

    public event Action UsingJoystrick;
    public event Action OnMainWeaponBtn;
    public event Action OnSubWeaponBtn;
    public event Action UpMainWeaponBtn;
    public event Action UpSubWeaponBtn;
    public event Action OnSwitchAudioButton;
}