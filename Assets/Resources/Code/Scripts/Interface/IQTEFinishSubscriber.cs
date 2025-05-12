public interface IQTEFinishSubscriber : IGlobalSubscriber
{
    void OnQTEFinished(bool success);
}