using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    public new string name;
    public float cooldownTime;
    public float activeTime;

    public virtual void Activate(GameObject holder) { }
    public virtual void Activate(GameObject holder, ObjectPool objectPool) { }

}
