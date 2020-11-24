using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFrogController : EnemyControllerBase
{
    protected override void ChangeState(EnemyState state)
    {
        base.ChangeState(state);
        switch(_currentState)
        {
            case EnemyState.Idle:
                _enemyRb.velocity = Vector2.zero;
                break;
            case EnemyState.Move:
                _startPoint = transform.position;
                break;
        }
    }
}
