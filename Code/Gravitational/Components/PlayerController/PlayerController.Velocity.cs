using System;
using System.Numerics;
using Sandbox;

namespace Sandbox;

public partial class PlayerController
{
	
	private Transform LocalTransform { get; set; }
	
	public Vector3 MoveDir { get; set; } = Vector3.Zero;

	public Vector3 WorldToLocalVelocity( Vector3 worldVelocity )
	{
		return LocalTransform.NormalToLocal( worldVelocity );
	}

	public Vector3 LocalToWorldVelocity(Vector3 localVelocity)
	{
		return LocalTransform.NormalToWorld(localVelocity);
	}

	private void ProcessVelocity()
	{
		LocalTransform = new Transform( WorldPosition, WorldRotation );
	}
	
}
