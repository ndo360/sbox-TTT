using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public partial class ShopItem : Panel
{
	public ItemInfo ItemInfo { get; init; }
	public bool IsDisabled { get; private set; }

	private readonly Image _itemIcon;
	private readonly Label _itemNameLabel;
	private readonly Label _itemPriceLabel;

	public ShopItem( Panel parent, ItemInfo itemInfo ) : base( parent )
	{
		AddClass( "rounded" );
		AddClass( "text-shadow" );
		AddClass( "background-color-primary" );

		_itemPriceLabel = Add.Label();
		_itemPriceLabel.AddClass( "item-price-label" );
		_itemPriceLabel.AddClass( "text-color-info" );

		_itemIcon = Add.Image();
		_itemIcon.AddClass( "item-icon" );

		_itemNameLabel = Add.Label();
		_itemNameLabel.AddClass( "item-name-label" );

		_itemNameLabel.Text = itemInfo.Title;
		_itemPriceLabel.Text = $"${itemInfo.Price}";
		_itemIcon.SetImage( itemInfo.Icon );

		ItemInfo = itemInfo;
	}

	public void UpdateAvailability( Player.BuyError buyError )
	{
		SetClass( "cannot-purchase", buyError != Player.BuyError.None );
		IsDisabled = buyError != Player.BuyError.None;
	}
}
