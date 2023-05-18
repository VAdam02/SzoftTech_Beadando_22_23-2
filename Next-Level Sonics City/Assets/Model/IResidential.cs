using Model.RoadGrids;
using System.Collections.Generic;

namespace Model
{
	public interface IResidential
	{
		public int ResidentLimit { get; }

		/// <summary>
		/// Register a residential zone to the road grid
		/// </summary>
		/// <param name="roadGrid">Roadgrid where to register</param>
		public void RegisterResidential(RoadGrid roadGrid);

		/// <summary>
		/// Unregister a residential zone from the road grid
		/// </summary>
		/// <param name="roadGrid">roadgrid where from unregister</param>
		public void UnregisterResidential(RoadGrid roadGrid);

		/// <summary>
		/// Move in a person to the residential zone
		/// </summary>
		/// <param name="person">Person who will move in</param>
		public void MoveIn(Person person);

		/// <summary>
		/// Move out a person from the residential zone
		/// </summary>
		/// <param name="person">Person who will move out</param>
		public void MoveOut(Person person);

		/// <summary>
		/// Get all residents in the residential zone
		/// </summary>
		/// <returns>List of residents in residential</returns>
		public List<Person> GetResidents();

		/// <summary>
		/// Get the count of residents in the residential zone
		/// </summary>
		/// <returns>Count of residents</returns>
		public int GetResidentsCount();

		/// <summary>
		/// Get the tile of the residential zone
		/// </summary>
		/// <returns>Tile of the residential zone</returns>
		public Tile GetTile();
	}
}