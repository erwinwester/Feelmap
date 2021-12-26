using Assets.Scripts.Graph;

namespace Assets.Scripts
{
    public class Subject : IGraphVertexInfoObject
    {
        public int Id { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Kost energie, kost geen energie, levert energie op.
        /// Getal -1 < n< 1
        /// Energy wordt beïnvloed door Feel en Distance (factor).
        /// Deze waarde kan alleen ingevuld worden als er geen influences zijn ingesteld.
        /// De energy van dit Subject wordt berekend op basis van de energie die via influences van andere subjecten afkomt.
        /// Men moet door instellen van influences met andere Subjects bepalen of men hier energie van krijgt of dat dit energie kost.
        /// </summary>
        public float Energy { get; set; }

        /// <summary>
        /// Onverschillig, Bevlogen of Gepassioneerd
        /// Getal tussen 0 en 1?
        /// </summary>
        public float Feel { get; set; }

        /// <summary>
        /// Ver weg of Dichtbij of Kern
        /// Getal tussen 0 en 1?
        /// </summary>
        public float Distance { get; set; }
        public bool Root { get; set; }

        string IGraphVertexInfoObject.GetDescription()
        {
            return Description;
        }

        float IGraphVertexInfoObject.GetDistance()
        {
            return Distance;
        }

        float IGraphVertexInfoObject.GetEnergy()
        {
            return Energy;
        }

        float IGraphVertexInfoObject.GetFeel()
        {
            return Feel;
        }

        bool IGraphVertexInfoObject.IsRoot()
        {
            return Root;
        }

        void IGraphVertexInfoObject.SetDescription(string description)
        {
            this.Description = description;
        }

        void IGraphVertexInfoObject.SetDistance(float distance)
        {
            this.Distance = distance;
        }

        void IGraphVertexInfoObject.SetEnergy(float energy)
        {
            this.Energy = energy;
        }

        void IGraphVertexInfoObject.SetFeel(float feel)
        {
            this.Feel = feel;
        }
    }
}
