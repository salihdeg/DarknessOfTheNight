public class Villager :Role<IInnocent>
{
    internal override void Start()
    {
        role = Roles.Villager;
        base.Start();
    }
}
