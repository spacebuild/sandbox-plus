using Sandbox.Gravitational.Util;
using Sdt.Environment;

namespace Sandbox.Gravitational.Components
{
	public partial class GravitationalAwareComponent: Component, IScenePhysicsEvents 
	{

		[Sync]
		public NetList<GravitationalEnvironmentComponent> Environments { get; set; } = new();
		
		[Sync] 
		public GravitationalEnvironmentComponent CurrentEnvironmentComponent { get; protected set; }
    
		[Sync] 
		public Vector3 Gravity { get; protected set; }
    
		[Sync] 
		public Vector3 GravityDirection { get; protected set; }


		public bool CustomGravityEnabled { get; protected set; }
		

		public GravitationalAwareComponent() => CheckEnabled();

		public void DisableGravity()
		{
			DisableEngineGravity();
			this.CurrentEnvironmentComponent = null;
			this.Gravity = Vector3.Zero;
			this.GravityDirection = Vector3.Zero;
		}

		private void DisableEngineGravity()
		{
			var body = GetPhysicsBody();
			if ( body is { GravityEnabled: true } )
			{
				body.GravityScale = 0;
				body.GravityEnabled = false;
			}
		}

		private PhysicsBody GetPhysicsBody()
		{
			var rigidBody = GetComponent<Rigidbody>();
			if (rigidBody is { PhysicsBody.BodyType: PhysicsBodyType.Dynamic } )
			{
				return rigidBody.PhysicsBody;
			}

			return null;
		}
		
		private Player GetPlayer()
		{
			return GetComponent<Player>();
		}

		public void Add( GravitationalEnvironmentComponent environmentComponent )
		{
			Environments.Add( environmentComponent );
		}
		public void Remove( GravitationalEnvironmentComponent environmentComponent )
		{
			Environments.Remove( environmentComponent );
		}

		protected override void OnFixedUpdate()
		{
			CheckEnabled();
		}

		private void CheckEnabled()
		{
			Enabled = GravitationalEnvironmentComponent.Instances > 0;
			CustomGravityEnabled = Enabled;
		}

		public void PostPhysicsStep()
		{
			if ( GravitationalEnvironmentComponent.Instances == 0 )
			{
				return;
			}
			var active = GravitationalUtils.getActiveEnvironment( Environments, this );
			if ( active != null )
			{
				var body = GetPhysicsBody();
				var isPlayer = GetPlayer() != null;
				if ( body != null || isPlayer )
				{
					switch ( active.Type )
					{
						case GravityType.CustomSpherical:
							var sphericalDirection =
								GravitationalUtils.CalculateSphericalGravitationalDirection( this, active );
							ProcessCustomGravity( active, sphericalDirection );
							break;
						case GravityType.CustomDownwards:
							var downWardsDirection =
								GravitationalUtils.CalculateDownwardsGravitationalDirection( this, active );
							ProcessCustomGravity( active, downWardsDirection );
							break;
						case GravityType.EngineDefault:
						default:
							EnableGravity(active,  active.GravityScale );
							break;
					}
				}
			} else {
				DisableGravity();
			}
		}

		private void EnableGravity(GravitationalEnvironmentComponent targetEntity, float gravityScale )
		{
			CurrentEnvironmentComponent = targetEntity;
			Gravity = Scene.PhysicsWorld.Gravity * gravityScale;
			GravityDirection = Scene.PhysicsWorld.Gravity.Normal;
			var body = GetPhysicsBody();
			if ( body != null && (!body.GravityEnabled || Math.Abs(body.GravityScale - gravityScale) > 0.0001f) )
			{
				body.GravityEnabled = true;
				body.GravityScale = gravityScale;
			}
		}
		
		private void ProcessCustomGravity( GravitationalEnvironmentComponent targetEntity, Vector3 gravitationalDirection )
		{
			DisableEngineGravity();
			var gravityVelocity = GravitationalUtils.CalculateGravity( this, targetEntity, gravitationalDirection );
			CurrentEnvironmentComponent = targetEntity;
			Gravity = gravityVelocity;
			GravityDirection = gravitationalDirection;
			if ( GetPlayer() != null )
			{
				return;
			}

			var body = GetPhysicsBody();
			if ( body == null )
			{
				return;
			}

			var group = body.PhysicsGroup;
			// TODO stop gravity if "hit" floor
			// Let's assume it will always be "up"
			var trace = Scene.Trace.Ray( WorldPosition, WorldPosition + (gravitationalDirection * 2) )
				.WithAnyTags( "solid" )
				.IgnoreGameObject(GameObject)
				.Run();
			if ( !trace.Hit )
			{
				group.AddVelocity( gravityVelocity * Time.Delta );
			}
			else if (!body.Velocity.IsNearZeroLength)
			{
				group.Sleeping = true;
				group.Sleeping = false;
			}
		}

	}
}
