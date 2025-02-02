using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class HomePage : Panel
{
	private Button ForceSpectatorButton { get; set; }

	public void GoToKeyBindingsPage()
	{
		SettingsMenu.Instance.AddPage( new KeyBindingsPage() );
	}

	public void GoToCrosshairPage()
	{
		SettingsMenu.Instance.AddPage( new CrosshairPage() );
	}

	public void GoToComponentTesting()
	{
		SettingsMenu.Instance.AddPage( new ComponentTestingPage() );
	}

	public override void Tick()
	{
		if ( Local.Pawn is not Player player )
			return;

		ForceSpectatorButton.Text = $"Force Spectator Mode ({(player.IsForcedSpectator ? "Enabled" : "Disabled")})";
	}

	public void ToggleForceSpectator()
	{
		Game.ToggleForceSpectator();
	}
}
