using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class WIPDisclaimer : Panel
{
	public static WIPDisclaimer Instance { get; set; }

	public WIPDisclaimer() : base()
	{
		Instance = this;

		Panel wrapper = new( this );
		wrapper.Style.FlexDirection = FlexDirection.Column;
		wrapper.Style.TextAlign = TextAlign.Center;
		wrapper.AddClass( "centered-vertical-90" );
		wrapper.AddClass( "opacity-medium" );
		wrapper.AddClass( "text-color-info" );
		wrapper.AddClass( "text-shadow" );

		wrapper.Add.Label( "TTT is work-in-progress! Checkout the project at github.com/mzegar/sbox-TTT or discord.gg/rrsrakF8N3" );

		AddClass( "fullscreen" );
	}
}
