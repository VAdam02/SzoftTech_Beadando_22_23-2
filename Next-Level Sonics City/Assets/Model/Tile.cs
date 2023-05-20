using Model.RoadGrids;
using Model.Tiles;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Model
{
	public abstract class Tile
	{
		private uint _designID;
		public uint DesignID
		{
			get { return _designID; }
			protected set
			{
				_designID = value;
				if (MainThreadDispatcher.Instance is MainThreadDispatcher mainThread)
				{
					mainThread.Enqueue(() =>
					{
						DesignIDChangeEvent.Invoke();
					});
				}
			}
		}
		public UnityEvent DesignIDChangeEvent = new();

		public Vector3 Coordinates { get; protected set; }
		public readonly UnityEvent<Tile> OnTileChange = new();
		public readonly UnityEvent<Tile> OnTileDelete = new();

		/// <summary>
		/// Constructor for Tile
		/// </summary>
		/// <param name="x">X coordinate for tile</param>
		/// <param name="y">Y coordinate for tile</param>
		/// <param name="designID">designID for the tile</param>
		public Tile(int x, int y, uint designID)
		{
			if (x < 0 || y < 0) { throw new System.ArgumentException("Coordinates must be positive"); }
			if (x >= City.Instance.GetSize() || y >= City.Instance.GetSize()) { throw new System.ArgumentException("Coordinates must be less than city size"); }

			DesignID = designID;
			Coordinates = new Vector3(x, y, 0);
		}

		/// <summary>
		/// Returns the type of the tile
		/// </summary>
		/// <returns>Type of the tile</returns>
		public abstract TileType GetTileType();

		/// <summary>
		/// Returns if the tile can be built on location
		/// </summary>
		/// <returns>True if there is empty tile</returns>
		public virtual bool CanBuild()
		{
			return City.Instance.GetTile(Coordinates) is EmptyTile;
		}

		/// <summary>
		/// Returns if the tile can be destroyed
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void UpdateCoordinates(int x, int y)
		{
			if (_isFinalized) { throw new InvalidOperationException(); }

			Coordinates = new Vector3(x, y, 0);
		}

		protected bool _isFinalized = false;

		/// <summary>
		/// <para>MUST BE CONTAINS ONLY <code>this.Finalizing()</code> AND ALL THE OTHER LOGIC MUST BE IMPLEMENTED IN THAT</para>
		/// <para>Finalizes the tile</para>
		/// </summary>
		public virtual void FinalizeTile()
		{
			Finalizing();
		}

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Finalizing()</code></para>
		/// <para>Do the actual finalization</para>
		/// </summary>
		protected void Finalizing()
		{
			if (_isFinalized) return;
			_isFinalized = true;

			if (this is IRoadGridElement roadGridElement)
			{
				roadGridElement.RegisterRoadGridElement();
			}
		}

		/// <summary>
		/// <para>MUST BE CONTAINS ONLY <code>this.Deleting()</code> AND ALL THE OTHER LOGIC MUST BE IMPLEMENTED IN THAT</para>
		/// <para>Delete the tile</para>
		/// </summary>
		public virtual void DeleteTile()
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			Deleting();
		}

		/// <summary>
		/// <para>MUST BE STARTED WITH <code>base.Deleting()</code></para>
		/// <para>Do the deletion administration</para>
		/// </summary>
		protected void Deleting()
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }

			if (this is IRoadGridElement roadGridElement)
			{
				roadGridElement.UnregisterRoadGridElement();
			}

			if (MainThreadDispatcher.Instance is MainThreadDispatcher mainThread)
			{
				mainThread.Enqueue(() =>
				{
					OnTileDelete.Invoke(this);
				});
			}
		}

		/// <summary>
		/// Called when a neighbor tile replaced
		/// </summary>
		/// <param name="oldTile">Old tile</param>
		/// <param name="newTile">New tile</param>
		public virtual void NeighborTileReplaced(Tile oldTile, Tile newTile)
		{
			if (!_isFinalized) { throw new InvalidOperationException(); }
		}

		/// <summary>
		/// Returns the price of building this tile
		/// </summary>
		/// <returns></returns>
		public abstract int GetBuildPrice();

		/// <summary>
		/// Returns the price of destroying this tile
		/// </summary>
		/// <returns></returns>
		public abstract int GetDestroyIncome();

		/// <summary>
		/// Returns the price of maintaining this tile
		/// </summary>
		/// <returns></returns>
		public virtual int GetMaintainanceCost()
		{
			return 0;
		}

		/// <summary>
		/// Returns the tile transparency for the effects
		/// </summary>
		/// <returns>Transparency of tile</returns>
		public abstract float GetTransparency();
	}
}