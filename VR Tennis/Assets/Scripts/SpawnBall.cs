using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnBall : MonoBehaviour
{
    public InputActionReference SpawnReference = null;
    public Rigidbody BallPrefab;
    public Transform Spawnpoint;

    private void Awake()
    {
        SpawnReference.action.started += SpawnTennisBall;
        for (int i = 0; i < 10; i++)
        {
            Instantiate(BallPrefab, Spawnpoint, true);

        }
    }

    private void OnDestroy()
    {
        SpawnReference.action.started -= SpawnTennisBall;
    }

    void SpawnTennisBall(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Rigidbody ball = Instantiate(BallPrefab, Spawnpoint, true);
        }


    }
}
