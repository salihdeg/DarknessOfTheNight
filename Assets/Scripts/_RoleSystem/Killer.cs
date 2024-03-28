public class Killer : Role<ITratior>
{
    internal override void Start()
    {
        role = Roles.Killer;
        base.Start();
    }
}