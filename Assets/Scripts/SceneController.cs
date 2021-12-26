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

		}

		public void InitializeGraphRandom()
		{
			List<Edge> edges = new List<Edge>();
			List<Vertex> vertices = new List<Vertex>();

			// Spawn all vertices.
			for (int i = 0; i < this.VerticesToSpawn; i++)
			{
				Vertex vertex = new();
				vertex.Id = i;
				vertex.Lightness = Random.value;
				vertex.SetInfoObject(new Subject { 
					Description = "Subject "+i.ToString(),
					Energy = Random.value,
					Distance = Random.Range(0.2f, 0.6f),
					Feel = Random.Range(0.4f, 1.0f),
					Root = (i == 0),
				});
				vertices.Add(vertex);
			}

			// Spawn all edges.
			var unconnectedVertices = new RandomList<Vertex>(vertices);
			var connectedVertices = new RandomList<Vertex>();
			//connectedVertices.Add(unconnectedVertices.RemoveOne());
			var root = unconnectedVertices.First(v => v.InfoObject.IsRoot());
			unconnectedVertices.Remove(root);
			connectedVertices.Add(root);

			int j = Random.Range(2, 5); ;
			Vertex v1 = connectedVertices.GetOne();
			while (unconnectedVertices.Any())
			{
				if (j<=0)
                {
					j = Random.Range(2, 5);
					v1 = connectedVertices.GetOne();
				}
				j--;

				Vertex v2 = unconnectedVertices.RemoveOne();

				Edge edge = new();
				edge.VertexA = v1;
				edge.VertexB = v2;
				// MaxLength
				// - More distance more length
				// - More feeling less length
				edge.MaxLength = EdgeMinLength + EdgeMaxLength * v2.InfoObject.GetDistance() / v2.InfoObject.GetFeel();
				edge.Stiffness = EdgeStiffness;
				edges.Add(edge);
				connectedVertices.Add(v2);
			}

			if (!Acyclic)
			{
				int maxEdges = vertices.Count * (vertices.Count - 1) / 2;
				int edgesToSpawn = (int)Mathf.Min(maxEdges, vertices.Count * AverageConnectedness);
				float actualAverageConnectedness = (float)edgesToSpawn / (float)vertices.Count;
				float currentAverageConnectedness = (float)edges.Count / (float)vertices.Count;
				while (currentAverageConnectedness < actualAverageConnectedness)
				{
					var notFullyConnectedVertices = new RandomList<Vertex>(vertices.Where(v => v.Connectedness < vertices.Count - 1));
					v1 = notFullyConnectedVertices.RemoveOne();

					notFullyConnectedVertices = new RandomList<Vertex>(notFullyConnectedVertices
						.Where(v =>
						!v.ConnectedEdges.Select(e => e.VertexA).Contains(v1)
						|| !v.ConnectedEdges.Select(e => e.VertexB).Contains(v1)));
					Vertex v2 = notFullyConnectedVertices.RemoveOne();

					Edge edge = new();
					edge.VertexA = v1;
					edge.VertexB = v2;
					edge.MaxLength = EdgeMaxLength;
					edge.Stiffness = EdgeStiffness;
					edges.Add(edge);
					unconnectedVertices.Remove(v2);

					currentAverageConnectedness = (float)edges.Count / (float)vertices.Count;
				}
			}

			graphManager.CreateGraph(vertices, edges);
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
				edge.Stiffness = EdgeStiffness;
				edge.SetInfoObject(Influence);
				edges.Add(edge);
			}

			graphManager.CreateGraph(vertices, edges);
		}

		private void SaveFeelmap()
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