using Sandbox;

namespace TTT;

public partial class Player
{
	public BaseRole Role { get; set; }
	public Team Team => Role.Info?.Team ?? Team.None;

	public void SetRole( BaseRole role )
	{
		if ( role == Role )
			return;

		Role?.OnDeselect( this );
		Role = role;
		Role.OnSelect( this );

		Event.Run( TTTEvent.Player.Role.Changed, this );

		// Always send the role to this player's client
		if ( IsServer )
			SendRoleToClient();
	}

	[ClientRpc]
	private void ClientSetRole( int id )
	{
		IsRoleKnown = true;
		SetRole( id );
	}

	public void SetRole( string libraryName )
	{
		SetRole( Library.Create<BaseRole>( libraryName ) );
	}

	public void SetRole( int id )
	{
		SetRole( Asset.CreateFromId<BaseRole>( id ) );
	}

	/// <summary>
	/// Sends the role and all connected additional data like logic buttons of the current Player to the given target or - if no target was provided - the player itself
	/// </summary>
	/// <param name="to">optional - The target</param>
	public void SendRoleToClient( To? to = null )
	{
		ClientSetRole( to ?? To.Single( this ), Role.Info.Id );

		if ( to == null || to.Value.ToString().Equals( Client.Name ) )
			SendLogicButtonsToClient();
	}
}
