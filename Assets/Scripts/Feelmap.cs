using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
	class Feelmap
	{
		public Subject MainSubject { get; set; }
		public List<Subject> Subjects = new();
		public List<Influence> Influences = new();
	}
}
