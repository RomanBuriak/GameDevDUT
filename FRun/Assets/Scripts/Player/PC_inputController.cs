using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement_controller))]
public class PC_inputController : MonoBehaviour
{
    Movement_controller _playerMovement;

    float _move;//значення яке говорить вліво -1 чи вправо 1 рухається плеєр
    bool _jump;//змінна яка відповідає за стан стрибку
    bool _crawling;//чи повзає плеєр

    private void Start()
    {
        _playerMovement = GetComponent<Movement_controller>();
    }

    // Update is called once per frame (буде відпрацьовувати з кожним кадром системи) частота виклику залежить від швидкодії пристрою (30-60 раз за сек)
    void Update()
    {
        _move = Input.GetAxisRaw("Horizontal");// return -1 if(click (A || <-)) and return 1 if(click(D || ->)
        if (Input.GetButtonUp("Jump"))
        {
            _jump = true;
        }

        _crawling = Input.GetKey(KeyCode.C);//return true if press and false if !press

        if (Input.GetKey(KeyCode.E))
        {
            _playerMovement.StartShooting();
        }

        if (Input.GetButtonUp("Fire1"))
        {
            _playerMovement.StartStrike();
        }
    }

    private void FixedUpdate()
    {
        _playerMovement.Move(_move, _jump, _crawling);
        _jump = false;
    }
}
