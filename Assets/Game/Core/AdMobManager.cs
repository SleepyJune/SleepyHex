using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using GoogleMobileAds.Api;

public class AdMobManager : MonoBehaviour
{
    private BannerView bannerView;

    public static AdMobManager instance;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this);


#if UNITY_ANDROID
        string appId = "ca-app-pub-7441401095869420~6580101511";
        //string appId = "ca-app-pub-3940256099942544~3347511713"; //for test
#elif UNITY_IPHONE
                string appId = "ca-app-pub-7441401095869420~6580101511";
#else
                string appId = "unexpected_platform";
#endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);

        if (PlayerPrefs.GetInt("PlayedTutorial", 0) > 0) //don't block tutorial
        {
            RequestBanner();
        }
    }

    private void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-7441401095869420/9429217018";
        //string adUnitId = "ca-app-pub-3940256099942544/6300978111"; //for test
#elif UNITY_IPHONE
                    string adUnitId = "ca-app-pub-7441401095869420/9429217018";
#else
                    string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }
}

