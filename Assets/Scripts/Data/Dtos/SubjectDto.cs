using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
	[Serializable]
	public class SubjectDto
	{
		public int Id { get; set; }
		public string Description { get; set; }
		public float Energy { get; set; }
		public float Feel { get; set; }
		public float Distance { get; set; }
		public bool Root { get; set; }
	}
}
