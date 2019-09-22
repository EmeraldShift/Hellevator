using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public class TennisBall : Bullet
{
    private bool _homing;
    
    public void Initialize(GameManager gm, float angle, int numBounces, float speed, bool homing)
    {
        base.Initialize(gm, angle, numBounces, speed);
        _homing = _homing;
    }

    public void FixedUpdate()
    {
        
    }
}
