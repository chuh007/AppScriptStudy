using System;
using TankCode.Combat;
using Unity.Netcode;
using UnityEngine;

namespace TankCode.Players
{
    public class CoinCollector : NetworkBehaviour
    {
        public NetworkVariable<int> totalCoins = new NetworkVariable<int>();

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Coin coin))
            {
                int amout = coin.Collect();
                
                if(!IsServer) return;
                totalCoins.Value += amout;
            }
        }
    }
}