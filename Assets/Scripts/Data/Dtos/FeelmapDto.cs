using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
	[Serializable]
	public class FeelmapDto
	{
		public List<SubjectDto> SubjectDtos = new();
		public List<InfluenceDto> InfluenceDtos = new();
	}
}
