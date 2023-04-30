using Model.Simulation;
using System.Collections;
using System.Collections.Generic;

namespace Model.Tiles
{
    public class Road : Tile
    {
		public Road FromLeft  { get; set; }
		public Road FromRight { get; set; }
		public Road FromAbove { get; set; }
		public Road FromBelow { get; set; }

		public Road(int x, int y, uint designID) : base(x, y, designID)
		{
			ConnectToSurrounding();
		}

		private void ConnectToSurrounding()
		{
			int lowerX, upperX, lowerY, upperY;

			lowerX = (int)Coordinates.x - 1;
			lowerY = (int)Coordinates.y - 1;
			upperX = (int)Coordinates.x + 1;
			upperY = (int)Coordinates.y + 1;

			int lowerLimit = 0;
			int upperLimit = SimEngine.Instance.GetSize();

			if (lowerX < lowerLimit)
			{
				if (SimEngine.Instance.GetTile(lowerX, (int)Coordinates.y) is Road)
				{
					((Road)SimEngine.Instance.GetTile(lowerX, (int)Coordinates.y)).FromRight = this;
					FromLeft = (Road)SimEngine.Instance.GetTile(lowerX, (int)Coordinates.y);
				}
			}

			if (lowerY < lowerLimit)
			{
				if (SimEngine.Instance.GetTile((int)Coordinates.x, lowerY) is Road)
				{
					((Road)SimEngine.Instance.GetTile((int)Coordinates.x, lowerY)).FromBelow = this;
					FromAbove = (Road)SimEngine.Instance.GetTile((int)Coordinates.x, lowerY);
				}
			}

			if (upperX > upperLimit)
			{
				if (SimEngine.Instance.GetTile(upperX, (int)Coordinates.y) is Road)
				{
					((Road)SimEngine.Instance.GetTile(lowerX, (int)Coordinates.y)).FromLeft = this;
					FromRight = (Road)SimEngine.Instance.GetTile(upperX, (int)Coordinates.y);
				}
			}

			if (upperY > upperLimit)
			{
				if (SimEngine.Instance.GetTile((int)Coordinates.x, upperY) is Road)
				{
					((Road)SimEngine.Instance.GetTile((int)Coordinates.x, upperY)).FromAbove = this;
					FromBelow = (Road)SimEngine.Instance.GetTile((int)Coordinates.x, upperY);
				}
			}
		}

		private const uint _left = 1;
		private const uint _above = 2;
		private const uint _right = 4;
		private const uint _below = 8;

		public void UpdateDesignID()
		{
			uint updatedDesignID = 0;

			if (FromLeft is Road)
			{
				updatedDesignID |= _left;
			}

			if (FromAbove is Road)
			{
				updatedDesignID |= _above;
			}

			if (FromRight is Road)
			{
				updatedDesignID |= _right;
			}

			if (FromBelow is Road)
			{
				updatedDesignID |= _below;
			}

			DesignID = updatedDesignID;
		}

		private void DisconnectFromSurrounding()
		{
			int lowerX, upperX, lowerY, upperY;

			lowerX = (int)Coordinates.x - 1;
			lowerY = (int)Coordinates.y - 1;
			upperX = (int)Coordinates.x + 1;
			upperY = (int)Coordinates.y + 1;

			int lowerLimit = 0;
			int upperLimit = SimEngine.Instance.GetSize();

			if (lowerX < lowerLimit)
			{
				if (SimEngine.Instance.GetTile(lowerX, (int)Coordinates.y) is Road)
				{
					((Road)SimEngine.Instance.GetTile(lowerX, (int)Coordinates.y)).FromRight = null;
				}
			}

			if (lowerY < lowerLimit)
			{
				if (SimEngine.Instance.GetTile((int)Coordinates.x, lowerY) is Road)
				{
					((Road)SimEngine.Instance.GetTile((int)Coordinates.x, lowerY)).FromBelow = null;
				}
			}

			if (upperX > upperLimit)
			{
				if (SimEngine.Instance.GetTile(upperX, (int)Coordinates.y) is Road)
				{
					((Road)SimEngine.Instance.GetTile(upperX, (int)Coordinates.y)).FromLeft = null;
				}
			}

			if (upperY > upperLimit)
			{
				if (SimEngine.Instance.GetTile((int)Coordinates.x, upperY) is Road)
				{
					((Road)SimEngine.Instance.GetTile((int)Coordinates.x, upperY)).FromAbove = null;
				}
			}
		}

		~Road()
		{
			DisconnectFromSurrounding();
		}

		public override int GetBuildPrice() //TODO implementing logic
		{
			return BUILD_PRICE;
		}

		public override int GetDestroyPrice()
		{
			return DESTROY_PRICE;
		}

		public override int GetMaintainanceCost()
		{
			return GetBuildPrice() / 10;
		}
	}
}
