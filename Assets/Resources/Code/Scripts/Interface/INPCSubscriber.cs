
public interface INPCSubscriber : IGlobalSubscriber
{
    bool isFighting { get;}
    void Attack(int damage);
    void GetDamage(int damage);
}
