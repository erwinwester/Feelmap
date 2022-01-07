using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static Randomization;

namespace Assets.Scripts
{
	public class SceneController : MonoBehaviour
	{
		[SerializeField]
		private string filename = @".\Data\feelmap.json";
		// Prefabs.
		public GraphManager graphManager;

		// Variables that are only evaluated at start.
		[Range(1f, 1000f)]
		public int VerticesToSpawn;
		[Range(1f, 50f)]
		[Tooltip("Not used when the graph is acyclic. Because the graph is always connected.")]
		public float AverageConnectedness;
		public bool Acyclic;
		public float EdgeMinLength = 2f;
		public float EdgeMaxLength = 6f;
		public float EdgeStiffness = 10f;

		public Feelmap feelmap = new();

		// Start is called before the first frame update
		void Start()
		{
			InitializeGraphRandom();
			//ReloadGraph();
		}

		// Update is called once per frame
		void Update()
		{
			// Save en load
			if (Input.GetKeyDown(KeyCode.S) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
			{
				SaveFeelmap(feelmap);
			}

			if (Input.GetKeyDown(KeyCode.L) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
			{
				ReloadGraph();
			}

			if (Input.GetKeyDown(KeyCode.R) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
			{
				InitializeGraphRandom();
			}
		}

		private void InitializeGraphRandom()
		{
			feelmap = new();

			// Spawn all vertices.
			for (int i = 0; i < this.VerticesToSpawn; i++)
			{
				feelmap.Subjects.Add(new Subject
				{
					Id = i,
					Description = "Subject " + i.ToString(),
					Energy = Random.value,
					Distance = Random.Range(0.2f, 0.6f),
					Feel = Random.Range(0.4f, 1.0f),
					Root = (i == 0),
				});
			}

			//===========================================================

			// Spawn all edges.
			var unconnectedSubjects = new RandomList<Subject>(feelmap.Subjects);
			var connectedSubjects = new RandomList<Subject>();

			var root = unconnectedSubjects.First(s => s.Root == true);
			unconnectedSubjects.Remove(root);
			connectedSubjects.Add(root);

			int id = 0;
			int j = Random.Range(2, 5);
			Subject s1 = connectedSubjects.GetOne();
			while (unconnectedSubjects.Any())
			{
				if (j<=0)
                {
					j = Random.Range(2, 5);
					s1 = connectedSubjects.GetOne();
				}
				j--;

				Subject s2 = unconnectedSubjects.RemoveOne();

				Influence influence = new() { 
					Id = id++,
					FromSubject = s1,
					ToSubject = s2,
					EnergyFactor = 1.0f,
				};

				feelmap.Influences.Add(influence);
				connectedSubjects.Add(s2);
			}

			CreateGraphFromFeelmap();
		}

		private void ReloadGraph()
		{
			feelmap = LoadFeelMap();

			// This first clears the previous graph
			CreateGraphFromFeelmap();
		}

		private void CreateGraphFromFeelmap()
		{
			List<Edge> edges = new List<Edge>();
			List<Vertex> vertices = new List<Vertex>();

			// Root subject
			// ?

			foreach (var Subject in feelmap.Subjects)
			{
				Vertex vertex = new();
				vertex.Id = Subject.Id;
				vertex.Lightness = Subject.Energy;
				vertex.SetInfoObject(Subject);
				vertices.Add(vertex);
			}

			foreach (var Influence in feelmap.Influences)
			{
				Edge edge = new();
				edge.VertexA = vertices.Single(v => v.Id == Influence.FromSubject.Id);
				edge.VertexB = vertices.Single(v => v.Id == Influence.ToSubject.Id);
				edge.MaxLength = EdgeMaxLength;
				edge.MaxLength = EdgeMinLength + EdgeMaxLength * edge.VertexA.InfoObject.GetDistance() / edge.VertexB.InfoObject.GetFeel();
				edge.Stiffness = EdgeStiffness;
				edge.SetInfoObject(Influence);
				edges.Add(edge);
			}

			graphManager.CreateGraph(vertices, edges);
		}

		private void SaveFeelmap(Feelmap feelmap)
		{
			// Copy to Dto for serialization
			FeelmapDto feelmapDto = new();
			feelmapDto.SubjectDtos = feelmap.Subjects.Select(s => new SubjectDto()
			{
				Id = s.Id,
				Description = s.Description,
				Energy = s.Energy,
				Distance = s.Distance,
				Feel = s.Feel,
				Root = s.Root,
			}).ToList();
			feelmapDto.InfluenceDtos = feelmap.Influences.Select(i => new InfluenceDto()
			{
				Id = i.Id,
				Description = i.Description,
				EnergyFactor = i.EnergyFactor,
				FromSubjectId = i.FromSubject.Id,
				ToSubjectId = i.ToSubject.Id,
			}).ToList();

			// Serialize to file
			JsonSerializer serializer = new();
			using StreamWriter sw = new(filename);
			using JsonWriter writer = new JsonTextWriter(sw);
			serializer.Serialize(writer, feelmapDto);
		}

		private Feelmap LoadFeelMap()
		{
			FeelmapDto feelmapDto;
			Feelmap feelmap = new();

			// Deserialize from file
			JsonSerializer serializer = new();
			using (StreamReader sr = new(filename))
			{
				using (JsonReader reader = new JsonTextReader(sr))
				{
					feelmapDto = serializer.Deserialize<FeelmapDto>(reader);
				}
			}

			// Copy to objects
			feelmap.Subjects = feelmapDto.SubjectDtos.Select(s => new Subject()
			{
				Id = s.Id,
				Description = s.Description,
				Energy = s.Energy,
				Distance = s.Distance,
				Feel = s.Feel,
				Root = s.Root,
			}).ToList();
			feelmap.Influences = feelmapDto.InfluenceDtos.Select(i => new Influence()
			{
				Id = i.Id,
				Description = i.Description,
				EnergyFactor = i.EnergyFactor,
				FromSubject = feelmap.Subjects.Single(s => s.Id == i.FromSubjectId),
				ToSubject = feelmap.Subjects.Single(s => s.Id == i.ToSubjectId),
			}).ToList();

			return feelmap;
		}
	}
}