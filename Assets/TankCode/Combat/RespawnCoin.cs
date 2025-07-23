using System;
using UnityEngine;
using UnityEngine.Events;

namespace TankCode.Combat
{
    public class RespawnCoin : Coin
    {
        public UnityEvent<RespawnCoin> OnCollected;

        public Vector2 _prevPosition;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _prevPosition = transform.position;
        }

        public override int Collect()
        {
            if(isCollected) return 0;

            if (!IsServer)
            {
                SetVisible(false);
                return 0;
            }
            
            isCollected = true;
            OnCollected?.Invoke(this);
            isActive.Value = false;

            return coinValue;
        }

        public void ResetCoin()
        {
            isCollected = false;
            isActive.Value = true;
            SetVisible(true);
        }

        private void Update()
        {
            if(IsServer) return;
            if (Vector2.Distance(_prevPosition, transform.position) > 0.5f)
            {
                _prevPosition = transform.position;
                SetVisible(true);
            }
        }
    }
}