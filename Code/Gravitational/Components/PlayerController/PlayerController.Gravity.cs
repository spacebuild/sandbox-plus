using Sandbox.Gravitational.Components;

namespace Sandbox;

public sealed partial class PlayerController
{
	
	public Vector3 GetGravityDirection(float multiplier = 1)
	{
		var gravity = GetComponent<GravitationalAwareComponent>();
		if ( gravity is { CustomGravityEnabled: true } )
		{
			return gravity.GravityDirection * multiplier;
		}
		return Vector3.Down * multiplier;
	}

	public Vector3 GetDownDirection(float multiplier = 1) => GetGravityDirection(multiplier);

	public Vector3 GetUpDirection(float multiplier = 1)
	{
		return -GetGravityDirection(multiplier);
	}
	
}
