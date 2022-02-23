using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR;
 
public class Racket : MonoBehaviour
{
    private InputDevice device;

    private GameObject racketFace;
    


    // technique inspired from https://github.com/sinoriani/Unity-Projects/blob/master/Tennis%20Game/Player.cs
    [SerializeField] private Transform aimTarget;
    [SerializeField] private float ballForce;

    private Vector3 currentPosition;
    private Vector3 lastPosition;
    private Matrix4x4 []transform_history=new Matrix4x4[10];// store last 10 local to world matrices for calculating velocity at a particular hit point
    private InputFeatureUsage<Vector3> deviceAccelerationUsage;

    private XRNodeState deviceState;

    private float timeSinceTrigger=0;

    // Start is called before the first frame update
    void Start()
    {
        InputTracking.nodeAdded+=nodeAdded;
        lastPosition = transform.position;
        getDevice();
        racketFace = Helper.GetChildWithName(gameObject, "RacketFace");        
    }


    void nodeAdded(XRNodeState node)
    {
        Debug.Log(node.nodeType);
        if(node.nodeType==XRNode.RightHand)
        {
            deviceState=node;
        }
    }
    void getDevice()
    {
        if( !device.isValid)
        {
            List<InputDevice> devices=new List<InputDevice>();
            InputDevices.GetDevicesAtXRNode(XRNode.RightHand, devices);
            if(devices.Count>0)
            {
                device = devices[0];
                deviceAccelerationUsage = CommonUsages.deviceAcceleration;
                Debug.Log("Get new device"+device);
            }else
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
        // shift along rotation history
        System.Array.Copy(transform_history,0,transform_history,1,transform_history.Length-1);
        // add in new position history
        transform_history[0]=racketFace.transform.localToWorldMatrix;
        timeSinceTrigger+=Time.fixedDeltaTime;
    }

    void OnTriggerEnter(Collider col)
    {        
        if(timeSinceTrigger<0.1)
        {
            return;
        }
        timeSinceTrigger=0;
        getDevice();
        //This method will run when your game object
        Debug.Log("Racket collided with " + col.gameObject.name);
        if (col.gameObject.name.Contains("Ball"))
        {
            Rigidbody ball = col.gameObject.GetComponent<Rigidbody>();
            Vector3 ballPos = ball.position;
            GameObject racketFace = Helper.GetChildWithName(gameObject, "RacketFace");
            Vector3 racketPos = racketFace.transform.position;
            // where on the racket it was hit
            Vector3 localHitPos=racketFace.transform.InverseTransformPoint(ballPos);
            // world position of the hit point 10 frames back
            Vector3 previousWorldHitPos=transform_history[transform_history.Length-1].MultiplyPoint(localHitPos);

            Vector3 velocity= (ballPos-previousWorldHitPos)/(Time.fixedDeltaTime*10.0f);
            print("Hit velocity:"+velocity+"!!!"+(previousWorldHitPos-ballPos));

            // do the bounce based on how fast the ball is coming towards the racket
            // it should bounce back at this but flipped in the
            //  racket  x axis
            // TODO: You need to fix this so that it also calculates the rotational velocity of the racket head
            // and then uses that to estimate the speed of motion of the racket at the hit point
            Vector3 incomingVelocity=ball.velocity - velocity;
            Vector3 localIncomingVelocity=transform.InverseTransformVector(incomingVelocity);
            Vector3 localOutgoingVelocity=new Vector3(-localIncomingVelocity.x,localIncomingVelocity.y,localIncomingVelocity.z);
            Vector3 outgoingVelocity=transform.TransformVector(localOutgoingVelocity)+velocity;
            Debug.Log("Ball collided!"+velocity+":"+incomingVelocity+":"+localIncomingVelocity+"!"+localOutgoingVelocity+"#"+outgoingVelocity);

            ball.velocity = outgoingVelocity;
            
            // = dir.normalized * ballForce + new Vector3(0, 10, 0);

            // ball.AddForce(Vector3.Reflect(ball.velocity, racketPos), ForceMode.Impulse);
        }
    }
}
