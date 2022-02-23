using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR;

public class Racket2 : MonoBehaviour
{
    private InputDevice device;

    // technique inspired from https://github.com/sinoriani/Unity-Projects/blob/master/Tennis%20Game/Player.cs
    [SerializeField] private Transform aimTarget;
    [SerializeField] private float ballForce;

    private Vector3 currentPosition;
    private Vector3 lastPosition;
    private Vector3 velocity;
    private InputFeatureUsage<Vector3> deviceAccelerationUsage;

    private XRNodeState deviceState;
    private DrawHelper drawHelper;

    private float timeSinceTrigger = 0;

    // Start is called before the first frame update
    void Start()
    {
        InputTracking.nodeAdded += nodeAdded;
        lastPosition = transform.position;
        drawHelper = new DrawHelper(this.gameObject);
        getDevice();
    }


    void nodeAdded(XRNodeState node)
    {
        Debug.Log(node.nodeType);
        if (node.nodeType == XRNode.RightHand)
        {
            deviceState = node;
        }
    }
    void getDevice()
    {
        if (!device.isValid)
        {
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesAtXRNode(XRNode.RightHand, devices);
            if (devices.Count > 0)
            {
                device = devices[0];
                deviceAccelerationUsage = CommonUsages.deviceAcceleration;
                Debug.Log("Get new device" + device);
            }
            else
            {
                Debug.Log("No RH device yet");
            }

        }

    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        GameObject racketFace = Helper.GetChildWithName(gameObject, "RacketFace");
        currentPosition = racketFace.transform.position;
        Vector3 thisVel = (currentPosition - lastPosition) / Time.fixedDeltaTime;
        // smooth out the velocity estimation
        velocity = thisVel * 0.2f + velocity * 0.8f;
        lastPosition = currentPosition;
        timeSinceTrigger += Time.fixedDeltaTime;
        // Debug.Log(velocity);
    }

    void OnTriggerEnter(Collider col)
    {
        // if (timeSinceTrigger < 0.1)
        // {
        //     return;
        // }
        timeSinceTrigger = 0;
        getDevice();
        //This method will run when your game object
        Debug.Log("Racket collided with " + col.gameObject.name);
        if (col.gameObject.name.Contains("Ball"))
        {
            Rigidbody ball = col.gameObject.GetComponent<Rigidbody>();
            Vector3 ballPos = ball.position;
            GameObject racketFace = Helper.GetChildWithName(gameObject, "RacketFace");
            Vector3 racketPos = racketFace.transform.position;

            // do the bounce based on how fast the ball is coming towards the racket
            // it should bounce back at this but flipped in the
            //  racket x axis
            // TODO: You need to fix this so that it also calculates the rotational velocity of the racket head
            // and then uses that to estimate the speed of motion of the racket at the hit point
            Vector3 incomingVelocity = ball.velocity - velocity;
            Vector3 localIncomingVelocity = transform.InverseTransformVector(incomingVelocity);
            Vector3 localOutgoingVelocity = new Vector3(localIncomingVelocity.x, localIncomingVelocity.y, localIncomingVelocity.z);
            Vector3 outgoingVelocity = transform.TransformVector(localOutgoingVelocity) + velocity;
            Debug.Log("Ball collided!" + velocity + ":" + incomingVelocity + ":" + localIncomingVelocity + "!" + localOutgoingVelocity + "#" + outgoingVelocity);

            // ball.velocity = outgoingVelocity;

            DrawBallPath(transform.position, outgoingVelocity);

            // ball.velocity = outgoingVelocity * ballForce;
            Vector3 multiply = outgoingVelocity * ballForce;
            Debug.Log("Final vector: " + multiply);
            if (multiply.y > 400)
            {
                multiply.y *= 0.6f; // smooth any erratic y velocity
            }
            if (multiply.z > 800)
            {
                multiply.z *= 0.6f;
            }
            Debug.Log("Final vector2: " + multiply);
            OculusDebug.Instance.log("Final vector2: " + multiply);
            ball.AddForce(multiply, ForceMode.Acceleration);

            // = dir.normalized * ballForce + new Vector3(0, 10, 0);

            // ball.AddForce(Vector3.Reflect(ball.velocity, racketPos), ForceMode.Impulse);
        }
    }

    private IEnumerator DrawBallPath(Vector3 start, Vector3 end)
    {
        drawHelper.DrawLine(start, end);

        WaitForSeconds wait = new WaitForSeconds(3);

        yield return wait;

        // drawHelper.ClearLine();
    }
}
