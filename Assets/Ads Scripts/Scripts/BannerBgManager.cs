using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class BannerBgManager : MonoBehaviour
{
    [Header("For Notch Devices")]
    public Crystal.SafeArea ScreenSafeArea;
    public Rect SafeAreaRect;
    private Texture2D adaptiveTexture;
    private Texture2D bannerTexture;
    private Texture2D nativeTexture;

    [Header("Banner Color")]
    public Color BannerColor;
    public Color bannerBorderColor;

    [Header("Adaptive Banner Color")]
    public Color adaptiveColor;
    public Color adaptiveBorderColor;

    [Header("Native Banner Color")]
    public Color nativeColor;
    public Color nativeBorderColor;


    [Header("Ad Label Color")]
    public Color adLabelColor;

    private bool showBannerBg = false;
    private bool showAdaptiveBg = false;
    private bool showNativeBg = false;
    private float bannerWidht;
    private float bannerHeight;
    private float adaptiveBannerWidht;
    private float adaptiveBannerHeight;
    private float nativeWidht;
    private float nativeHeight;
    private float defaultBannerWidht;
    private float defaultBannerHeight;
    private float defaultAdaptiveWidht;
    private float defaultAdaptiveHeight;
    private float defaultNativeWidht;
    private float defaultNativeHeight;
    public int borderWidth = 3;
    private int label_Widht = 20, label_Height = 20, label_FontSize = 15; //  label settings dont change
    [System.Serializable]
    public class BannerPositionValues
    {
        public float posX;
        public float posY;
        public float labelX;
        public float labelY;

    }
    private BannerPositionValues bannerPos, adaptivePos, nativePos;


    #region Banner

    public void SetDefaultBannerSize()
    {
        if (AdsManager.Instance.HasSmartbanner)
            defaultBannerWidht = Screen.width;
        else
            defaultBannerWidht = (320 * (float)Screen.dpi) / 160;

        defaultBannerHeight = (55 * (float)Screen.dpi) / 160;
        bannerWidht = defaultBannerWidht;
        bannerHeight = defaultBannerHeight;
    }

    public void SetBannerSize(BannerView view)
    {
        bannerTexture = null;
        bannerWidht = view.GetWidthInPixels();
        bannerHeight = view.GetHeightInPixels();
        if (bannerWidht <= 0 || bannerHeight <= 0)
            SetDefaultBannerSize();

    }


    public void DestroyBannerBG()
    {
        bannerTexture = null;
        bannerWidht = bannerHeight = 0;
    }

    #endregion

    #region Adaptive Banner

    public void SetDefaultAdaptiveBannerSize()
    {
        Debug.Log("1");
        defaultAdaptiveWidht = Screen.width;
        defaultAdaptiveHeight = (35 * (float)Screen.dpi) / 160;
        adaptiveBannerWidht = defaultAdaptiveWidht;
        adaptiveBannerHeight = defaultAdaptiveHeight;
    }

    public void SetAdaptivBannerSize(BannerView view)
    {
        Debug.Log("2");
        adaptiveTexture = null;
        adaptiveBannerWidht = view.GetWidthInPixels();

        adaptiveBannerHeight = view.GetHeightInPixels();

        if (adaptiveBannerWidht <= 0 || adaptiveBannerHeight <= 0)
            SetDefaultAdaptiveBannerSize();

    }


    public void DestroyAdaptiveBannerBG()
    {
        adaptiveTexture = null;
        adaptiveBannerWidht = adaptiveBannerHeight = 0;
    }


    #endregion

    #region Native Banner


    public void SetDefaultNativeSize()
    {
        if (AdsManager.Instance.HasSmartbanner)
            defaultNativeWidht = Screen.width;
        else
            defaultNativeWidht = (300 * (float)Screen.dpi) / 160;

        defaultNativeHeight = (255 * (float)Screen.dpi) / 160;
        nativeWidht = defaultNativeWidht;
        nativeHeight = defaultNativeHeight;
    }

    public void SetNativeSize(BannerView view)
    {
        nativeTexture = null;
        nativeWidht = view.GetWidthInPixels();
        nativeHeight = view.GetHeightInPixels();
        if (nativeWidht <= 0 || nativeHeight <= 0)
            SetDefaultNativeSize();

    }


    public void DestroyNativeG()
    {
        nativeTexture = null;
        nativeWidht = nativeHeight = 0;
    }
    #endregion

    public void OnGUI()
    {
        if (AdsManager.Instance.showBannerBackground == false)
            return;

        showBannerBg = bannerWidht > 0 && bannerHeight > 0;
        showAdaptiveBg = adaptiveBannerWidht > 0 && adaptiveBannerHeight > 0;
        showNativeBg = nativeWidht > 0 && nativeHeight > 0;
        SafeAreaRect = ScreenSafeArea.GetSafeArea();
        if (showBannerBg)
        {
            // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
            if (bannerTexture == null)
            {
                bannerWidht += 30;
                bannerHeight += 20;
                bannerTexture = new Texture2D((int)bannerWidht, (int)bannerHeight, TextureFormat.ARGB32, false);


                for (int y = 0; y < bannerTexture.height; y++)
                {
                    for (int x = 0; x < bannerTexture.width; x++)
                    {

                        bannerTexture.SetPixel(x, y, BannerColor);
                    }
                }
                var borderColor = bannerBorderColor;
                for (int x = 0; x < bannerTexture.width; x++)
                {
                    for (int y = 0; y < bannerTexture.height; y++)
                    {
                        if (x < borderWidth || x > bannerTexture.width - 1 - borderWidth) bannerTexture.SetPixel(x, y, borderColor);
                        else if (y < borderWidth || y > bannerTexture.height - 1 - borderWidth) bannerTexture.SetPixel(x, y, borderColor);
                    }
                }
#if UNITY_IOS
                if (bannerTexture)
                    bannerPos = GetBannerPosition(AdsManager.Instance.smallBanneradPosition, bannerTexture, true, AdsManager.BannerAdTypes.BANNER);
#else
                if (bannerTexture)
                    bannerPos = GetBannerPosition(AdsManager.Instance.smallBanneradPosition, bannerTexture, true, AdsManager.BannerAdTypes.BANNER);
#endif
                bannerTexture.Apply();
            }
            GUIStyle myStyle = new GUIStyle();
            myStyle.fontStyle = FontStyle.Bold;
            myStyle.normal.textColor = adLabelColor;
            myStyle.fontSize = label_FontSize;
#if UNITY_IOS
            //IOS Small Banner Does Not Follow Safe Area, So Not Needed Here
            GUI.DrawTexture(new Rect(bannerPos.posX, bannerPos.posY, bannerTexture.width, bannerTexture.height), bannerTexture, ScaleMode.StretchToFill, false);
            GUI.Label(new Rect(bannerPos.labelX, bannerPos.labelY, label_Widht, label_Height), "AD", myStyle);
#elif UNITY_ANDROID

            // TODO Changed Later Cause in Android For Notch Devices To Adapt to Banner Position According To The Safe Area,
            GUI.DrawTexture(new Rect((bannerPos.posX + (Screen.width - SafeAreaRect.width)), bannerPos.posY, bannerTexture.width, bannerTexture.height), bannerTexture, ScaleMode.StretchToFill, false);
            GUI.Label(new Rect((bannerPos.labelX + (Screen.width - SafeAreaRect.width)), bannerPos.labelY+4, label_Widht, label_Height), "AD", myStyle);
#endif
            //-160,+20
        }
        if (showAdaptiveBg)
        {
            if (adaptiveTexture == null)
            {
                adaptiveBannerWidht = Screen.width;
                adaptiveBannerHeight += 30;
                adaptiveTexture = new Texture2D((int)adaptiveBannerWidht, (int)adaptiveBannerHeight, TextureFormat.ARGB32, false);

                for (int y = 0; y < adaptiveTexture.height; y++)
                {
                    for (int x = 0; x < adaptiveTexture.width; x++)
                    {

                        adaptiveTexture.SetPixel(x, y, adaptiveColor);
                    }
                }

                var borderColor = adaptiveBorderColor;
                for (int x = 0; x < adaptiveTexture.width; x++)
                {
                    for (int y = 0; y < adaptiveTexture.height; y++)
                    {
                        //if (x < borderWidth || x > Screen.width - 1 - borderWidth) adaptiveTexture.SetPixel(x, y, borderColor);
                        adaptiveTexture.SetPixel(0, y, borderColor);
                        adaptiveTexture.SetPixel(Screen.width - 1, y, borderColor);
                        adaptiveTexture.SetPixel(Screen.width, y, borderColor);
                        adaptiveTexture.SetPixel(Screen.width, y, borderColor);

                        if (y < borderWidth || y > adaptiveTexture.height - 1 - borderWidth) adaptiveTexture.SetPixel(x, y, borderColor);
                    }
                }
                if (adaptiveTexture)
                    adaptivePos = GetBannerPosition(AdsManager.Instance.adeptiveBanneradPosition, adaptiveTexture, true, AdsManager.BannerAdTypes.ADAPTIVE);
                adaptiveTexture.Apply();
            }
            GUIStyle myStyle = new GUIStyle();
            myStyle.fontSize = label_FontSize;
            myStyle.fontStyle = FontStyle.Bold;
            myStyle.normal.textColor = adLabelColor;
#if UNITY_IOS
            //IOS Adaptive Banner Does Not Follow Safe Area, So Not Needed Here
            //GUI.DrawTexture(new Rect((adaptivePos.posX + (Screen.width - SafeAreaRect.width)), adaptivePos.posY, Screen.width, adaptiveBannerHeight), adaptiveTexture, ScaleMode.StretchToFill, false);
            //GUI.Label(new Rect((adaptivePos.labelX + (Screen.width - SafeAreaRect.width)), adaptivePos.labelY, label_Widht, label_Height), "AD", myStyle);
#elif UNITY_ANDROID
            GUI.DrawTexture(new Rect(adaptivePos.posX, adaptivePos.posY, Screen.width, adaptiveBannerHeight), adaptiveTexture, ScaleMode.StretchToFill, false);
            GUI.Label(new Rect(adaptivePos.labelX, adaptivePos.labelY, label_Widht, label_Height), "AD", myStyle);
#endif

        }
        if (showNativeBg)
        {
            // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
            if (nativeTexture == null)
            {
                nativeWidht += 30;
                nativeHeight += 20;
                nativeTexture = new Texture2D((int)nativeWidht, (int)nativeHeight, TextureFormat.ARGB32, false);
                for (int y = 0; y < nativeTexture.height; y++)
                {
                    for (int x = 0; x < nativeTexture.width; x++)
                    {
                        nativeTexture.SetPixel(x, y, nativeColor);
                    }
                }
                var borderColor = nativeBorderColor;
                for (int x = 0; x < nativeTexture.width; x++)
                {
                    for (int y = 0; y < nativeTexture.height; y++)
                    {
                        if (x < borderWidth || x > nativeTexture.width - 1 - borderWidth) nativeTexture.SetPixel(x, y, borderColor);
                        else if (y < borderWidth || y > nativeTexture.height - 1 - borderWidth) nativeTexture.SetPixel(x, y, borderColor);
                    }
                }
#if UNITY_IOS
                if (nativeTexture)
                    nativePos = GetBannerPosition(AdsManager.Instance.bigBanneradPosition, nativeTexture, true, AdsManager.BannerAdTypes.NATIVE);
#else
                if (nativeTexture)
                    nativePos = GetBannerPosition(AdsManager.Instance.bigBanneradPosition, nativeTexture, true, AdsManager.BannerAdTypes.NATIVE);
#endif
                nativeTexture.Apply();
            }

            GUIStyle myStyle = new GUIStyle();
            myStyle.fontSize = label_FontSize;
            myStyle.fontStyle = FontStyle.Bold;
            myStyle.normal.textColor = adLabelColor;
#if UNITY_IOS
            //TODO Changed Later, Not Needed in IOS
            //GUI.DrawTexture(new Rect((nativePos.posX + (Screen.width - SafeAreaRect.width)), nativePos.posY, nativeTexture.width, nativeTexture.height), nativeTexture, ScaleMode.StretchToFill, false);
            //GUI.Label(new Rect((nativePos.labelX + (Screen.width - SafeAreaRect.width)), nativePos.labelY, label_Widht, label_Height), "AD", myStyle);
#elif UNITY_ANDROID
            GUI.DrawTexture(new Rect(nativePos.posX, nativePos.posY, nativeTexture.width, nativeTexture.height), nativeTexture, ScaleMode.StretchToFill, false);
            GUI.Label(new Rect(nativePos.labelX-350, nativePos.labelY-4, label_Widht, label_Height), "ADVERTISEMENT", myStyle);
#endif

            //-150.-20
        }

    }

    BannerPositionValues GetBannerPosition(AdPosition pos, Texture2D texture, bool applySafeArea, AdsManager.BannerAdTypes type)
    {
        BannerPositionValues values = new BannerPositionValues();
        switch (pos)
        {
            case AdPosition.Top:
                if (applySafeArea)
                {
                    if (Screen.orientation == ScreenOrientation.Portrait)
                    {
                        values.posX = (Screen.width / 2) - (texture.width / 2);
                        values.posY = Screen.height - Screen.height + LastSafeArea.y;
                    }
                    else
                    {

                        values.posX = (Screen.width / 2) - (texture.width / 2);
                        values.posY = Screen.height - Screen.height;
                        values.posY -= 10;

                    }
                }
                else
                {

                    values.posX = (Screen.width / 2) - (texture.width / 2);
                    values.posY = Screen.height - Screen.height;
                }
                if (type == AdsManager.BannerAdTypes.BANNER && AdsManager.Instance.HasSmartbanner)
                {
                    values.labelX = (values.posX + texture.width) - (label_Widht + borderWidth+label_FontSize);
                    values.labelY = values.posY + texture.height - (borderWidth + label_FontSize);
                }
                else
                {
                    values.labelX = (values.posX + texture.width) - label_Widht;
                    values.labelY = values.posY + (texture.height - (borderWidth + label_FontSize));
                }
                    break;

            case AdPosition.TopLeft:
                if (applySafeArea)
                {
                    if (Screen.orientation == ScreenOrientation.Portrait)
                    {
                        values.posX = Screen.width - Screen.width;
                        values.posY = Screen.height - Screen.height + LastSafeArea.y;
                    }
                    else
                    {
                        values.posX = LastSafeArea.x - 10;
                        values.posY = 0;
                        values.posY -= 10;

                    }
                }
                else
                {
                    values.posX = Screen.width - Screen.width;
                    values.posY = Screen.height - Screen.height;
                }
                values.labelX = (values.posX + texture.width) - label_Widht;
                values.labelY = values.posY + (texture.height - (borderWidth + 12));
                break;

            case AdPosition.TopRight:
                if (applySafeArea)
                {
                    if (Screen.orientation == ScreenOrientation.Portrait)
                    {
                        values.posX = Screen.width - texture.width;
                        values.posY = Screen.height - Screen.height + LastSafeArea.y;
                    }
                    else
                    {
                        values.posX = Screen.width - texture.width - LastSafeArea.x;
                        values.posX += 10;
                        values.posY = Screen.height - Screen.height;
                        values.posY -= 10;
                    }
                }
                else
                {
                    values.posX = Screen.width - texture.width;
                    values.posY = Screen.height - Screen.height;
                }
                values.labelX = values.posX + borderWidth;
                values.labelY = values.posY + (texture.height - (borderWidth + label_FontSize));
                break;

            case AdPosition.Center:

                if (applySafeArea)
                {
                    if (Screen.orientation == ScreenOrientation.Portrait)
                    {
                        values.posX = (Screen.width / 2) - (texture.width / 2);
                        values.posY = (Screen.height / 2) - (texture.height / 2) + LastSafeArea.y;
                    }
                    else
                    {

                        values.posX = (Screen.width / 2) - (texture.width / 2);
                        values.posY = Screen.height / 2 - (texture.height / 2) - (LastSafeArea.y / 2);
                    }
                }
                else
                {

                    values.posX = (Screen.width / 2) - (texture.width / 2);
                    values.posY = (Screen.height / 2) - (texture.height / 2);

                }
                if (type == AdsManager.BannerAdTypes.BANNER && AdsManager.Instance.HasSmartbanner)
                {
                    values.labelX = (values.posX + texture.width) - (label_Widht + borderWidth + label_FontSize);
                    values.labelY = values.posY + texture.height - (borderWidth + label_FontSize);
                }
                else
                {
                    values.labelX = (values.posX + texture.width) - label_Widht;
                    values.labelY = values.posY + (texture.height - (borderWidth + label_FontSize));
                }
                break;

            case AdPosition.Bottom:
                if (applySafeArea)
                {
                    if (Screen.orientation == ScreenOrientation.Portrait)
                    {
                        values.posX = (Screen.width / 2) - (texture.width / 2);
                        values.posY = Screen.height - texture.height - LastSafeArea.y;
                    }
                    else
                    {
                        values.posX = (Screen.width / 2) - (texture.width / 2);
                        values.posY = Screen.height - texture.height - LastSafeArea.y;
                        if (type == AdsManager.BannerAdTypes.NATIVE)
                            values.posY += 10;
                        else
                            values.posY += 5;
                    }
                }
                else
                {

                    values.posX = (Screen.width / 2) - (texture.width / 2);
                    values.posY = Screen.height - texture.height;
                }
                if (type == AdsManager.BannerAdTypes.BANNER && AdsManager.Instance.HasSmartbanner)
                {
                    values.labelX = (values.posX + texture.width) - (label_Widht + borderWidth + label_FontSize);
                    values.labelY = values.posY + borderWidth;
                }
                else
                {
                    values.labelX = (values.posX + texture.width) - label_Widht;
                    values.labelY = values.posY + borderWidth;
                }
                break;

            case AdPosition.BottomLeft:
                if (applySafeArea)
                {
                    if (Screen.orientation == ScreenOrientation.Portrait)
                    {
                        values.posX = Screen.width - Screen.width;
                        values.posY = Screen.height - texture.height + LastSafeArea.y;
                    }
                    else
                    {
                        values.posX = Screen.width - Screen.width + LastSafeArea.x;
                        values.posX -= 10;
                        values.posY = Screen.height - texture.height - LastSafeArea.y;
                        if (type == AdsManager.BannerAdTypes.NATIVE)
                            values.posY += 10;
                        else
                            values.posY += 5;
                    }

                }
                else
                {
                    values.posX = Screen.width - Screen.width;
                    values.posY = Screen.height - texture.height - LastSafeArea.y;
                }
                values.labelX = (values.posX + texture.width) - label_Widht;
                values.labelY = values.posY + borderWidth;

                break;
            case AdPosition.BottomRight:
                if (applySafeArea)
                {
                    if (Screen.orientation == ScreenOrientation.Portrait)
                    {
                        values.posX = Screen.width - texture.width;
                        values.posY = Screen.height - texture.height - LastSafeArea.y;
                    }
                    else
                    {
                        values.posX = Screen.width - texture.width - LastSafeArea.x;
                        values.posX += 10;
                        values.posY = Screen.height - texture.height - LastSafeArea.y;
                        if (type == AdsManager.BannerAdTypes.NATIVE)
                            values.posY += 10;
                        else
                            values.posY += 5;

                    }
                }
                else
                {
                    values.posX = Screen.width - texture.width;
                    values.posY = Screen.height - texture.height;
                }
                values.labelX = values.posX + borderWidth;
                values.labelY = values.posY + borderWidth;
                break;
        }

        if (type == AdsManager.BannerAdTypes.ADAPTIVE)
            values.posX = Screen.width - Screen.width;

        return values;


    }

    #region SafeArea Controlling

    /// <summary>
    /// Simulation device that uses safe area due to a physical notch or software home bar. For use in Editor only.
    /// </summary>
    public enum SimDevice
    {
        /// <summary>
        /// Don't use a simulated safe area - GUI will be full screen as normal.
        /// </summary>
        None,
        /// <summary>
        /// Simulate the iPhone X and Xs (identical safe areas).
        /// </summary>
        iPhoneX,
        /// <summary>
        /// Simulate the iPhone Xs Max and XR (identical safe areas).
        /// </summary>
        iPhoneXsMax,
        /// <summary>
        /// Simulate the Google Pixel 3 XL using landscape left.
        /// </summary>
        Pixel3XL_LSL,
        /// <summary>
        /// Simulate the Google Pixel 3 XL using landscape right.
        /// </summary>
        Pixel3XL_LSR
    }

    /// <summary>
    /// Simulation mode for use in editor only. This can be edited at runtime to toggle between different safe areas.
    /// </summary>
    public static SimDevice Sim = SimDevice.None;

    /// <summary>
    /// Normalised safe areas for iPhone X with Home indicator (ratios are identical to Xs, 11 Pro). Absolute values:
    ///  PortraitU x=0, y=102, w=1125, h=2202 on full extents w=1125, h=2436;
    ///  PortraitD x=0, y=102, w=1125, h=2202 on full extents w=1125, h=2436 (not supported, remains in Portrait Up);
    ///  LandscapeL x=132, y=63, w=2172, h=1062 on full extents w=2436, h=1125;
    ///  LandscapeR x=132, y=63, w=2172, h=1062 on full extents w=2436, h=1125.
    ///  Aspect Ratio: ~19.5:9.
    /// </summary>
    Rect[] NSA_iPhoneX = new Rect[]
    {
            new Rect (0f, 102f / 2436f, 1f, 2202f / 2436f),  // Portrait
            new Rect (132f / 2436f, 63f / 1125f, 2172f / 2436f, 1062f / 1125f)  // Landscape
    };

    /// <summary>
    /// Normalised safe areas for iPhone Xs Max with Home indicator (ratios are identical to XR, 11, 11 Pro Max). Absolute values:
    ///  PortraitU x=0, y=102, w=1242, h=2454 on full extents w=1242, h=2688;
    ///  PortraitD x=0, y=102, w=1242, h=2454 on full extents w=1242, h=2688 (not supported, remains in Portrait Up);
    ///  LandscapeL x=132, y=63, w=2424, h=1179 on full extents w=2688, h=1242;
    ///  LandscapeR x=132, y=63, w=2424, h=1179 on full extents w=2688, h=1242.
    ///  Aspect Ratio: ~19.5:9.
    /// </summary>
    Rect[] NSA_iPhoneXsMax = new Rect[]
    {
            new Rect (0f, 102f / 2688f, 1f, 2454f / 2688f),  // Portrait
            new Rect (132f / 2688f, 63f / 1242f, 2424f / 2688f, 1179f / 1242f)  // Landscape
    };

    /// <summary>
    /// Normalised safe areas for Pixel 3 XL using landscape left. Absolute values:
    ///  PortraitU x=0, y=0, w=1440, h=2789 on full extents w=1440, h=2960;
    ///  PortraitD x=0, y=0, w=1440, h=2789 on full extents w=1440, h=2960;
    ///  LandscapeL x=171, y=0, w=2789, h=1440 on full extents w=2960, h=1440;
    ///  LandscapeR x=0, y=0, w=2789, h=1440 on full extents w=2960, h=1440.
    ///  Aspect Ratio: 18.5:9.
    /// </summary>
    Rect[] NSA_Pixel3XL_LSL = new Rect[]
    {
            new Rect (0f, 0f, 1f, 2789f / 2960f),  // Portrait
            new Rect (0f, 0f, 2789f / 2960f, 1f)  // Landscape
    };

    /// <summary>
    /// Normalised safe areas for Pixel 3 XL using landscape right. Absolute values and aspect ratio same as above.
    /// </summary>
    Rect[] NSA_Pixel3XL_LSR = new Rect[]
    {
            new Rect (0f, 0f, 1f, 2789f / 2960f),  // Portrait
            new Rect (171f / 2960f, 0f, 2789f / 2960f, 1f)  // Landscape
    };


    Rect LastSafeArea = new Rect(0, 0, 0, 0);
    Vector2Int LastScreenSize = new Vector2Int(0, 0);
    ScreenOrientation LastOrientation = ScreenOrientation.AutoRotation;
    bool ConformX = true;  // Conform to screen safe area on X-axis (default true, disable to ignore)
    bool ConformY = true;  // Conform to screen safe area on Y-axis (default true, disable to ignore)
    [SerializeField] bool Logging = false;  // Conform to screen safe area on Y-axis (default true, disable to ignore)

    void Awake()
    {
#if UNITY_ANDROID

        ConformX = false;
        ConformY = false;
#elif UNITY_IOS
     ConformX = true;  
     ConformY = true;

#endif


        //Panel = GetComponent<RectTransform>();

        //if (Panel == null)
        //{
        //    Debug.LogError("Cannot apply safe area - no RectTransform found on " + name);
        //    Destroy(gameObject);
        //}

        Refresh();
    }

    void Update()
    {
        Refresh();
    }

    void Refresh()
    {
        Rect safeArea = GetSafeArea();

        if (safeArea != LastSafeArea
            || Screen.width != LastScreenSize.x
            || Screen.height != LastScreenSize.y
            || Screen.orientation != LastOrientation)
        {
            // Fix for having auto-rotate off and manually forcing a screen orientation.
            // See https://forum.unity.com/threads/569236/#post-4473253 and https://forum.unity.com/threads/569236/page-2#post-5166467
            LastScreenSize.x = Screen.width;
            LastScreenSize.y = Screen.height;
            LastOrientation = Screen.orientation;

            ApplySafeArea(safeArea);
        }
    }

    Rect GetSafeArea()
    {
        Rect safeArea = Screen.safeArea;

        if (Application.isEditor && Sim != SimDevice.None)
        {
            Rect nsa = new Rect(0, 0, Screen.width, Screen.height);

            switch (Sim)
            {
                case SimDevice.iPhoneX:
                    if (Screen.height > Screen.width)  // Portrait
                        nsa = NSA_iPhoneX[0];
                    else  // Landscape
                        nsa = NSA_iPhoneX[1];
                    break;
                case SimDevice.iPhoneXsMax:
                    if (Screen.height > Screen.width)  // Portrait
                        nsa = NSA_iPhoneXsMax[0];
                    else  // Landscape
                        nsa = NSA_iPhoneXsMax[1];
                    break;
                case SimDevice.Pixel3XL_LSL:
                    if (Screen.height > Screen.width)  // Portrait
                        nsa = NSA_Pixel3XL_LSL[0];
                    else  // Landscape
                        nsa = NSA_Pixel3XL_LSL[1];
                    break;
                case SimDevice.Pixel3XL_LSR:
                    if (Screen.height > Screen.width)  // Portrait
                        nsa = NSA_Pixel3XL_LSR[0];
                    else  // Landscape
                        nsa = NSA_Pixel3XL_LSR[1];
                    break;
                default:
                    break;
            }

            safeArea = new Rect(Screen.width * nsa.x, Screen.height * nsa.y, Screen.width * nsa.width, Screen.height * nsa.height);
        }

        return safeArea;
    }

    void ApplySafeArea(Rect r)
    {
        LastSafeArea = r;

        // Ignore x-axis?
        if (!ConformX)
        {
            r.x = 0;
            r.width = Screen.width;
        }

        // Ignore y-axis?
        if (!ConformY)
        {
            r.y = 0;
            r.height = Screen.height;
        }

        // Check for invalid screen startup state on some Samsung devices (see below)
        if (Screen.width > 0 && Screen.height > 0)
        {
            // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            // Fix for some Samsung devices (e.g. Note 10+, A71, S20) where Refresh gets called twice and the first time returns NaN anchor coordinates
            // See https://forum.unity.com/threads/569236/page-2#post-6199352
            //if (anchorMin.x >= 0 && anchorMin.y >= 0 && anchorMax.x >= 0 && anchorMax.y >= 0)
            //{
            //    Panel.anchorMin = anchorMin;
            //    Panel.anchorMax = anchorMax;
            //}
        }

        if (Logging)
        {
            Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
            name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
        }
    }


    #endregion


}
