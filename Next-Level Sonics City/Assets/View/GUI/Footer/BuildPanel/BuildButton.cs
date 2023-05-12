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
			switch (type)
			{
				case TileType.PoliceDepartment:
					TileManager.Instance.GhostTile = TileManager.Instance.GenerateFromModel(new PoliceDepartmentBuildingTile(0, 0, 0, Rotation.Zero));
					break;
				case TileType.Stadion:
					TileManager.Instance.GhostTile = TileManager.Instance.GenerateFromModel(new Stadion(0, 0, 0, Rotation.Zero));
					break;
				case TileType.FireDepartment:
					TileManager.Instance.GhostTile = TileManager.Instance.GenerateFromModel(new FireDepartment(0, 0, 0, Rotation.Zero));
					break;
				case TileType.MiddleSchool:
					TileManager.Instance.GhostTile = TileManager.Instance.GenerateFromModel(new MiddleSchool(0, 0, 0, Rotation.Zero));
					break;
				case TileType.HighSchool:
					TileManager.Instance.GhostTile = TileManager.Instance.GenerateFromModel(new HighSchool(0, 0, 0, Rotation.Zero));
					break;
				case TileType.PowerPlant:
					TileManager.Instance.GhostTile = TileManager.Instance.GenerateFromModel(new PowerPlant(0, 0, 0, Rotation.Zero));
					break;
				case TileType.Forest:
					TileManager.Instance.GhostTile = TileManager.Instance.GenerateFromModel(new Forest(0, 0, 0));
					break;
				case TileType.ElectricPole:
					TileManager.Instance.GhostTile = TileManager.Instance.GenerateFromModel(new ElectricPole(0, 0, 0));
					break;

				case TileType.Road:
					TileManager.Instance.GhostTile = TileManager.Instance.GenerateFromModel(new RoadTile(0, 0, 0));
					break;
				default:
					throw new InvalidOperationException();
			}
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
