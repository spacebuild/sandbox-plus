﻿namespace Sandbox;

public sealed partial class PlayerController : Component
{
	[Property, FeatureEnabled( "Input", Icon = "sports_esports" )] public bool UseInputControls { get; set; } = true;
	[Property, Feature( "Input" )] public float WalkSpeed { get; set; } = 110;
	[Property, Feature( "Input" )] public float RunSpeed { get; set; } = 320;
	[Property, Feature( "Input" )] public float DuckedSpeed { get; set; } = 70;
	[Property, Feature( "Input" )] public float JumpSpeed { get; set; } = 300;
	[Property, Feature( "Input" )] public float DuckedHeight { get; set; } = 36;
	
	[Property, Feature( "Input" )] public Angles LookInput { get; set; }

	/// <summary>
	/// Allows to player to interact with things by "use"ing them. 
	/// Usually by pressing the "use" button.
	/// </summary>
	[Property, Feature( "Input" ), ToggleGroup( "EnablePressing", Label = "Enable Pressing" )] public bool EnablePressing { get; set; } = true;

	/// <summary>
	/// The button that the player will press to use things
	/// </summary>
	[Property, Feature( "Input" ), Group( "EnablePressing" ), InputAction] public string UseButton { get; set; } = "use";

	/// <summary>
	/// How far from the eye can the player reach to use things
	/// </summary>
	[Property, Feature( "Input" ), Group( "EnablePressing" )] public float ReachLength { get; set; } = 130;

	TimeSince timeSinceJump = 0;

	void UpdateEyeAngles()
	{
		var input = Input.AnalogLook;

		IEvents.PostToGameObject( GameObject, x => x.OnEyeAngles( ref input ) );

		var ee = EyeAngles;
		ee += input;
		ee.roll = 0;
		ee.pitch = ee.pitch.Clamp( -89.9f, 89.9f );

		EyeAngles = ee;
	}

	void InputMove()
	{
		var lookInput = (LookInput + Input.AnalogLook).Normal;

		// Since we're a FPS game, let's clamp the player's pitch between -90, and 90.
		LookInput = lookInput.WithPitch( lookInput.pitch.Clamp( -90f, 90f ) );
		var rot = EyeAngles.ToRotation();
		WishVelocity = Mode.UpdateMove( rot, Input.AnalogMove );
	}

	void InputJump()
	{
		if ( TimeSinceGrounded > 0.33f ) return; // been off the ground for this many seconds, don't jump
		if ( !Input.Pressed( "Jump" ) ) return; // not pressing jump
		if ( timeSinceJump < 0.5f ) return; // don't jump too often
		if ( JumpSpeed <= 0 ) return;

		timeSinceJump = 0;
		Jump( GetUpDirection(JumpSpeed) );
		OnJumped();
	}

	[Rpc.Broadcast]
	public void OnJumped()
	{
		if ( UseAnimatorControls && Renderer.IsValid() )
		{
			Renderer.Set( "b_jump", true );
		}
	}


	float unduckedHeight = -1;
	Vector3 bodyDuckOffset = 0;

	/// <summary>
	/// Called during FixedUpdate when UseInputControls is enmabled. Will duck if requested.
	/// If not, and we're ducked, will unduck if there is room
	/// </summary>
	public void UpdateDucking( bool wantsDuck )
	{
		if ( wantsDuck == IsDucking ) return;

		unduckedHeight = MathF.Max( unduckedHeight, BodyHeight );
		var unduckDelta = unduckedHeight - DuckedHeight;

		// Can we unduck?
		if ( !wantsDuck )
		{
			if ( !IsOnGround )
				return;

			if ( Headroom < unduckDelta )
				return;
		}

		IsDucking = wantsDuck;

		if ( wantsDuck )
		{
			BodyHeight = DuckedHeight;

			// if we're not on the ground, keep out head in the same position
			if ( !IsOnGround )
			{
				WorldPosition += GetUpDirection(unduckDelta);
				Transform.ClearInterpolation();
				bodyDuckOffset = GetUpDirection(-unduckDelta);
			}
		}
		else
		{
			BodyHeight = unduckedHeight;
		}
	}
}
