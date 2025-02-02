﻿
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class ChatBox : Panel
{
	public enum Channel
	{
		All,
		Role,
		Spectator
	}

	private static readonly Color _allChatColor = Color.FromBytes( 26, 196, 77 );
	private static readonly Color _spectatorChatColor = Color.FromBytes( 252, 219, 56 );

	public static ChatBox Instance;

	public Panel EntryCanvas { get; set; }
	public TabTextEntry Input { get; set; }
	public Channel CurrentChannel { get; private set; } = Channel.All;

	public bool IsOpen
	{
		get => HasClass( "open" );
		set
		{
			SetClass( "open", value );
			if ( value )
			{
				Input.Focus();
				Input.Text = string.Empty;
				Input.Label.SetCaretPosition( 0 );
			}
		}
	}

	public ChatBox()
	{
		Instance = this;

		Sandbox.Hooks.Chat.OnOpenChat += () =>
		{
			IsOpen = !IsOpen;
		};

		EntryCanvas.PreferScrollToBottom = true;
		EntryCanvas.TryScrollToBottom();

		Input.AddEventListener( "onsubmit", Submit );
		Input.AddEventListener( "onblur", () => IsOpen = false );
		Input.OnTabPressed += OnTabPressed;
	}

	public override void Tick()
	{
		base.Tick();

		if ( Local.Pawn is not Player player )
			return;

		if ( !IsOpen )
			return;

		if ( !player.IsAlive() )
			CurrentChannel = Channel.Spectator;

		switch ( CurrentChannel )
		{
			case Channel.All:
				Input.Style.BorderColor = _allChatColor;
				return;
			case Channel.Spectator:
				Input.Style.BorderColor = _spectatorChatColor;
				return;
			case Channel.Role:
				Input.Style.BorderColor = player.Role.Info.Color;
				return;
		}

		Input.Placeholder = string.Empty;
	}

	public void AddEntry( string name, string message, string c = "" )
	{
		var entry = new ChatEntry( name, message );
		if ( !string.IsNullOrEmpty( c ) ) entry.AddClass( c );
		EntryCanvas.AddChild( entry );
	}

	public void AddEntry( string name, string message, Color? color )
	{
		var entry = new ChatEntry( name, message, color );
		EntryCanvas.AddChild( entry );
	}

	private void Submit()
	{
		if ( string.IsNullOrWhiteSpace( Input.Text ) ) return;

		SendChat( Input.Text, CurrentChannel );
	}

	[ServerCmd]
	public static void SendChat( string message, Channel channel )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player player )
			return;

		if ( message.Contains( '\n' ) || message.Contains( '\r' ) )
			return;

		if ( !player.IsAlive() )
		{
			AddChat( To.Multiple( Utils.GetDeadClients() ), player.Client.Name, message, Channel.Spectator );
			return;
		}

		if ( channel == Channel.All )
			AddChat( To.Everyone, player.Client.Name, message, channel, player.IsRoleKnown ? player.Role.Info.Id : -1 );
		else if ( channel == Channel.Role && player.Role.Info.CanRoleChat )
			AddChat( To.Multiple( Utils.GetClientsWithRole( player.Role ) ), player.Client.Name, message, channel, player.Role.Info.Id );
	}

	[ClientCmd( "chat_add", CanBeCalledFromServer = true )]
	public static void AddChat( string name, string message, Channel channel, int roleId = -1 )
	{
		switch ( channel )
		{
			case Channel.All:
				Instance?.AddEntry( name, message, roleId != -1 ? Asset.FromId<RoleInfo>( roleId ).Color : _allChatColor );
				return;
			case Channel.Role:
				Instance?.AddEntry( $"(TEAM) {name}", message, Asset.FromId<RoleInfo>( roleId ).Color );
				return;
			case Channel.Spectator:
				Instance?.AddEntry( name, message, _spectatorChatColor );
				return;
		}
	}

	[ClientCmd( "chat_add_info", CanBeCalledFromServer = true )]
	public static void AddInfo( string message )
	{
		Instance?.AddEntry( message, "", "info" );
	}

	private void OnTabPressed()
	{
		if ( Local.Pawn is not Player player || !player.IsAlive() )
			return;

		if ( player.Role.Info.CanRoleChat )
			CurrentChannel = CurrentChannel == Channel.All ? Channel.Role : Channel.All;
	}
}

