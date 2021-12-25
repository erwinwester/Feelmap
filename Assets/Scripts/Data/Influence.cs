using Assets.Scripts.Graph;
using System;

namespace Assets.Scripts
{
    public class Influence : IGraphEdgeInfoObject
	{
		public int Id { get; set; }

		public string Description { get; set; }
		public Subject FromSubject { get; set; }
		public Subject ToSubject { get; set; }
		public float EnergyFactor { get; set; }

        string IGraphEdgeInfoObject.GetDescription()
        {
            return Description;
        }

        float IGraphEdgeInfoObject.GetEnergyFactor()
        {
            return EnergyFactor;
        }

        void IGraphEdgeInfoObject.SetDescription(string description)
        {
            this.Description = description;
        }

        void IGraphEdgeInfoObject.SetEnergyFactor(float energyFactor)
        {
            this.EnergyFactor = energyFactor;
        }
    }
}
