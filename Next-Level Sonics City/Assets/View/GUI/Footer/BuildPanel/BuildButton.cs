using Model.Tiles;
using Model.Tiles.Buildings;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace View.GUI.Footer.BuildPanel
{
	public class BuildButton : MonoBehaviour, IClickable
	{
		public TileType type;
		public void OnClick(bool isLeftMouseButton, Vector3 location)
		{
			TileManager.Instance.GhostTile = type switch
			{
				TileType.PoliceDepartment => TileManager.Instance.GenerateFromModel(new PoliceDepartmentBuildingTile(0, 0, 0, Rotation.Zero)),
				TileType.Stadion => TileManager.Instance.GenerateFromModel(new StadionBuildingTile(0, 0, 0, Rotation.Zero)),
				TileType.MiddleSchool => TileManager.Instance.GenerateFromModel(new MiddleSchool(0, 0, 0, Rotation.Zero)),
				TileType.HighSchool => TileManager.Instance.GenerateFromModel(new HighSchool(0, 0, 0, Rotation.Zero)),
				TileType.PowerPlant => TileManager.Instance.GenerateFromModel(new PowerPlant(0, 0, 0, Rotation.Zero)),
				TileType.Forest => TileManager.Instance.GenerateFromModel(new ForestTile(0, 0, 0)),
				TileType.ElectricPole => TileManager.Instance.GenerateFromModel(new ElectricPole(0, 0, 0)),
				TileType.Road => TileManager.Instance.GenerateFromModel(new RoadTile(0, 0)),
				_ => throw new InvalidOperationException(),
			};
		}

		public bool OnDrag(bool isLeftMouseButton, Vector3 direction) { return true; }

		public void OnDragEnd(bool isLeftMouseButton) { }

		public void OnDragStart(bool isLeftMouseButton, Vector3 location) { }

		public void OnHover(Vector3 location) { }

		public void OnHoverEnd() { }

		public void OnHoverStart(Vector3 location) { }

		public void OnScroll(float delta) { }

		public void OnSecondClick(List<IClickable> clicked) { }
	}
}
