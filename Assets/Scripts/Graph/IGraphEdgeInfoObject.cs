namespace Assets.Scripts.Graph
{
    public interface IGraphEdgeInfoObject
    {
        string GetDescription();
        float GetEnergyFactor();
        void SetDescription(string description);
        void SetEnergyFactor(float energyFactor);
    }
}
