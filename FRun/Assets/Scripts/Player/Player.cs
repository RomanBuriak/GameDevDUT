using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private int _maxHP;
    private int _currentHP;
    [SerializeField] private int _maxMP;
    private int _currentMP;

    [SerializeField] Slider _HPSlider;
    [SerializeField] Slider _MPSlider;


    void Start()
    {
        _currentHP = _maxHP;
        _currentMP = _maxMP;

        _HPSlider.maxValue = _maxHP;
        _HPSlider.value = _maxHP;
        _MPSlider.maxValue = _maxMP;
        _MPSlider.value = _maxMP;
    }

    public void ChangeHP(int value)
    {
        _currentHP += value;
        if(_currentHP > _maxHP)
        {
            _currentHP = _maxHP;
        }
        else if(_currentHP <= 0)
        {
            OnDeath();
        }
        Debug.Log("Value = " + value);
        _HPSlider.value = _currentHP;
        Debug.Log("CurrentHP = " + _currentHP);

    }

    public bool ChangeMP(int value)
    {
        Debug.Log("MP value = " + value);
        if (value < 0 && _currentMP < Mathf.Abs(value))
            return false;

        _currentMP += value;
        if (_currentMP > _maxMP)
            _currentMP = _maxMP;

        _MPSlider.value = _currentMP;
        Debug.Log("Current MP = " + _currentMP);
        return true;
    }

    public void OnDeath()
    {
        Destroy(gameObject);
    }
}
