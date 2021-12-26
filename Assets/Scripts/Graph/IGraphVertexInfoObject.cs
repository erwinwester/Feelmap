namespace Assets.Scripts.Graph
{
    public interface IGraphVertexInfoObject
    {
        bool IsRoot();
        string GetDescription();
        float GetEnergy();
        float GetFeel();
        float GetDistance();
        void SetDescription(string description);
        void SetEnergy(float energy);
        void SetFeel(float feel);
        void SetDistance(float distance);
    }
}