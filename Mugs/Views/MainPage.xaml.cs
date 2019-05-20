using Mugs.Items;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mugs.Views
{
    [DesignTimeVisible(true)]
    public partial class MainPage : MasterDetailPage
    {
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;

            MenuPages.Add((int)MenuItemType.OcalaFL, (NavigationPage)Detail);
        }

        public async Task NavigateFromMenu(int id)
        {
            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.GadsdenAL:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.gadsdentimes.com/")));
                        break;
                    case (int)MenuItemType.DaytonaFL:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://www.daytonamugshots.com/")));
                        break;
                    case (int)MenuItemType.GainesvilleFL:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://www.mugshotsgainesville.com/")));
                        break;
                    case (int)MenuItemType.JacksonvilleFL:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.jacksonville.com/")));
                        break;
                    case (int)MenuItemType.LakelandFL:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.theledger.com/")));
                        break;
                    case (int)MenuItemType.LeesburgFL:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.dailycommercial.com/")));
                        break;
                    case (int)MenuItemType.OcalaFL:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://www.mugshotsocala.com/")));
                        break;
                    case (int)MenuItemType.PanamaCityFL:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.newsherald.com/")));
                        break;
                    case (int)MenuItemType.SarasotaFL:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://www.sarasotamugshots.com/")));
                        break;
                    case (int)MenuItemType.StAugustineFL:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.staugustine.com/")));
                        break;
                    case (int)MenuItemType.WestPalmBeachFL:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.palmbeachpost.com/")));
                        break;
                    case (int)MenuItemType.WinterHavenFL:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.theledger.com/")));
                        break;
                    case (int)MenuItemType.AugustaGA:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.augustachronicle.com/")));
                        break;
                    case (int)MenuItemType.TopekaKS:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.cjonline.com/")));
                        break;
                    case (int)MenuItemType.HoumaLA:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.houmatoday.com/")));
                        break;
                    case (int)MenuItemType.BurlingtonNC:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.thetimesnews.com/")));
                        break;
                    case (int)MenuItemType.GastoniaNC:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.gastongazette.com/")));
                        break;
                    case (int)MenuItemType.HendersonvilleNC:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.blueridgenow.com/")));
                        break;
                    case (int)MenuItemType.JacksonvilleNC:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.jdnews.com/")));
                        break;
                    case (int)MenuItemType.KinstonNC:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.kinston.com/")));
                        break;
                    case (int)MenuItemType.NewBernNC:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.newbernsj.com/")));
                        break;
                    case (int)MenuItemType.ShelbyNC:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.shelbystar.com/")));
                        break;
                    case (int)MenuItemType.WilmingtonNC:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://www.wilmingtonmugshots.com/")));
                        break;
                    case (int)MenuItemType.EriePA:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.goerie.com/")));
                        break;
                    case (int)MenuItemType.BlufftonSC:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.blufftontoday.com/")));
                        break;
                    case (int)MenuItemType.SpartanburgSC:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.goupstate.com/")));
                        break;
                    case (int)MenuItemType.AustinTX:
                        MenuPages.Add(id, new NavigationPage(new InmatesPage("http://mugshots.statesman.com/")));
                        break;
                }
            }

            var newPage = MenuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}
