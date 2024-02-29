using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public static event Action PlayerDie;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer($"Vehicles")) return; 
        
        PlayerDie?.Invoke();
    }
}
