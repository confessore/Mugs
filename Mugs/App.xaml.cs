using Mugs.Services;
using Mugs.Views;
using Xamarin.Forms;

namespace Mugs
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<HtmlParser>();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
