using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Unity.VisualScripting;

using UnityEngine;

using static Randomization;

namespace Assets.Scripts
{
	public class GraphManager : MonoBehaviour
	{
		// Variables that may change at runtime.
		public float GraphCeiling = 10f;
		public float GraphFloor = -10f;
		public float RepulsionForceScale = 1f;
		public float PullForceScale = 1f;
		public float MaxForceMagnitude = 10f;

		// updated in Update()
		public Vector3 CenterOfGraph = Vector3.zero;

		// Internal variables.
		private List<Edge> edges = new List<Edge>();
		private List<Vertex> vertices = new List<Vertex>();

		// Update is called once per frame
		public void Update()
		{
			float deltaTime = Time.deltaTime;
			int vertexCount = 0;
			Vector3 vertexPositionSum = Vector3.zero;

			foreach (var vertex in this.vertices)
			{
				vertex.Move(this.vertices.Where(v => v != vertex), deltaTime, this.MaxForceMagnitude * deltaTime, this.GraphFloor, this.GraphCeiling);
				vertexPositionSum += vertex.transform.position;
				vertexCount++;
			}

			CenterOfGraph = (vertexCount > 0) ? vertexPositionSum / vertexCount : Vector3.zero;

			foreach (var edge in this.edges)
			{
				edge.transform.position = edge.VertexA.transform.position;
				edge.transform.LookAt(edge.VertexB.transform);
				edge.transform.localScale = new Vector3(1, 1, Vector3.Distance(edge.VertexA.transform.position, edge.VertexB.transform.position));
			}
		}

		public void CreateGraph(List<Edge> newEdges, List<Vertex> newVertices)
		{

		}

		public void ClearGraph()
		{
			edges.Clear();
			vertices.Clear();
		}
	}
}