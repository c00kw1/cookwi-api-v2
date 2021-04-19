namespace Cookwi.Common.Models
{
    public enum Role
    {
        Admin,
        User
    }

    public enum Gender
    {
        Male,
        Female,
        Neutral
    }

    public enum TribeAccess
    {
        Write,
        Read
    }

    public enum UnitType
    {
        Liquid, // L, cL, mL
        Weight, // kg, g
        Size, // cm
        Container, // spoon, glass
        Other
    }
}
