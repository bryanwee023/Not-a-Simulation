using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField]
    private bool canDeflect;
    [SerializeField]
    protected float speed;

    protected int attack;
    protected bool deflected = false;
    

    public void SetAttack(int attack)
    {
        this.attack = attack;
    }

    public virtual void Deflect(Vector3 direction, float multiplier)
    {
        if (canDeflect)
        {
            this.deflected = true;
            this.transform.rotation = Quaternion.LookRotation(direction);
            this.attack = (int)(this.attack * multiplier);
            this.speed *= 2;
        }
    }
}
