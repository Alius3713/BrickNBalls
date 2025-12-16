using Brick_n_Balls.Core;
using Brick_n_Balls.ECS.Components;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Brick_n_Balls.Gameplay
{
    public class BrickRegistry : MonoBehaviour
    {
        [SerializeField] private int _defaultHp = 3;
        [SerializeField] private float _hitCooldown = 0.06f;

        private readonly Dictionary<Entity, int> _hp = new();
        private readonly HashSet<Entity> _bricks = new();
        private readonly Dictionary<Entity, float> _lastHitTime = new();

        public void Register(Entity brickEntity, int? hpOverride = null)
        {
            _lastHitTime[brickEntity] = -999f;
            if (brickEntity == Entity.Null) return;
            _bricks.Add(brickEntity);
            _hp[brickEntity] = Mathf.Max(1, hpOverride ?? _defaultHp);
        }

        public void Unregister(Entity brickEntity)
        {
            if (brickEntity == Entity.Null) return;
            _hp.Remove(brickEntity);
            _lastHitTime.Remove(brickEntity);
            _bricks.Remove(brickEntity);
        }

        public void ClearAll(EntityManager em)
        {
            foreach (var entity in _bricks)
            {
                if (entity != Entity.Null && em.Exists(entity))
                {
                    em.DestroyEntity(entity);
                }
            }
            _bricks.Clear();
            _hp.Clear();
            _lastHitTime.Clear();
        }

        public void ApplyHit(Entity brickEntity, EntityManager em)
        {
            if (brickEntity == Entity.Null) return;

            if (!em.Exists(brickEntity))
            {
                _hp.Remove(brickEntity);
                _lastHitTime.Remove(brickEntity);
                return;
            }

            if (!_hp.TryGetValue(brickEntity, out int hp))
            {
                _lastHitTime.Remove(brickEntity);
                return;
            }
            
            float now = Time.time;
            if (_lastHitTime.TryGetValue(brickEntity, out float last) && (now - last) < _hitCooldown) return;
            _lastHitTime[brickEntity] = now;

            hp -= 1;
            _hp[brickEntity] = hp;

            ScoreManager.Instance?.AddScore(1);

            if (hp <= 0)
            {
                em.DestroyEntity(brickEntity);
                _hp.Remove(brickEntity);
                _lastHitTime.Remove(brickEntity);
            }
        }
    }
}
