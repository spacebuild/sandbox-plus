using Sandbox.Gravitational.Components;

namespace Sandbox;

public sealed partial class PlayerController
{
	
	public Vector3 GetGravityDirection()
	{
		var gravity = GetComponent<GravitationalAwareComponent>();
		if ( gravity is { Enabled: true } )
		{
			return gravity.GravityDirection;
		}
		return Vector3.Down;
	}

	public Vector3 GetDownDirection() => GetGravityDirection();

	public Vector3 GetUpDirection()
	{
		return -GetGravityDirection();
	}
	
}
