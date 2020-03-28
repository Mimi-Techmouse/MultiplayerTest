/////////////////////////////////////////////////////////////////////////////////
//
//	vp_PlayerEventHandler.cs
//	© Opsive. All Rights Reserved.
//	https://twitter.com/Opsive
//	http://www.opsive.com
//
//	description:	this class declares events for communication between behaviours
//					that make up a basic player object. it also binds several object
//					component states to player activity events
//
///////////////////////////////////////////////////////////////////////////////// 

using System;
using UnityEngine;

public class mt_EventHandler : vp_StateEventHandler {

    // health
    public vp_Value<float> Health;

    // position and rotation
    public vp_Value<Vector3> Position;      // Plane Position
    public vp_Value<Vector3> Rotation;      // Plane Rotation

    // rotations
    public vp_Value<float> Pitch;
    public vp_Value<float> Roll;
    public vp_Value<float> Yaw;

    // activities
    public vp_Activity Dead;
    public vp_Activity MainAttack;
    public vp_Activity Fly;
    public vp_Activity Interact;
    public vp_Activity Zoom;

    // interaction
    public vp_Value<vp_Interactable> Interactable;
    public vp_Value<bool> CanInteract;
    public vp_Message<float> DamageMe;


    /// <summary>
    /// 
    /// </summary>
    protected override void Awake()
    {

        base.Awake();
        
        BindStateToActivity(Dead);
        BindStateToActivity(MainAttack);
        BindStateToActivity(Fly);
        BindStateToActivity(Interact);
        BindStateToActivity(Zoom);

    }


}