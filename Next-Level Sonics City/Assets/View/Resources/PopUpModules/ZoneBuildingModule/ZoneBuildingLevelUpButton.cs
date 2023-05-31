using Model;
using Model.Tiles.Buildings;
using System.Collections.Generic;
using UnityEngine;
using View.GUI;

public class ZoneBuildingLevelUpButton : MonoBehaviour, IClickable
{
	private Tile _tileModel;
	private void Awake()
	{
		transform.parent.parent.GetComponent<PopUpWindow>().TileModelChanged.AddListener(OnTileModelChanged);
	}

	private void OnTileModelChanged(Tile tilemodel)
	{
		_tileModel = tilemodel;
		if (((IZoneBuilding)_tileModel).Level == ZoneBuildingLevel.THREE || ((IZoneBuilding)tilemodel).Level == ZoneBuildingLevel.ZERO) { gameObject.SetActive(false); }
	}

	public void OnClick(bool isLeftMouseButton, Vector3 location)
	{
		((IZoneBuilding)_tileModel)?.LevelUp();
	}

	public bool OnDrag(bool isLeftMouseButton, Vector3 direction)
	{
		return true;
	}

	public void OnDragEnd(bool isLeftMouseButton)
	{
		
	}

	public void OnDragStart(bool isLeftMouseButton, Vector3 location)
	{
		
	}

	public void OnHover(Vector3 location)
	{
		
	}

	public void OnHoverEnd()
	{
		
	}

	public void OnHoverStart(Vector3 location)
	{

	}

	public void OnScroll(float delta)
	{

	}

	public void OnSecondClick(List<IClickable> clicked)
	{

	}
}
