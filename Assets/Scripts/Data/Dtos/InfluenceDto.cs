using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
	[Serializable]
	public class InfluenceDto
	{
		public int Id { get; set; }

		public string Description { get; set; }
		public int FromSubjectId { get; set; }
		public int ToSubjectId { get; set; }
		public float EnergyFactor { get; set; }
	}
}
