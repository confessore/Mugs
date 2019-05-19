using Admob.Droid.Renderers;
using Android.Content;
using Android.Gms.Ads;
using Android.Widget;
using Mugs.Ads;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AdMobDetail), typeof(AdMobDetailRenderer))]
namespace Admob.Droid.Renderers
{
    public class AdMobDetailRenderer : ViewRenderer<AdMobDetail, AdView>
    {
        public AdMobDetailRenderer(Context context) : base(context) { }
#if DEBUG
        readonly string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#else
        readonly string adUnitId = "ca-app-pub-1561048054448608/8346085348";
#endif
        readonly AdSize adSize = AdSize.SmartBanner;
        AdView adView;

        AdView CreateAdView()
        {
            if (adView != null)
                return adView;
            adView = new AdView(Context)
            {
                AdSize = adSize,
                AdUnitId = adUnitId,
                LayoutParameters = new LinearLayout.LayoutParams(
                    LayoutParams.WrapContent, LayoutParams.WrapContent)
            };

#if DEBUG
            adView.LoadAd(new AdRequest.Builder().AddTestDevice("04DB1A4AC8F198E3ABC7232881E7DE56").Build());
#else
            adView.LoadAd(new AdRequest.Builder().Build());
#endif
            return adView;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<AdMobDetail> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                CreateAdView();
                SetNativeControl(adView);
            }
        }
    }
}