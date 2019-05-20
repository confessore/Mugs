using Foundation;
using Google.MobileAds;
using Mugs.Ads;
using Mugs.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AdMobMain), typeof(AdMobMainRenderer))]
namespace Mugs.iOS.Renderers
{
    [Protocol]
    public class AdMobMainRenderer : ViewRenderer<AdMobMain, BannerView>
    {
#if DEBUG
        readonly string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#else
        readonly string adUnitId = "ca-app-pub-1561048054448608/7420688939";
#endif

        private BannerView CreateBannerView()
        {
            var bannerView = new BannerView(AdSizeCons.SmartBannerPortrait)
            {
                AdUnitID = adUnitId,
                RootViewController = GetVisibleViewController()
            };

            bannerView.LoadRequest(GetRequest());

            Request GetRequest()
            {
                var request = Request.GetDefaultRequest();
                return request;
            }

            return bannerView;
        }

        UIViewController GetVisibleViewController()
        {
            foreach (var window in UIApplication.SharedApplication.Windows)
                if (window.RootViewController != null)
                    return window.RootViewController;
            return null;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<AdMobMain> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
                SetNativeControl(CreateBannerView());
        }
    }
}
