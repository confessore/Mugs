using Mugs.Models;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace Mugs.Views
{
    [DesignTimeVisible(true)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;
        public MenuPage()
        {
            InitializeComponent();

            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem { Id = MenuItemType.GadsdenAL, Title = "Gadsden, AL" },
                new HomeMenuItem { Id = MenuItemType.DaytonaFL, Title = "Daytona, FL" },
                new HomeMenuItem { Id = MenuItemType.GainesvilleFL, Title = "Gainesville, FL"},
                new HomeMenuItem { Id = MenuItemType.JacksonvilleFL, Title = "Jacksonville, FL"},
                new HomeMenuItem { Id = MenuItemType.LakelandFL, Title = "Lakeland, FL" },
                new HomeMenuItem { Id = MenuItemType.LeesburgFL, Title = "Leesburg, FL" },
                new HomeMenuItem { Id = MenuItemType.OcalaFL, Title = "Ocala, FL" },
                new HomeMenuItem { Id = MenuItemType.PanamaCityFL, Title = "Panama City, FL" },
                new HomeMenuItem { Id = MenuItemType.SarasotaFL, Title = "Sarasota, FL" },
                new HomeMenuItem { Id = MenuItemType.StAugustineFL, Title = "St. Augustine, FL" },
                new HomeMenuItem { Id = MenuItemType.WestPalmBeachFL, Title = "West Palm Beach, FL" },
                new HomeMenuItem { Id = MenuItemType.WinterHavenFL, Title = "Winter Haven, FL" },
                new HomeMenuItem { Id = MenuItemType.AugustaGA, Title = "Augusta, GA" },
                new HomeMenuItem { Id = MenuItemType.TopekaKS, Title = "Topeka, KS" },
                new HomeMenuItem { Id = MenuItemType.HoumaLA, Title = "Houma, LA" },
                new HomeMenuItem { Id = MenuItemType.BurlingtonNC, Title = "Burlington, NC" },
                new HomeMenuItem { Id = MenuItemType.GastoniaNC, Title = "Gastonia, NC" },
                new HomeMenuItem { Id = MenuItemType.HendersonvilleNC, Title = "Hendersonville, NC" },
                new HomeMenuItem { Id = MenuItemType.JacksonvilleNC, Title = "Jacksonville, NC" },
                new HomeMenuItem { Id = MenuItemType.KinstonNC, Title = "Kinston, NC" },
                new HomeMenuItem { Id = MenuItemType.NewBernNC, Title = "New Bern, NC" },
                new HomeMenuItem { Id = MenuItemType.ShelbyNC, Title = "Shelby, NC" },
                new HomeMenuItem { Id = MenuItemType.WilmingtonNC, Title = "Wilmington, NC" },
                new HomeMenuItem { Id = MenuItemType.EriePA, Title = "Erie, PA" },
                new HomeMenuItem { Id = MenuItemType.BlufftonSC, Title = "Bluffton, SC" },
                new HomeMenuItem { Id = MenuItemType.SpartanburgSC, Title = "Spartanburg, SC" },
                new HomeMenuItem { Id = MenuItemType.AustinTX, Title = "Austin, TX" },
            };

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                await RootPage.NavigateFromMenu(id);
            };
        }
    }
}
