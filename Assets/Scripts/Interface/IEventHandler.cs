namespace ReusableScripts.Interface
{
    /// <summary>
    /// Base interface for event handlers
    /// </summary>
    public interface IEventHandler
    {
        void Handle(IGameEvent gameEvent);
    }
}