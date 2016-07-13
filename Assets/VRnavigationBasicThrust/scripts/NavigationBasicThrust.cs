using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SteamVR_TrackedObject))]
public class NavigationBasicThrust : MonoBehaviour
{
    public Rigidbody NaviBase;
    public Vector3 ThrustDirection;
    public float ThrustForce;
    public bool ShowTrustMockup = true;
    public GameObject ThrustMockup;
    public float trailgap = 2;

    SteamVR_TrackedObject trackedObj;
    FixedJoint joint;
    GameObject attachedObject;
    Vector3 tempVector;
    Vector3 oldLocation;
    Vector3 originalScale;
    float oldTriggerTravel;
    float distance;

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        oldLocation = NaviBase.position;
    }

    void FixedUpdate()
    {
        var device = SteamVR_Controller.Input((int)trackedObj.index);

        var triggerAxis = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);

        float triggerTravel = triggerAxis.x;
        var triggerPressed = triggerTravel > 0.01;

        var delta = Vector3.Distance(oldLocation, NaviBase.position);
        oldLocation = NaviBase.position;

        distance += delta;

        //if (oldTriggerTravel != triggerTravel)
        //{
        //    Debug.logger.Log("triggerTravel=" + triggerTravel);
        //    oldTriggerTravel = triggerTravel;
        //    //Debug.Log("NaviBase=" + NaviBase.position);
        //    Debug.logger.Log("distance=" + distance);
        //}


        if (distance > trailgap)
        {
            var trailSegment = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            trailSegment.transform.position = NaviBase.position;
            // trailSegment.transform.rotation = NaviBase.rotation;
            //var scaleFactor = 0.2f + (1f-triggerTravel) * 4f ;
            var scaleFactor = 0.4f + 1.6f / (NaviBase.velocity.magnitude / 4 + 1);

            trailSegment.transform.localScale *= scaleFactor;

            //trailSegment.transform.localScale = trailSegment.transform.localScale * NaviBase.velocity;
            distance = 0f;
        }

        // add force
        if (triggerPressed)
        {
            tempVector = Quaternion.Euler(ThrustDirection) * Vector3.forward;
            NaviBase.AddForce(transform.rotation * tempVector * ThrustForce * triggerTravel);
            NaviBase.maxAngularVelocity = 2f;

            if (attachedObject != null)
            {
                var scaleFactor = 1f + triggerTravel * 10f;
                attachedObject.transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z * scaleFactor);
            }
        }
        // show trust mockup
        if (ShowTrustMockup && ThrustMockup != null)
        {
            if (attachedObject == null && triggerPressed)
            {
                attachedObject = (GameObject)Instantiate(ThrustMockup, Vector3.zero, Quaternion.identity);
                attachedObject.transform.SetParent(transform, false);
                attachedObject.transform.Rotate(ThrustDirection);
                originalScale = attachedObject.transform.localScale;
            }
            else if (attachedObject != null && !triggerPressed)
            {
                Destroy(attachedObject);
            }
        }
    }
}
