using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AU.Com.Bluedot.Point.Net.Engine;
using System;
using System.Collections.Generic;


namespace BDPointAndroidXamarinDemo
{

    public class InitializationStatusListener : Java.Lang.Object, IInitializationResultListener
    {
        Context appContext;
        public InitializationStatusListener(Context context)
        {
            appContext = context;
        }
        public void OnInitializationFinished(BDError error)
        {
            if (error == null)
            {
                Toast.MakeText(appContext, "Initialized Success", ToastLength.Short).Show();
                return;
            }
            Toast.MakeText(appContext, "Error: " + error.Reason, ToastLength.Long).Show();
        }
    }

    public class GeoStatusListener : Java.Lang.Object, IGeoTriggeringStatusListener
    {
        Context appContext;
        public GeoStatusListener(Context context)
        {
            appContext = context;
        }

        public void OnGeoTriggeringResult(BDError geoTriggerError)
        {
            if (geoTriggerError != null)
            {
                Toast.MakeText(appContext, "Error in GeoTrigger: " + geoTriggerError.Reason, ToastLength.Long).Show();
                return;
            }
            Toast.MakeText(appContext, "GeoTrigger action success", ToastLength.Long).Show();
        }
    }

    public class TempoStatusListener : Java.Lang.Object, ITempoServiceStatusListener
    {
        Context appContext;
        public TempoStatusListener(Context context)
        {
            appContext = context;
        }

        public void OnTempoResult(BDError tempoError)
        {
            if (tempoError != null)
            {
                Toast.MakeText(appContext, "Error in Tempo: " + tempoError.Reason, ToastLength.Long).Show();
                return;
            }
            Toast.MakeText(appContext, "Tempo start success", ToastLength.Short).Show();
        }
    }


    public class ResetStatusListener : Java.Lang.Object, IResetResultReceiver
    {
        Context appContext;
        public ResetStatusListener(Context context)
        {
            appContext = context;
        }

        public void OnResetFinished(BDError error)
        {
            if (error != null)
            {
                Toast.MakeText(appContext, "Error in Reset: " + error.Reason, ToastLength.Long).Show();
                return;
            }
            Toast.MakeText(appContext, "Reset success", ToastLength.Short).Show();
        }
    }

