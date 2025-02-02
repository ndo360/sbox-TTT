using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class CrosshairPage : Panel
{
	public bool ShowTop { get; set; } = true;
	public bool ShowDot { get; set; } = false;

	public int Size { get; set; } = 4;
	public int Thickness { get; set; } = 3;
	public int Gap { get; set; } = 3;
	public int OutlineThickness { get; set; } = 0;

	public Color SelectedColor { get; set; } = Color.White;

	public CrosshairPage()
	{
		var crosshairConfig = FileSystem.Data.ReadJson<Crosshair.Properties>( "crosshair.json" );
		if ( crosshairConfig == null )
			return;

		ShowTop = crosshairConfig.ShowTop;
		ShowDot = crosshairConfig.ShowDot;
		Size = crosshairConfig.Size;
		Thickness = crosshairConfig.Thickness;
		Gap = crosshairConfig.Gap;
		OutlineThickness = crosshairConfig.OutlineThickness;
		SelectedColor = crosshairConfig.Color;
	}

	public void SaveCrosshairData()
	{
		if ( Crosshair.Instance != null )
			FileSystem.Data.WriteJson( "crosshair.json", Crosshair.Instance.Config );
		SettingsMenu.Instance.PopPage();
	}

	public override void Tick()
	{
		Crosshair.Instance?.SetupCrosshair( new UI.Crosshair.Properties(
			ShowTop,
			ShowDot,
			Size,
			Thickness,
			OutlineThickness,
			Gap,
			SelectedColor
		) );
	}
}
