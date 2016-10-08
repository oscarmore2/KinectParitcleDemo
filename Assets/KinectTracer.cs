using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Kinect = Windows.Kinect;

//[RequireComponent(typeof(PositionQueue))]
public class KinectTracer : MonoBehaviour {
	/*

    public MatOfPoint2f ApproxCurve;

    PositionQueue queue;

    public GameObject BodySourceManager;
    private BodySourceManager _BodyManager;


    public bool ContinueTracing = false;

    // Use this for initialization
    void Start()
    {
        queue = GetComponent<PositionQueue>();
    }

    // Update is called once per frame
    void Update()
    {
        if (BodySourceManager == null)
        {
            return;
        }

        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }

        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }
        bool isTracked = false;

        Kinect.Body activeBody = null;
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }
            if(body.IsTracked)
                activeBody = body;
        }
        if (activeBody != null )
        {
            Kinect.Joint HandRight = activeBody.Joints[Kinect.JointType.HandTipRight];
            Vector3 rightHandPos = GetVector3FromJoint(HandRight);
            RecordPosition(rightHandPos);
            isTracked = true;
        }
        if (!isTracked && !Input.GetMouseButton(0))
        {
            queue.Clear();
        }


    }
    private void RecordPosition(Vector3 pos)
    {
        Vector3 posOnScreen = Camera.main.WorldToScreenPoint(pos);

        queue.PushPosition(posOnScreen);
    }
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
    */
}
