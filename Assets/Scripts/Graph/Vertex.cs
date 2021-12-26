using Assets.Scripts.Graph;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Vertex : MonoBehaviour
{
	public int Id { get; set; }

	[Range(0f, 1f)]
	public float Lightness = 0f;

	public List<Edge> ConnectedEdges = new List<Edge>();
	public int Connectedness => this.ConnectedEdges.Count;
	
	private Rigidbody _rigidbody;
	private Renderer _renderer;

	public TextMeshProUGUI textLabel;

	public IGraphVertexInfoObject InfoObject { get; private set; }

	private bool InfoObjectUpdated;

	public void SetInfoObject(IGraphVertexInfoObject infoObject)
    {
		this.InfoObject = infoObject;
		this.InfoObjectUpdated = true;
	}

	public void Start()
	{
		_rigidbody = this.GetComponent<Rigidbody>();
		_renderer = this.GetComponentInChildren<MeshRenderer>();
	}

    public void Update()
    {
		// Update color depending on Lightness
		this._renderer.material.color = Color.Lerp(Color.black, Color.white, Lightness);

		// Update text when updated
		if (this.InfoObjectUpdated == true)
		{
			this.InfoObjectUpdated = false;
			if (textLabel != null && this.InfoObject != null)
			{
				textLabel.text = this.InfoObject.GetDescription();
			}
		}

		// Position the UI text over the Sphere of the vertex
		Vector3 textPosition = Camera.main.WorldToScreenPoint(this.transform.position);
		textLabel.transform.position = textPosition;

		// Hide Text if the vertex is not visible (else it will show up when it is behind camera at some point)
		if (_renderer.isVisible == false)
		{
			textLabel.enabled = false;
		}
		else
		{
			textLabel.enabled = true;
		}
	}

	public Vector3 GetRepulsionForce(Vertex otherVertex)
	{
		Vector3 difference = otherVertex.transform.position - this.transform.position;
		float distanceSquared = difference.sqrMagnitude;
		if (distanceSquared < 0.0001f) { return Vector3.zero; }
		return (Connectedness / distanceSquared) * difference.normalized;
	}

	public void Move(IEnumerable<Vertex> otherVertices, float deltaTime, float maximumForceMagnitude, float floor, float ceiling)
	{
		// Calculate each force that acts on this vertex.
		// Start with the repulsion forces from the other vertices.
		IEnumerable<Vector3> repulsionForces = otherVertices.Select(v => v.GetRepulsionForce(this));

		// At the same time, the edges will be tugging on this vertex if the maximum length is exceeded.
		IEnumerable<Vector3> pullForces = this.ConnectedEdges.Select(edge => edge.GetPullStrength(this));

		// Then the force for the vertical position.
		// We need to convert the lightness to a value between floor and ceiling.
		float idealFloatHeight = (ceiling - floor) * this.Lightness + floor;
		Vector3 floatForce = Vector3.up * (idealFloatHeight - this.transform.position.y);

		// And we need a bit of wiggle to keep from following too strict a pattern.
		Vector3 wiggleForce = (Vector3.forward * Random.value + Vector3.right * Random.value + Vector3.up * Random.value) * 0.001f;

		// Put all the forces together.
		Vector3 compositeForce = Vector3.zero;
		foreach (Vector3 repulsionForce in repulsionForces)
		{
			compositeForce += repulsionForce;
		}

		foreach (var pullForce in pullForces)
		{
			compositeForce += pullForce;
		}

		compositeForce += floatForce;
		compositeForce += wiggleForce;

		// And scale them to the elapsed time and maximum move speed.
		// this.transform.Translate(Vector3.ClampMagnitude(compositeForce * deltaTime, maximumMoveDelta));
		this._rigidbody.AddForce(Vector3.ClampMagnitude(compositeForce, maximumForceMagnitude));
		//this._rigidbody.AddForce(compositeForce);
	}
}
