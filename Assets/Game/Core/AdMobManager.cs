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

        var playCount = PlayerPrefs.GetInt("GlobalPlayCount", 0);

        if (playCount <= 25 || PlayerPrefs.GetInt("PlayedTutorial", 0) == 0) //don't block tutorial
        {
            return;
        }


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

        RequestBanner();
    }

    private void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-7441401095869420/8634562987";
        //string adUnitId = "ca-app-pub-3940256099942544/6300978111"; //for test
#elif UNITY_IPHONE
                    string adUnitId = "ca-app-pub-7441401095869420/9429217018";
#else
                    string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

        /*
        // Called when an ad request has successfully loaded.
        bannerView.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        bannerView.OnAdOpening += HandleOnAdOpened;
        // Called when the user returned from the app after an ad click.
        bannerView.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        bannerView.OnAdLeavingApplication += HandleOnAdLeavingApplication;
        */

        // Create an empty ad request.
        AdRequest request 
            = new AdRequest.Builder()        
            .Build(); //.AddTestDevice("ECCCC4AB628B5A5E1FFB38969709E24D")

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }


}

