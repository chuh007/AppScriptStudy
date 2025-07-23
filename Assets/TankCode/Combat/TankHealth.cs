using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace TankCode.Combat
{
    public class TankHealth : NetworkBehaviour
    {
        //이건 반드시 초기화가 Awake나 여기에 있어야해
        public NetworkVariable<int> currentHealth = new NetworkVariable<int>(); 
        public int maxHealth = 100;

        public UnityEvent OnDead;
        public UnityEvent<int, int> OnHealthChange;
        
        [field: SerializeField] public bool IsDead { get; private set; }

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                currentHealth.OnValueChanged += HandleHealthChange;
                HandleHealthChange(0, maxHealth); //최초에는 한번 실행해줘야 된다. 안그러면 기본값인 채로
            }
            
            if (!IsServer) return; //아래의 코드는 서버만 실행한다.
            currentHealth.Value = maxHealth; //네트워크 변수는 서버만 그 값을 변경하는 것이 가능하다.
        }

        public override void OnNetworkDespawn()
        {
            if (IsClient)
            {
                currentHealth.OnValueChanged -= HandleHealthChange;
            }
        }
        
        private void HandleHealthChange(int previousValue, int newValue) 
            => OnHealthChange?.Invoke(newValue, maxHealth);


        public void TakeDamage(int amount) => ModifyHealth(-amount);
        public void RestoreHealth(int amount) => ModifyHealth(amount);
        
        //이건 오직 서버만 실행할 수 있어야 한다. 다른게 실행하면 안된다.
        private void ModifyHealth(int value)
        {
            if (IsDead) return;

            currentHealth.Value = Mathf.Clamp(currentHealth.Value + value, 0, maxHealth);
            if (currentHealth.Value == 0)
            {
                OnDead?.Invoke();
                IsDead = true;
            }
        }
    }
}