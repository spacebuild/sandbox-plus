using Sandbox.Gravitational.Resources;
using Sandbox.Gravitational.Util;
using Sdt.Environment;

namespace Sandbox.Gravitational.Components;

public sealed partial class GravitationalEnvironmentComponent: Component
{

	public static int Instances = 0;
	
	public string Name { get; set; }
	
	public float OuterRadius { get; set; }
	
	public float InnerRadius { get; set; }
	
	public GravityType Type { get; set; }
	
	public float GravityScale { get; set; }
	
	public bool Inverse { get; set; }
	
	public float Volume { get; set;  }

	public EnvironmentPreset Preset { get; set; }

	public float Atmosphere { get; set; }

	public int Temperature { get; set; }

	public int NightTemperature { get; set; }

	public NetList<NetworkedResource> Resources { get; set; }

	public GravitationalEnvironmentComponent()
	{
		GravitationalEnvironmentComponent.Instances++;
	}
}
