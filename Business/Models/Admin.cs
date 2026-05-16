namespace FitTrack.Models;

public class Admin : Person
{
    public Admin()
    {
        Role = "Admin";
    }

    public override string GetRoleLabel()
    {
        return "Admin";
    }

    public override string ToString()
    {
        return base.ToString() + " | Full System Access";
    }
}
