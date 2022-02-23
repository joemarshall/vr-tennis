using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShooter : MonoBehaviour
{

    public bool runBallMachine = true;

    [SerializeField] private Rigidbody ball;
    [SerializeField] private float ballForce;
    [SerializeField] private float upForce;
    [SerializeField] private Transform aimTarget;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShootBalls());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator ShootBalls()
    {
        WaitForSeconds wait = new WaitForSeconds(5);

        while (runBallMachine)
        {
  //          Debug.Log("shoot ball");

            GameObject[] balls = GameObject.FindGameObjectsWithTag("Shoot Ball");


/*            foreach (GameObject b in balls)
            {
                GameObject.Destroy(b);
            }*/

            Rigidbody spawnBall = Instantiate(ball, transform.position, transform.rotation);
            spawnBall.tag = "Shoot Ball";

            Vector3 dir = (aimTarget.position - transform.position); // get the direction to where we want to send the ball
//            Debug.Log("Ball direction " + dir + " normalized = " + dir.normalized);
            spawnBall.velocity = dir.normalized * ballForce + new Vector3(0, upForce, 0);

            yield return wait;
        }
    }
}
