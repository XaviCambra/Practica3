using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeItem : Item
{
    public override void Pick(MarioPlayerController Player)
    {
        if (Player.GetLife() < 8.0f)
        {
            Player.AddLife();
            gameObject.SetActive(false);
        }
    }
}
