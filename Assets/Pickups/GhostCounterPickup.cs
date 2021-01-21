using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCounterPickup : PickupActor
{
    [SerializeField] float amount = 0.5f;
    
    protected override void OnSpawn()
    {
        Debug.Log("Spawning Ghost Counter Pickup");
    }

    protected override void OnPickup()
    {
        player.currentGhostTimer += player.baseGhostTimer * amount;
        var ps = Instantiate(onPickupPS, transform.position, Quaternion.identity);
        Destroy(ps, 2.0f);
    }
}
