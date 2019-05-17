namespace Mugs.Models
{
    public enum MenuItemType
    {
        GadsdenAL,
        DaytonaFL,
        GainesvilleFL,
        JacksonvilleFL,
        LakelandFL,
        LeesburgFL,
        OcalaFL,
        PanamaCityFL,
        SarasotaFL,
        StAugustineFL,
        WestPalmBeachFL,
        WinterHavenFL,
        AugustaGA,
        TopekaKS,
        HoumaLA,
        BurlingtonNC,
        GastoniaNC,
        HendersonvilleNC,
        JacksonvilleNC,
        KinstonNC,
        NewBernNC,
        ShelbyNC,
        WilmingtonNC,
        EriePA,
        BlufftonSC,
        SpartanburgSC,
        AustinTX,
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
