using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class SpeechBubbleHelper : MonoBehaviour
{
    public Text speechBubbleText;

    public void SetText(string text)
    {
        speechBubbleText.text = text;
    }
}
