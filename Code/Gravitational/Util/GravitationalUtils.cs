using Sandbox.Gravitational.Components;

namespace Sandbox.Gravitational.Util;

public static class GravitationalUtils
{
	
	private const float Gravity = 800; // TODO get it out of settings! (is a vector!)
	
	public static GravitationalEnvironmentComponent getActiveEnvironment( IList<GravitationalEnvironmentComponent> environments, GravitationalAwareComponent target )
	{
		//TODO what are the rules?
		return environments.Count > 0 ? environments[0] : null;
	}
	
	public static Vector3 CalculateSphericalGravitationalDirection( GravitationalAwareComponent component, GravitationalEnvironmentComponent targetEntity )
	{
		var direction = (component.Transform.Position - targetEntity.WorldPosition).Normal; // Or center of mass?
		return targetEntity.Inverse ? direction : -direction;
	}
	
	public static Vector3 CalculateDownwardsGravitationalDirection( GravitationalAwareComponent component, GravitationalEnvironmentComponent targetEntity )
	{
on:		var rotation = targetEntity.WorldRotation;
		return targetEntity.Inverse ? rotation.Up : rotation.Down;
	}
	
	public static Vector3 CalculateGravity( GravitationalAwareComponent component, GravitationalEnvironmentComponent targetEntity, Vector3 gravitationalDirection )
	{
		var speed = CalculateGravityPull( component, targetEntity );
		return gravitationalDirection * speed;
	}
	
	private static float CalculateGravityPull(GravitationalAwareComponent component, GravitationalEnvironmentComponent targetEntity)
	{
		return Gravity * targetEntity.GravityScale;
	}
	
}
