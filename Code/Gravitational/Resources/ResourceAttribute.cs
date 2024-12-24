namespace Sandbox.Gravitational.Resources;

public readonly struct ResourceAttribute
{
	public ResourceAttribute(string name, float modifier = 1f)
	{
		Name = name;
		Modifier = modifier;
	}

	public const string Coolant = "coolant";
	public const string Flammable = "Flammable";
	
	public string Name { get; }
	public float Modifier { get; }

	public override string ToString() => $"Attribute {Name} x {Modifier}";
	
}
