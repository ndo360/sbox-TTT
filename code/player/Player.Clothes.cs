using System.Collections.Generic;
using Sandbox;

namespace TTT;

public partial class BaseClothing : ModelEntity
{
	public Player Wearer => Parent as Player;
	public virtual void Attached() { }
	public virtual void Detatched() { }
}

public partial class Player
{
	protected List<BaseClothing> Clothing { get; set; } = new();

	private void DressPlayer()
	{
		AttachClothing( "models/citizen_clothes/hat/balaclava/models/balaclava.vmdl" );
		AttachClothing( "models/citizen_clothes/jacket/longsleeve/models/longsleeve.vmdl" );
		AttachClothing( "models/citizen_clothes/jacket/longsleeve/models/longsleeve.vmdl" );
		AttachClothing( "models/citizen_clothes/trousers/smarttrousers/smarttrousers.vmdl" );
		AttachClothing( "models/citizen_clothes/shoes/smartshoes/smartshoes.vmdl" );
		AttachClothing( "models/citizen_clothes/vest/tactical_vest/models/tactical_vest.vmdl" );
	}

	public BaseClothing AttachClothing( string modelName )
	{
		var entity = new BaseClothing();
		entity.SetModel( modelName );
		AttachClothing( entity );
		return entity;
	}

	public T AttachClothing<T>() where T : BaseClothing, new()
	{
		var entity = new T();
		AttachClothing( entity );
		return entity;
	}

	public void AttachClothing( BaseClothing clothing )
	{
		clothing.SetParent( this, true );
		clothing.EnableShadowInFirstPerson = true;
		clothing.EnableHideInFirstPerson = true;
		clothing.Attached();

		Clothing.Add( clothing );
	}

	public void RemoveClothing()
	{
		Clothing.ForEach( ( entity ) =>
		{
			entity.Detatched();
			entity.Delete();
		} );

		Clothing.Clear();
	}
}
