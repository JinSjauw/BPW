using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageAble
{
    void TakeDamage(int damage);
    void SetHP(int setHP);
    void Die();
}
