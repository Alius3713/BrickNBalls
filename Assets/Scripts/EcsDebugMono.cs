using Brick_n_Balls.ECS.Components;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class EcsDebugMono : MonoBehaviour
{
    [SerializeField] private InputActionReference debugAction; // przypniesz w inspectorze

    private void OnEnable()
    {
        debugAction.action.performed += OnDebugPressed;
        debugAction.action.Enable();
    }

    private void OnDisable()
    {
        debugAction.action.performed -= OnDebugPressed;
        debugAction.action.Disable();
    }

    private void OnDebugPressed(InputAction.CallbackContext ctx)
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null)
        {
            Debug.Log("[ECS DEBUG] World == null");
            return;
        }

        var em = world.EntityManager;

        int balls = em.CreateEntityQuery(typeof(BallTag)).CalculateEntityCount();
        int bricks = em.CreateEntityQuery(typeof(BrickTag)).CalculateEntityCount();
        int hitBricks = em.CreateEntityQuery(typeof(BrickHitFlag)).CalculateEntityCount();
        int destroyBalls = em.CreateEntityQuery(typeof(BallDestroyFlag)).CalculateEntityCount();

        Debug.Log($"[ECS DEBUG] Balls:{balls} Bricks:{bricks} BrickHitFlag:{hitBricks} BallDestroyFlag:{destroyBalls}");
    }
}
