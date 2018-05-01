using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

class ImageShare : MonoBehaviour
{
    public void StartShare()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            StartCoroutine(TakeScreenshot());
        }
        else
        {
            Debug.Log("Android only");
        }
    }

    IEnumerator TakeScreenshot()
    {
        yield return new WaitForEndOfFrame();

        string path = Application.persistentDataPath + "/myscore.png";

        //ScreenCapture.CaptureScreenshot("myscore.png");

        // Save your image on designate path
        //byte[] bytes = MyImage.EncodeToPNG();
        //File.WriteAllBytes(path, bytes);

        Texture2D screenImage = new Texture2D(Screen.width, Screen.height);

        //Get Image from screen
        screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenImage.Apply();

        //Convert to png
        byte[] imageBytes = screenImage.EncodeToPNG();

        //System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/GameOverScreenShot");
        //path = Application.persistentDataPath + "/GameOverScreenShot" + "/DiedScreenShot.png";
        System.IO.File.WriteAllBytes(path, imageBytes);

        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        //intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TITLE"), "SleepyHex");
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "Demo");
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "Beat my score!");
        intentObject.Call<AndroidJavaObject>("setType", "image/png");

        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
        AndroidJavaClass fileClass = new AndroidJavaClass("java.io.File");

        AndroidJavaObject fileObject = new AndroidJavaObject("java.io.File", path);// Set Image Path Here
        AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromFile", fileObject);

        // string uriPath =  uriObject.Call("getPath");
        bool fileExist = fileObject.Call<bool>("exists");
        Debug.Log("File exist : " + fileExist);

        // Attach image to intent
        if (fileExist)
        {
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);

            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share Via");

            currentActivity.Call("startActivity", jChooser);
        }
    }
}
