using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_spacemovement : MonoBehaviour {

    // Assorted control variables. These mostly handle realism settings, change as you see fit.
    public float accelerateConst = 5;               // Set these close to 0 to smooth out acceleration. Don't set it TO zero or you will have a division by zero error.
    public float decelerateConst = 0.065f;           // I found this value gives semi-realistic deceleration, change as you see fit.

    /*	The ratio of MaxSpeed to Speed Const determines your true max speed. The formula is maxSpeed/SpeedConst = True Speed. 
		This way you wont have to scale your objects to make them seem like they are going fast or slow.
		MaxSpeed is what you will want to use for a GUI though.
	*/
    static public float maxSpeed = 100.0f;
    public float speedConst = 0;

    public int throttleConst = 50;
    public float raiseFlapRate = 1.0f;                      // Smoother when close to zero
    public float lowerFlapRate = 1.0f;						// Smoother when close to zero
	public float maxAfterburner = 5.0f;             // The maximum thrust your afterburner will produce
    public float afterburnerAccelerate = 0.5f;
	public float afterburnerDecelerate = 1.0f;
	public float liftConst = 7.5f; 					// Another arbitrary constant, change it as you see fit.
	public float angleOfAttack = 15.0f;			// Effective range: 0 <= angleOfAttack <= 20
	public float gravityConst = 9.8f;             // An arbitrary gravity constant, there is no particular reason it has to be 9.8...
    public int levelFlightPercent = 25; 
	public float maxDiveForce = 0.1f;
	public float noseDiveConst = 0.01f; 
	public float minSmooth = 0.5f;
	public float maxSmooth = 500.0f; 
	public float maxControlSpeedPercent = 75.0f;		// When your speed is withen the range defined by these two variables, your ship's rotation sensitivity fluxuates.
	public float minControlSpeedPercent = 25.0f;        // If you reach the speed defined by either of these, your ship has reached it's max or min sensitivity.


    // Rotation Variables, change these to give the effect of flying anything from a cargo plane to a fighter jet.
    public bool lockRotation = false;   // If this is checked, it locks pitch roll and yaw constants to the var rotationConst.
    public int lockedRotationValue = 120;
	public int pitchConst = 100; 
    public int rollConst = 100; 
    public int yawConst = 100;

    // Airplane Aerodynamics - I strongly reccomend not touching these...
    private float nosePitch;
    private float trueSmooth;
	private float smoothRotation;
	private float truePitch;
	private float trueRoll;
	private float trueYaw;
	private float trueThrust;
    static public float trueDrag;

    // Misc. Variables
    static public float afterburnerConst;
    static public int altitude;


    // HUD and Heading Variables. Use these to create your insturments.
    static public int trueSpeed;
	static public int attitude; 
	static public int incidence;
	static public int bank;
	static public int heading;


    private Rigidbody m_Rigid = null;
    public Rigidbody Rigid {
        get {
            if (m_Rigid == null)
                m_Rigid = gameObject.GetComponent<Rigidbody>();
            return m_Rigid;
        } set {
            m_Rigid = value;
        }
    }

    private Transform m_Trans = null;
    public Transform Flyer {
        get {
            if (m_Trans == null)
                m_Trans = gameObject.GetComponent<Transform>();
            return m_Trans;
        } set {
            m_Trans = value;
        }
    }

    protected mt_EventHandler m_EventHandler = null;
    public mt_EventHandler EventHandler
    {
        get
        {
            if (m_EventHandler == null)
                m_EventHandler = transform.root.GetComponentInChildren<mt_EventHandler>();
            return m_EventHandler;
        }
    }

    // Use this for initialization
    void Start () {
        trueDrag = 0;
        afterburnerConst = 0;
        smoothRotation = minSmooth + 0.01f;
        if (lockRotation == true)
        {
            pitchConst = lockedRotationValue;
            rollConst = lockedRotationValue;
            yawConst = lockedRotationValue;
            Cursor.visible = false;
        }
    }
	
	// Update is called once per frame
	void Update () {


        // * * This section of code handles the plane's rotation.

        float pitch = -vp_Input.GetAxisRaw("Mouse Y") * pitchConst;
        float roll = vp_Input.GetAxisRaw("Roll") * rollConst;
        float yaw = -vp_Input.GetAxisRaw("Mouse X") * yawConst;


        pitch *= Time.deltaTime;
        roll *= -Time.deltaTime;
        yaw *= Time.deltaTime;

        // Smothing Rotations...	
        if ((smoothRotation > minSmooth) && (smoothRotation < maxSmooth))
        {
            smoothRotation = Mathf.Lerp(smoothRotation, trueThrust, (maxSpeed - (maxSpeed / minControlSpeedPercent)) * Time.deltaTime);
        }
        if (smoothRotation <= minSmooth)
        {
            smoothRotation = smoothRotation + 0.01f;
        }
        if ((smoothRotation >= maxSmooth) && (trueThrust < (maxSpeed * (minControlSpeedPercent / 100))))
        {
            smoothRotation = smoothRotation - 0.1f;
        }
        trueSmooth = Mathf.Lerp(trueSmooth, smoothRotation, 5 * Time.deltaTime);
        truePitch = Mathf.Lerp(truePitch, pitch, trueSmooth * Time.deltaTime);
        trueRoll = Mathf.Lerp(trueRoll, roll, trueSmooth * Time.deltaTime);
        trueYaw = Mathf.Lerp(trueYaw, yaw, trueSmooth * Time.deltaTime);




        // * * This next block handles the thrust and drag.
        var throttle = (((-(vp_Input.GetAxisRaw("Mouse X")) + 1) / 2) * 100);


        if (throttle / speedConst >= trueThrust)
        {
            trueThrust = Mathf.SmoothStep(trueThrust, throttle / speedConst, accelerateConst * Time.deltaTime);
        }
        if (throttle / speedConst < trueThrust)
        {
            trueThrust = Mathf.Lerp(trueThrust, throttle / speedConst, decelerateConst * Time.deltaTime);
        }

        Rigid.drag = liftConst * ((trueThrust) * (trueThrust));

        if (trueThrust <= (maxSpeed / levelFlightPercent))
        {

            nosePitch = Mathf.Lerp(nosePitch, maxDiveForce, noseDiveConst * Time.deltaTime);
        }
        else
        {

            nosePitch = Mathf.Lerp(nosePitch, 0, 2 * noseDiveConst * Time.deltaTime);
        }

        trueSpeed = Mathf.FloorToInt((trueThrust / 2) * maxSpeed);

        // ** Additional Input

        // Airbrake
        if (Input.GetButton("Airbrake"))
        {
            trueDrag = Mathf.Lerp(trueDrag, trueSpeed, raiseFlapRate * Time.deltaTime);

        }

        if ((!Input.GetButton("Airbrake")) && (trueDrag != 0))
        {
            trueDrag = Mathf.Lerp(trueDrag, 0, lowerFlapRate * Time.deltaTime);
        }


        // Afterburner
        if (Input.GetButton("Afterburner"))
        {
            afterburnerConst = Mathf.Lerp(afterburnerConst, maxAfterburner, afterburnerAccelerate * Time.deltaTime);
        }

        if ((!Input.GetButton("Afterburner")) && (afterburnerConst != 0))
        {
            afterburnerConst = Mathf.Lerp(afterburnerConst, 0, afterburnerDecelerate * Time.deltaTime);
        }


        // Adding nose dive when speed gets below a percent of your max speed	
        if (((trueSpeed - trueDrag) + afterburnerConst) <= (maxSpeed * levelFlightPercent / 100))
        {
            noseDiveConst = Mathf.Lerp(noseDiveConst, maxDiveForce, (((trueSpeed - trueDrag) + afterburnerConst) - (maxSpeed * levelFlightPercent / 100)) * 5 * Time.deltaTime);
            Flyer.Rotate(noseDiveConst, 0, 0, Space.World);
        }


        // Calculating Flight Mechanics. Used mostly for the HUD.
        attitude = Mathf.FloorToInt(-((Vector3.Angle(Vector3.up, Flyer.forward)) - 90));
        bank = Mathf.FloorToInt(((Vector3.Angle(Vector3.up, Flyer.up))));
        incidence = Mathf.FloorToInt(attitude + angleOfAttack);
        heading = Mathf.FloorToInt(Flyer.eulerAngles.y);

        altitude = Mathf.FloorToInt(Flyer.transform.position.y);
        
        //Debug.Log ((((trueSpeed - trueDrag) + afterburnerConst) - (maxSpeed * levelFlightPercent/100)));
    }	// End function Update( );

    void FixedUpdate()
    {
        if (trueThrust <= maxSpeed)
        {
            // Horizontal Force
            transform.Translate(0, 0, ((trueSpeed - trueDrag) / 100 + afterburnerConst));
        }

        Rigid.AddForce(0, (Rigid.drag - gravityConst), 0);
        transform.Rotate(truePitch, -trueYaw, trueRoll);

    }// End function FixedUpdateUpdate( )


    /// <summary>
    /// registers this component with the event handler (if any)
    /// </summary>
    protected virtual void OnEnable()
    {

        if (EventHandler != null)
            EventHandler.Register(this);

    }


    /// <summary>
    /// unregisters this component from the event handler (if any)
    /// </summary>
    protected virtual void OnDisable()
    {

        if (EventHandler != null)
            EventHandler.Unregister(this);

    }
}
