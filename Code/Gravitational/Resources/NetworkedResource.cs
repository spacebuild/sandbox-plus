namespace Sandbox.Gravitational.Resources;

public struct NetworkedResource
{
	public string ResourceName { get; set; }
	public float Amount { get; set; }
	public float MaxAmount { get; set; }
	
	public ResourceInfo ResourceInfo { get => ResourceInfo.Get( ResourceName ); }
}
