using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Ump;
using GoogleMobileAds.Ump.Api;

public class GDPRScript : MonoBehaviour
{

    [SerializeField] private bool TestCheck;
    [SerializeField] private DebugGeography _debugGeography;

    ConsentForm _consentForm;
    // Start is called before the first frame update
    void Start()
    {
        ConsentRequestParameters request;

        if (TestCheck)
        {
#if UNITY_ANDROID
            var debugSettings = new ConsentDebugSettings
            {
                // Geography appears as in EEA for debug devices.
                DebugGeography = _debugGeography,
                TestDeviceHashedIds = new List<string>
                {
                    "965E4A26737DF85475A353251709C315"
                }
            };
#elif UNITY_IOS
            var debugSettings = new ConsentDebugSettings
            {
                // Geography appears as in EEA for debug devices.
                DebugGeography = _debugGeography,
                TestDeviceHashedIds = new List<string>
                {
                    "2077ef9a63d2b398840261c8221a0c9b"
                }
            };

#endif

            // Here false means users are not under age.
            request = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false,
                ConsentDebugSettings = debugSettings,
            };
        }
        else
        {
            // Create a ConsentRequestParameters object.
            request = new ConsentRequestParameters();
        }

        // Check the current consent information status.
        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }

    void OnConsentInfoUpdated(FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            return;
        }

        if (ConsentInformation.IsConsentFormAvailable())
        {
            LoadConsentForm();
        }
        else
        {
            StartCoroutine(LoadScene());

        }
        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
    }

    void LoadConsentForm()
    {
        // Loads a consent form.
        ConsentForm.Load(OnLoadConsentForm);
    }

    void OnLoadConsentForm(ConsentForm consentForm, FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            return;
        }

        // The consent form was loaded.
        // Save the consent form for future requests.
        _consentForm = consentForm;

        // You are now ready to show the form.
        if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
        {
            _consentForm.Show(OnShowForm);
        }
        else if (ConsentInformation.ConsentStatus == ConsentStatus.Obtained)
        {
            StartCoroutine(LoadScene());

        }
    }


    void OnShowForm(FormError error)
    {
        if (error != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(error);
            return;
        }

        // Handle dismissal by reloading form.
        LoadConsentForm();


        StartCoroutine(LoadScene());
    }


    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
}
