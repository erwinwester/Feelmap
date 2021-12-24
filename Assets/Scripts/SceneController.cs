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
		public Vertex VertexPrefab;
		public Edge EdgePrefab;
		public Transform Container;
		public GraphManager graphManager;

		// Variables that are only evaluated at start.
		[Range(1f, 1000f)]
		public int VerticesToSpawn;
		[Range(1f, 50f)]
		[Tooltip("Not used when the graph is acyclic. Because the graph is always connected.")]
		public float AverageConnectedness;
		public bool Acyclic;
		public float EdgeMaxLength = 10f;
		public float EdgeStiffness = 10f;

		public Feelmap feelmap = new();

		// Start is called before the first frame update
		void Start()
		{
			InitializeGraphRandom();
			//CreateGraph();
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
				Vertex vertex = Instantiate(this.VertexPrefab, this.Container);
				vertex.Lightness = Random.value;
				vertices.Add(vertex);
			}

			// Spawn all edges.
			var unconnectedVertices = new RandomList<Vertex>(vertices);
			var connectedVertices = new RandomList<Vertex>();
			connectedVertices.Add(unconnectedVertices.RemoveOne());

			while (unconnectedVertices.Any())
			{
				Vertex parent = connectedVertices.GetOne();
				Vertex child = unconnectedVertices.RemoveOne();
				edges.Add(SpawnEdge(parent, child));
				connectedVertices.Add(child);
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
					Vertex v1 = notFullyConnectedVertices.RemoveOne();

					notFullyConnectedVertices = new RandomList<Vertex>(notFullyConnectedVertices
						.Where(v =>
						!v.ConnectedEdges.Select(e => e.VertexA).Contains(v1)
						|| !v.ConnectedEdges.Select(e => e.VertexB).Contains(v1)));
					Vertex v2 = notFullyConnectedVertices.RemoveOne();

					edges.Add(SpawnEdge(v1, v2));
					unconnectedVertices.Remove(v2);

					currentAverageConnectedness = (float)edges.Count / (float)vertices.Count;
				}
			}

			graphManager.CreateGraph(edges, vertices);
		}

		private Edge SpawnEdge(Vertex v1, Vertex v2)
		{
			Edge edge = Instantiate(EdgePrefab, Container);
			edge.VertexA = v1;
			edge.VertexB = v2;
			edge.MaxLength = EdgeMaxLength;
			edge.Stiffness = EdgeStiffness;

			v1.ConnectedEdges.Add(edge);
			v2.ConnectedEdges.Add(edge);

			return edge;
		}

		private void ReloadGraph()
		{
			Container.DestroyChildren();
			feelmap = LoadFeelMap();
			// Clear Graph objects

			// Create graph
			CreateGraphFromFeelmap();
		}

		private void CreateGraphFromFeelmap()
		{
			List<Edge> edges = new List<Edge>();
			List<Vertex> vertices = new List<Vertex>();

			foreach (var Subject in feelmap.Subjects)
			{
				Vertex vertex = Instantiate(this.VertexPrefab, this.Container);
				vertex.Lightness = Subject.Energy;
				vertices.Add(vertex);
			}

			foreach (var Influence in feelmap.Influences)
			{
				Edge edge = Instantiate(this.EdgePrefab, this.Container);
				edges.Add(SpawnEdge());
			}

			graphManager.CreateGraph(edges, vertices);
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