using System;
using UnityEngine;
using UnityEngine.Events;

#if IAP_Purchaser
using UnityEngine.Purchasing;
#endif

public class IAPManager : MonoBehaviour
#if IAP_Purchaser
    ,
    IStoreListener
#endif

{
    public static IAPManager instance;
    #if IAP_Purchaser

    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;

    //Step 1 create your products on google play and app store

    [Header("Android IAP Keys")]
    public string[] Android_Non_Consumable;
    public string[] Android_Consumable;

    [Header("IOS IAP Keys")]
    public string[] IOS_Non_Consumable;
    public string[] IOS_Consumable;

    [Space(20)]
    public string[] Non_Consumable;
    public string[] Consumable;

    private string inAppToBuy;

    //Step 3 Create methods
    //public void BuyRemoveAds()
    //{
    //    BuyProductID(Non_Consumable[0]);
    //}
	public void ClickRestorePurchaseButton() 
    {
        RestorePurchases();//only support IOS device(apple)
    }
    
    void AssigningKeys()
    {
#if UNITY_ANDROID
        Non_Consumable = new string[Android_Non_Consumable.Length];
        Consumable = new string[Android_Consumable.Length];

        for (int i = 0; i < Android_Non_Consumable.Length; i++)
        {
            Non_Consumable[i] = Android_Non_Consumable[i];
        }

        for (int i = 0; i < Android_Consumable.Length; i++)
        {
            Consumable[i] = Android_Consumable[i];
        }


#elif UNITY_IOS

        Non_Consumable = new string[IOS_Non_Consumable.Length];
        Consumable = new string[IOS_Consumable.Length];

        for (int i = 0; i < IOS_Non_Consumable.Length; i++)
        {
            Non_Consumable[i] = IOS_Non_Consumable[i];
        }

        for (int i = 0; i < IOS_Consumable.Length; i++)
        {
            Consumable[i] = IOS_Consumable[i];
        }

#endif
    }

    ////Step 4 modify purchasing
    //public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    //{
    //    if (String.Equals(args.purchasedProduct.definition.id, removeAds, StringComparison.Ordinal))
    //    {
    //       SSTools.ShowMessage("Remove Ads Successful" , SSTools.Position.bottom , SSTools.Time.threeSecond);
    //    }
    //    else if (String.Equals(args.purchasedProduct.definition.id, premium, StringComparison.Ordinal))
    //    {
    //       SSTools.ShowMessage("premium Successful" , SSTools.Position.bottom , SSTools.Time.threeSecond);
    //    }
    //    else
    //    {
    //      SSTools.ShowMessage("Purchase Failed" , SSTools.Position.bottom , SSTools.Time.threeSecond);
    //    }
    //    return PurchaseProcessingResult.Complete;
    //}





    //**************************** Dont worry about these methods ***********************************
    private void Awake()
    {
        AssigningKeys();

        TestSingleton();



    }

    void Start()
    {
        if (m_StoreController == null)
        {
            InitializePurchasing();
        }

    }
    private static UnityAction OnProcessSuccessful = null;

    void SuccessFulPurchase()
    {

        Debug.Log("Function To Be Called After Purchase : " + OnProcessSuccessful.Method.Name);

        OnProcessSuccessful?.Invoke();

        //ShopManager.instance.ConsumableItemPurchase();

        Debug.Log("Successfull Purchased");
    }
    //************************** Adjust these methods **************************************
    public void InitializePurchasing()
    {
        if (IsInitialized()) { return; }
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //Step 2 choose if your product is a consumable or non consumable
        for (int i = 0; i < Non_Consumable.Length; i++)
        {
            builder.AddProduct(Non_Consumable[i], ProductType.NonConsumable);
        }
        for (int i = 0; i < Consumable.Length; i++)
        {
            builder.AddProduct(Consumable[i], ProductType.Consumable);
        }


        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    private void TestSingleton()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void BuyProductID(string productId , UnityAction OnSuccess)
    {
        if (OnSuccess != null)
            OnProcessSuccessful = OnSuccess;

        inAppToBuy = productId;


        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log(string.Format("BuyProductID: FAIL. Not purchasing product: '{0}'", product.definition.id));

                //SSTools.ShowMessage("BuyProductID: FAIL. Not purchasing product" , SSTools.Position.bottom , SSTools.Time.threeSecond);
            }
        }
        else
        {
            Debug.Log(string.Format("BuyProductID FAIL. Not initialized"));

            //SSTools.ShowMessage("BuyProductID FAIL. Not initialized." , SSTools.Position.bottom , SSTools.Time.threeSecond);
        }
    }

    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

#if UNITY_IOS
       
            Debug.Log("RestorePurchases started ...");

            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result) => {
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
       
#endif
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("IAP Initialized: PASS");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error + " " + message);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        if (String.Equals(purchaseEvent.purchasedProduct.definition.id, inAppToBuy, StringComparison.Ordinal))
        {
            SuccessFulPurchase();

            //Debug.Log("Purchase SuccessFull");
        }
        else
        {
            Debug.Log("Purchase Failed ");

        }
        return PurchaseProcessingResult.Complete;
            //throw new NotImplementedException();
    }


    //Used Localize Price according to region 
    public string GetProductInfo(string id)
    {
        Product product = m_StoreController.products.WithID(id);

        if (product != null)
        {
            string price = product.metadata.localizedPriceString.ToString();
            Debug.Log("Price of the Product is " + product.definition.id + " is " + price);

            return price;
        }
        else
        {
            Debug.Log("Product not Available");

            return "";
        }
    }


    public string LocalizeNonConsumablePurchase(int _ID)
    {
        string price;

        price = GetProductInfo(Non_Consumable[_ID]);
        return price;
    }

    public string LocalizeConsumablePurchase(int _ID)
    {
        string price;

        price = GetProductInfo(Consumable[_ID]);
        return price;
    }

#endif
}