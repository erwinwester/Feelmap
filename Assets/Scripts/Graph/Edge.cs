using Assets.Scripts.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
	public Vertex VertexA;
	public Vertex VertexB;
	public float MaxLength = 1f;
	public float Stiffness = 10f;

	public IGraphEdgeInfoObject InfoObject { get; private set; } 

	public void SetInfoObject(IGraphEdgeInfoObject infoObject)
    {
		this.InfoObject = infoObject;
    }

	public Vector3 GetPullStrength(Vertex onVertex)
	{
		Vertex otherVertex;
		if (onVertex == VertexA)
		{
			otherVertex = VertexB;
		}
		else if (onVertex == VertexB)
		{
			otherVertex = VertexA;
		}
		else
		{
			return Vector3.zero;
		}

		Vector3 distanceVector = otherVertex.transform.position - onVertex.transform.position;
		float distance = distanceVector.magnitude;
		if (distance < this.MaxLength)
		{
			return Vector3.zero;
		}

		return distanceVector.normalized * (distance - this.MaxLength) * this.Stiffness;
	}
}
