using System.Collections.Generic;
using Model.RoadGrids;

namespace Model
{
	public interface IResidential
	{
		public void RegisterResidential(RoadGrid roadGrid);
		public void UnregisterResidential(RoadGrid roadGrid);

		public bool MoveIn(Person person);
		public bool MoveOut(Person person);
		public List<Person> GetResidents();
		public int GetResidentsCount();
		public int GetResidentsLimit();

		public Tile GetTile();
	}
}