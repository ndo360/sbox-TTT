﻿using Sandbox;

namespace TTT;

[Hammer.EditorModel( "models/ammo/ammo_rifle/ammo_rifle.vmdl" )]
[Library( "ttt_ammo_rifle" )]
public partial class RifleAmmo : Ammo
{
	public override AmmoType Type => AmmoType.Rifle;
	public override int DefaultAmmoCount => 20;
	protected override string WorldModelPath => "models/ammo/ammo_rifle/ammo_rifle.vmdl";
}