    [Activity(Label = "BDPointXamarinDemo", MainLauncher = true, Icon = "@mipmap/ic_launcher", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        public static MainActivity Instance;

        readonly string[] PermissionsLocation =
        {
            Android.Manifest.Permission.AccessFineLocation
        };
        const string permissionFine = Android.Manifest.Permission.AccessFineLocation;
        const int RequestLocationId = 0;

        TextView textViewStatusLog;
        TextView appVersionTextView;
        TextView sdkVersionTextView;
        EditText editTextProjectId;
        EditText editTextDestId;
        ToggleButton initButton;
        ToggleButton geoButton;
        ToggleButton bgGeoButton;
        ToggleButton tempoButton;
        //Enter ProjectId here
        string projectId = "";

        ServiceManager serviceManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Instance = this;

            serviceManager = ServiceManager.GetInstance(this);

            //set CustomEventMetadata
            IDictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("uuid", "1234");
            keyValuePairs.Add("size", "34");
            serviceManager.CustomEventMetaData = keyValuePairs;

            SetContentView(Resource.Layout.Main);


            textViewStatusLog = (TextView)FindViewById(Resource.Id.tvStatusLog);
            appVersionTextView = (TextView)FindViewById(Resource.Id.tvAppVersion);
            sdkVersionTextView = (TextView)FindViewById(Resource.Id.tvSDKVersion);
            editTextProjectId = (EditText)FindViewById(Resource.Id.etProjectId);
            editTextDestId = (EditText)FindViewById(Resource.Id.etDestinationId);


            String appVersion = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;
            String sdkVersion = serviceManager.SdkVersion;

            appVersionTextView.Text = GetString(Resource.String.bluedot_app, appVersion);
            sdkVersionTextView.Text = GetString(Resource.String.bluedot_sdk, sdkVersion);
            initButton = FindViewById<ToggleButton>(Resource.Id.init);
            initButton.Click += (sender, e) =>
            {

                if ((CheckSelfPermission(permissionFine) == (int)Permission.Granted))
                {
                    if (initButton.Checked) {
                        StartInit();
                        StartTempo("");
                    } else {
                        Reset();
                    }
                }
                else
                {
                    RequestPermissions(PermissionsLocation, RequestLocationId);
                }

            };

            geoButton = FindViewById<ToggleButton>(Resource.Id.geoTrigger);
            geoButton.Click += (sender, e) =>
            {
                if (geoButton.Checked)
                    StartGeoTrigger();
                else
                    StopGeoTrigger();

            };

            bgGeoButton = FindViewById<ToggleButton>(Resource.Id.bgGeoTrigger);
            bgGeoButton.Click += (sender, e) =>
            {
                if (bgGeoButton.Checked)
                    StartBgGeoTrigger();
                else
                    StopGeoTrigger();

            };

            tempoButton = FindViewById<ToggleButton>(Resource.Id.tempo);
            tempoButton.Click += (sender, e) =>
            {
                string destinationId = editTextDestId.Text;
                if (destinationId != null && destinationId != "")
                {
                    if (tempoButton.Checked)
                    {
                        StartTempo(destinationId);
                    }
                    else
                    {
                        StopTempo();
                    }
                }
                else
                {
                    // Reset tempo button if failed to start tempo
                    resetTempoButton();
                    Toast.MakeText(this, "Error: Invalid destination Id", ToastLength.Short).Show();
                }

            };
        }

        private void StartInit()
        {
            projectId = editTextProjectId.Text;
            if (projectId != null && projectId != "")
            {
                if (!serviceManager.IsBluedotServiceInitialized)
                {
                    serviceManager.Initialize(projectId: projectId,
                        initializationResultListener: new InitializationStatusListener(context: this));
                    UpdateLog("Initializing..");
                    UpdateLog("Version:" + serviceManager.SdkVersion);
                }
                else
                {
                    UpdateLog("Already Initialized");
                }
            }
            else
            {
                RunOnUiThread(() =>
                {
                    initButton.Checked = false;
                });
                UpdateLog("Invalid project Id.");
                Toast.MakeText(this, "Error: Invalid projectId", ToastLength.Long).Show();
            }

        }

        private void StartGeoTrigger()
        {
            GeoTriggeringService.Builder()
               .InvokeNotification(CreateNotification())
               .Start(context: this, geoTriggeringStatusListener: new GeoStatusListener(context: this));
            UpdateLog("Started Geo Triggering");
            RunOnUiThread(() =>
            {
                bgGeoButton.Checked = true;
                bgGeoButton.Visibility = Android.Views.ViewStates.Gone;
            });
        }

        private void StartBgGeoTrigger()
        {
            GeoTriggeringService.Builder()
                .Start(context: this,
                geoTriggeringStatusListener: new GeoStatusListener(context: this));
            UpdateLog("Started Background Geo Triggering");
            RunOnUiThread(() =>
            {
                geoButton.Checked = true;
                geoButton.Visibility = Android.Views.ViewStates.Gone;
            });

        }

        private void StopGeoTrigger()
        {
            GeoTriggeringService.Stop(context: this,
                geoTriggeringStatusListener: new GeoStatusListener(context: this));
            UpdateLog("Stopped Geo Triggering");
            updateGeoButtons(false);
        }

        private void updateGeoButtons(bool value)
        {
            RunOnUiThread(() =>
            {
                geoButton.Checked = value;
                bgGeoButton.Checked = value;
                if (!value)
                {
                    geoButton.Visibility = Android.Views.ViewStates.Visible;
                    bgGeoButton.Visibility = Android.Views.ViewStates.Visible;
                }
            });
        }

        private void StartTempo(string destinationId)
        {
            var metadata = new Dictionary<string, string>();
            var orderId = RandomString(6);
            metadata.Add("hs_orderId", orderId);
            metadata.Add("hs_customerName", "Testing");
            serviceManager.CustomEventMetaData = metadata;

            TempoService.Builder()
                .InvokeDestinationId(destinationId)
                .InvokeNotification(CreateNotification())
                .Start(context: this, new TempoStatusListener(context: this));
            UpdateLog("Tempo Start with Id:" + orderId);

        }

        private void StopTempo()
        {
            BDError error = TempoService.Stop(this);
            if (error == null)
                UpdateLog("Tempo Stop success");
            else
                UpdateLog("Error in stopping Tempo" + error.Reason);
        }

        public void resetTempoButton()
        {
            RunOnUiThread(() =>
            {
                tempoButton.Checked = false;
            });
        }

        private void Reset()
        {
            if (serviceManager.IsBluedotServiceInitialized)
            {
                serviceManager.Reset(receiver: new ResetStatusListener(context: this));
                UpdateLog("Reseting SDK");
            }
            else
                UpdateLog("Already Reset");
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestLocationId:
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            //Permission granted
                            StartInit();
                        }
                        else
                        {
                            //Permission Denied :(
                            //Disabling location functionality
                            Toast.MakeText(this, "Location permission is denied.", ToastLength.Long).Show();

                        }
                    }
                    break;
            }

        }

        public void UpdateLog(String s)
        {
            RunOnUiThread(() =>
            {
                textViewStatusLog.Append(s + "\n");
            });
        }

        private static Random random = new Random();

        private static String RandomString(int length)
        {
            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz1234567890";
            var stringChars = new char[length];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            return new String(stringChars);
        }


        private Notification CreateNotification()
        {

            String channelId;

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                channelId = "Bluedot" + GetString(Resource.String.app_name);
                String channelName = "Bluedot Service" + GetString(Resource.String.app_name);

                NotificationChannel notificationChannel = new NotificationChannel(channelId, channelName, NotificationImportance.High);
                notificationChannel.EnableLights(false);
                notificationChannel.EnableVibration(false);

                NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(notificationChannel);

                Notification.Builder notification = new Notification.Builder(this, channelId)
                    .SetContentTitle(GetString(Resource.String.foreground_notification_title))
                    .SetContentText(GetString(Resource.String.foreground_notification_text))
                    .SetStyle(new Notification.BigTextStyle().BigText(GetString(Resource.String.foreground_notification_text)))
                        .SetOngoing(true)
                        .SetCategory(Notification.CategoryService)
                        .SetSmallIcon(Resource.Mipmap.ic_launcher);

                return notification.Build();
            }
            else
            {

                Notification.Builder notification = new Notification.Builder(this)
                    .SetContentTitle(GetString(Resource.String.foreground_notification_title))
                    .SetContentText(GetString(Resource.String.foreground_notification_text))
                    .SetStyle(new Notification.BigTextStyle().BigText(GetString(Resource.String.foreground_notification_text)))
                        .SetOngoing(true)
                        .SetCategory(Notification.CategoryService)
                        .SetSmallIcon(Resource.Mipmap.ic_launcher);

                return notification.Build();
            }
        }

    }
}
