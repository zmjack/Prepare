namespace Prepare
{
    public interface IDesignTimePrepareFactory
    {
        void Prepare(PrepareProject project, string[] args);
    }
}
