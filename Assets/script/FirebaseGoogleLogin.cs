using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Firebase.Auth;
using Google;
using System.Net.Http;
using UnityEngine.UI;
using System;

public class FirebaseGoogleLogin : MonoBehaviour
{

    public string GoogleWebAPI = "300743319814-2p7r4eo7f9na2u3d226n2ckce2bmpmj3.apps.googleusercontent.com";
    private string WebClientId = "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";

    public GoogleSignInConfiguration Configuration;

    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    public Text UsernameTxt, UsernameEmailTxt;

    public Image UserProfilePic;
    public string imageUrl;
    public GameObject Loginscreen, ProfileScreen;

    private void Awake()
    {
        Configuration = new GoogleSignInConfiguration
        {
            WebClientId = GoogleWebAPI,
            RequestIdToken = true

        };
    }

    // Start is called before the first frame update
    void Start()
    {
        InitFirebase();
    }

    void InitFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    public void GoogleSignInClick()
    {
        GoogleSignIn.Configuration = Configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(OnGoogleAuthenticatedFinished);
    }
    public void OnGoogleAuthenticatedFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            Debug.Log("fault");
        }
        else if (task.IsCanceled)
        {
            Debug.Log("Login Cancel");
        }
        else
        {
            Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

            Debug.Log($"GoogleLoginToken: {task.Result.IdToken}");

            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("SignInWithCrendialAsync was canceled");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.Log("SignInWithCrendialAsync encountred error");
                    return;
                }
                user = auth.CurrentUser;
            

                UsernameTxt.text = user.DisplayName;
                UsernameEmailTxt.text = user.Email;

                Loginscreen.SetActive(false);
                ProfileScreen.SetActive(true);

                StartCoroutine(LoadImage(CheckImageUrl(user.PhotoUrl.ToString())));

            });
        }
    }

    private string CheckImageUrl(string Url)
    {
        if (!string.IsNullOrEmpty(Url))
        {
            return Url;
        }
        return imageUrl;

    }
    IEnumerator LoadImage(string imageUri)
    {
        WWW www = new WWW(imageUri);
        yield return www;
        UserProfilePic.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));

    }

}
