using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinItem : Item
{
    public override void Pick(MarioPlayerController Player)
    {
            Player.AddCoin();
            gameObject.SetActive(false);
    }
}
