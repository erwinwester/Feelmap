using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
	public class Influence
	{
		public int Id { get; set; }

		public string Description { get; set; }
		public Subject FromSubject { get; set; }
		public Subject ToSubject { get; set; }
		public float EnergyFactor { get; set; }
	}
}
