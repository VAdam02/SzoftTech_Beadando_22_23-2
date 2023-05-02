using System.Collections.Generic;

namespace Model
{
	public class City
	{
		public City()
		{

		}

		private readonly List<IWorkplace> _workplace = new();
		public List<IWorkplace> Workplaces { get { return _workplace; } }
		public void AddWorkplace(IWorkplace workplace)
		{
			lock (_workplace)
			{
				_workplace.Add(workplace);
			}
		}
		public void RemoveWorkplace(IWorkplace workplace)
		{
			lock (_workplace)
			{
				_workplace.Remove(workplace);
			}
		}
	}
}