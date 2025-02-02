using Sandbox;

namespace TTT;

[Hammer.EntityTool( "Random Ammo", "TTT", "Place where a random ammo box will spawn in the beginning of the round." )]
[Hammer.EditorModel( "models/ammo/ammo_smg/ammo_smg.vmdl" )]
[Library( "ttt_ammo_random" )]
public class AmmoRandom : Entity
{
	public override void Spawn()
	{
		base.Spawn();
		Transmit = TransmitType.Never;
	}

	public void Activate()
	{
		var ammoTypeToSpawn = (AmmoType)Rand.Int( 1, 5 );
		var ent = Ammo.Create( ammoTypeToSpawn );
		ent.Position = Position;
		ent.Rotation = Rotation;
	}

	[Event.Entity.PostCleanup]
	private void OnCleanup()
	{
		Activate();
	}
}
