using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeItem : Item
{
    public override void Pick(MarioPlayerController Player)
    {
        if (Player.GetLife() < 1.0f)
        {
            Player.AddLife();
            gameObject.SetActive(false);
        }
    }
}
