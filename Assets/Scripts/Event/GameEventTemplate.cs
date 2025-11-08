using ReusableScripts.Core;
using ReusableScripts.Interface;
using UnityEngine;

namespace ReusableScripts.Event
{
    public class GameEventTemplate : IGameEvent
    {
        public float DamageAmount { get; set; }
        public float CurrentHealth { get; set; }
        public float MaxHealth { get; set; }
        public Vector3 HitPosition { get; set; }

        public GameEventTemplate(float damageAmount, float currentHealth, float maxHealth, Vector3 hitPosition)
        {
            DamageAmount = damageAmount;
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
            HitPosition = hitPosition;
        }
        
        // Paste this to a monobehavior script
        // EventBus.Instance.Publish(new GameEventTemplate(
        //     damageAmount: 10f, 
        //     currentHealth: 20f, 
        //     maxHealth: 50f, 
        //     hitPosition: transform.position));
    }
}