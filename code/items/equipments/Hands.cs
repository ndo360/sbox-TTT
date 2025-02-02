using System.Threading.Tasks;

using Sandbox;

namespace TTT;

public interface IGrabbable
{
	bool IsHolding { get; }
	void Drop();
	void Update( Player player );
	void SecondaryAction();
}

[Hammer.Skip]
[Library( "ttt_equipment_hands", Title = "Hands" )]
public partial class Hands : Carriable
{
	public static readonly float MAX_INTERACT_DISTANCE = 75;
	public static readonly string MIDDLE_HANDS_ATTACHMENT = "middle_of_both_hands";

	private const float MAX_PICKUP_MASS = 205;
	private Vector3 MAX_PICKUP_SIZE = new( 75, 75, 75 );
	private const float PUSHING_FORCE = 350f;

	private IGrabbable GrabbedEntity;
	private bool IsHoldingEntity => GrabbedEntity != null && (GrabbedEntity?.IsHolding ?? false);
	private bool IsPushingEntity = false;

	public override void Spawn()
	{
		base.Spawn();

		RenderColor = Color.Transparent;
	}

	public override void Simulate( Client client )
	{
		if ( !IsServer )
			return;

		if ( Owner is not Player player )
			return;

		using ( Prediction.Off() )
		{
			if ( Input.Pressed( InputButton.Attack1 ) )
			{
				if ( IsHoldingEntity )
				{
					GrabbedEntity?.SecondaryAction();
				}
				else
				{
					TryGrabEntity( player );
				}
			}
			else if ( Input.Pressed( InputButton.Attack2 ) )
			{
				if ( IsHoldingEntity )
				{
					GrabbedEntity?.Drop();
				}
				else
				{
					PushEntity( player );
				}
			}

			GrabbedEntity?.Update( player );
		}
	}

	private void PushEntity( Player player )
	{
		if ( IsPushingEntity )
		{
			return;
		}

		TraceResult tr = Trace.Ray( player.EyePosition, player.EyePosition + player.EyeRotation.Forward * MAX_INTERACT_DISTANCE )
				.EntitiesOnly()
				.Ignore( player )
				.Run();

		if ( !tr.Hit || !tr.Entity.IsValid() )
		{
			return;
		}

		IsPushingEntity = true;

		player.SetAnimParameter( "b_attack", true );
		player.SetAnimParameter( "holdtype", 4 );
		player.SetAnimParameter( "holdtype_handedness", 0 );

		tr.Entity.Velocity += player.EyeRotation.Forward * PUSHING_FORCE;

		_ = WaitForAnimationFinish();
	}

	private async Task WaitForAnimationFinish()
	{
		await GameTask.DelaySeconds( 0.6f );
		IsPushingEntity = false;
	}

	private void TryGrabEntity( Player player )
	{
		if ( IsHoldingEntity )
		{
			return;
		}

		Vector3 eyePos = player.EyePosition;
		Vector3 eyeDir = player.EyeRotation.Forward;

		TraceResult tr = Trace.Ray( eyePos, eyePos + eyeDir * MAX_INTERACT_DISTANCE )
			.UseHitboxes()
			.Ignore( player )
			.HitLayer( CollisionLayer.Debris )
			.EntitiesOnly()
			.Run();

		// Make sure trace is hit and not null.
		if ( !tr.Hit || !tr.Entity.IsValid() )
			return;

		// Only allow dynamic entities to be picked up.
		if ( tr.Body == null || tr.Body.BodyType == PhysicsBodyType.Keyframed || tr.Body.BodyType == PhysicsBodyType.Static )
			return;

		// Cannot pickup items held by other players.
		if ( tr.Entity.Parent != null )
			return;

		switch ( tr.Entity )
		{
			case Corpse corpse:
				GrabbedEntity = new GrabbableCorpse( player, corpse, tr.Body, tr.Bone );
				break;
			case Carriable: // Ignore any size requirements, any weapon can be picked up.
				GrabbedEntity = new GrabbableProp( player, tr.Entity );
				break;
			case ModelEntity model:
				if ( !model.CollisionBounds.Size.HasGreatorOrEqualAxis( MAX_PICKUP_SIZE ) && model.PhysicsGroup.Mass < MAX_PICKUP_MASS )
					GrabbedEntity = new GrabbableProp( player, tr.Entity );
				break;
		}
	}

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		GrabbedEntity?.Drop();

		base.ActiveEnd( ent, dropped );
	}

	protected override void OnDestroy()
	{
		GrabbedEntity?.Drop();

		base.OnDestroy();
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		if ( !IsServer )
			return;

		if ( IsPushingEntity )
			return;

		if ( IsHoldingEntity )
		{
			anim.SetAnimParameter( "holdtype", 4 );
			anim.SetAnimParameter( "holdtype_handedness", 0 );
		}
		else
		{
			anim.SetAnimParameter( "holdtype", 0 );
		}
	}
}
