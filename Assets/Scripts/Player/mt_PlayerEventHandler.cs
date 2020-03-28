using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_PlayerEventHandler : mt_EventHandler {

    //Camera
    public vp_Value<bool> IsFirstPerson;
    public vp_Value<Vector2> InputSmoothLook;
    public vp_Activity<Vector3> CameraEarthQuake;
    public vp_Value<Vector3> LookPoint;
    public vp_Value<Vector3> CameraEarthQuakeForce;
    public vp_Value<Vector3> CameraLookDirection;
    public vp_Value<Vector2> CameraRotation;

    //Multiplayer Support
	public vp_Value<bool> IsLocal;
    public vp_Value<float> BodyPitch;
    public vp_Value<float> BodyRoll;
    public vp_Value<float> BodyYaw;

    //For FPController / Physics
	public vp_Message<Vector3> Move;
	public vp_Value<Vector3> Velocity;
	public vp_Value<float> SlopeLimit;
	public vp_Value<float> StepOffset;
	public vp_Value<float> Radius;
	public vp_Value<float> Height;
	public vp_Value<float> FallSpeed;
	public vp_Message<float> FallImpact;
	public vp_Message<float> HeadImpact;
	public vp_Message<Vector3> ForceImpact;
	public vp_Message Stop;
	public vp_Value<Transform> Platform;
	public vp_Value<Vector3> PositionOnPlatform;
	public vp_Value<bool> Grounded;
    public vp_Value<Vector2> InputMoveVector;
    public vp_Activity Run; //For turbo'ing
    public vp_Activity Jump; //I think this just controls movement up
    public vp_Activity Crouch; //I think this just controls movement down
    public vp_Activity OutOfControl; //In case we want spinning later

    //Player Input things
	public vp_Value<bool> Pause; //Remove this later
	public vp_Message<string, bool> InputGetButton;
	public vp_Message<string, bool> InputGetButtonUp;
	public vp_Message<string, bool> InputGetButtonDown;

    /// <summary>
    /// Initialize the activities
    /// </summary>
    protected override void Awake()
    {

        base.Awake();

        BindStateToActivity(Run);
        BindStateToActivity(Jump);
        BindStateToActivity(Crouch);
        BindStateToActivity(OutOfControl);
        BindStateToActivity(CameraEarthQuake);

    }

}
