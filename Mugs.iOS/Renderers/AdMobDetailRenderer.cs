using Foundation;
using Google.MobileAds;
using Mugs.Ads;
using Mugs.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AdMobDetail), typeof(AdMobDetailRenderer))]
namespace Mugs.iOS.Renderers
{
    [Protocol]
    public class AdMobDetailRenderer : ViewRenderer<AdMobDetail, BannerView>
    {
#if DEBUG
        readonly string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#else
        readonly string adUnitId = "ca-app-pub-1561048054448608/7023558528";
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

        protected override void OnElementChanged(ElementChangedEventArgs<AdMobDetail> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
                SetNativeControl(CreateBannerView());
        }
    }
}
