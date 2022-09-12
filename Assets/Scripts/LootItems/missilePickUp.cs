using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missilePickUp : LootItem
{
    protected override void PickUp()
    {
        player.PickUpMissile();
        base.PickUp();
    }


}
