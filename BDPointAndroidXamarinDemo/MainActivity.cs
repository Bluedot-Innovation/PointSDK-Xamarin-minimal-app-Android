using Android.App;
using Android.Widget;
using Android.OS;
using AU.Com.Bluedot.Application.Model;
using AU.Com.Bluedot.Point;
using AU.Com.Bluedot.Point.Net.Engine;
using System;
using System.Collections.Generic;
using Android.Content.PM;
using Java.Interop;
/*
* Xamarin Activity implementing Bluedot Point SDK service interfaces(generated through the binding project) using JNI
*/

namespace BDPointAndroidXamarinDemo
{
    [Activity(Label = "BDPointXamarinDemo", MainLauncher = true, Icon = "@mipmap/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity, IInitializationResultListener, IGeoTriggeringStatusListener, ITempoServiceStatusListener, IResetResultReceiver
    {

        readonly string[] PermissionsLocation =
        {
            Android.Manifest.Permission.AccessFineLocation
        };
        const string permissionFine = Android.Manifest.Permission.AccessFineLocation;
        const int RequestLocationId = 0;

        TextView textViewStatusLog;
        EditText editTextProjectId;
        EditText editTextDestId;
        ToggleButton initButton;
        ToggleButton geoButton;
        ToggleButton tempoButton;
        //Enter ProjectId here
        String projectId = "";

        ServiceManager serviceManager;
      
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            serviceManager = ServiceManager.GetInstance(this);

            //set CustomEventMetadata
            IDictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("uuid", "1234");
            keyValuePairs.Add("size", "34");
            serviceManager.SetCustomEventMetaData(keyValuePairs);

            SetContentView(Resource.Layout.Main);


            textViewStatusLog = (TextView)FindViewById(Resource.Id.tvStatusLog);
            editTextProjectId = (EditText)FindViewById(Resource.Id.etProjectId);
            editTextDestId = (EditText)FindViewById(Resource.Id.etDestinationId);

            initButton = FindViewById<ToggleButton>(Resource.Id.init);
            initButton.Click += (sender, e) =>
            {

                if ((CheckSelfPermission(permissionFine) == (int)Permission.Granted))
                {
                    if (initButton.Checked)
                        startInit();
                    else
                        reset();
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
                    startGeoTrigger();
                else
                    stopGeoTrigger();

            };

            tempoButton = FindViewById<ToggleButton>(Resource.Id.tempo);
            tempoButton.Click += (sender, e) =>
            {
                if (tempoButton.Checked)
                    startTempo();
                else
                    stopTempo();

            };
        }

        private void startInit()
        {
            projectId = editTextProjectId.Text;
            if (!serviceManager.IsBluedotServiceInitialized)
            {
                serviceManager.Initialize(projectId, this);
                updateLog("Initializing..");
            }
            else
            {
                updateLog("Already Initialized");
            }

        }

        private void startGeoTrigger()
        {

            GeoTriggeringService.Builder()
               .InvokeNotification(createNotification())
               .Start(this, this);
        }


        private void stopGeoTrigger()
        {
            GeoTriggeringService.Stop(this, this);
        }

        private void startTempo()
        {
            String destinationId = editTextDestId.Text;
            TempoService.Builder()
                .InvokeDestinationId(destinationId)
                .InvokeNotification(createNotification())
                .Start(this, this);
        }

        private void stopTempo()
        {
            BDError error = TempoService.Stop(this);
            if (error == null)
                updateLog("Tempo Stop success");
            else
                updateLog("Error in stopping Tempo" + error.Reason);
        }

        private void reset()
        {
            if (serviceManager.IsBluedotServiceInitialized)
            {
                serviceManager.Reset(this);
                updateLog("Reseting SDK");
            }
            else
                updateLog("Already Reset");
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
                            startInit();
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



       private void updateLog(String s)
        {
            RunOnUiThread(() =>
            {
                textViewStatusLog.Append(s + "\n");
            });
        }


        private Notification createNotification()
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
                        .SetSmallIcon(Resource.Mipmap.Icon);

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
                        .SetSmallIcon(Resource.Mipmap.Icon);

                return notification.Build();
            }
        }

        public void OnInitializationFinished(BDError error)
        {
            if (error == null)
            {
                //initButton.Checked = true;
                updateLog("Initialized Success");
                return;
            }
            updateLog("Error " + error.Reason);
        }

        public void OnGeoTriggeringResult(BDError geoTriggerError)
        {
            if (geoTriggerError != null)
            {
                updateLog("Error in GeoTrigger" + geoTriggerError.Reason);
                return;
            }
            updateLog("GeoTrigger action success");

        }

        public void OnTempoResult(BDError tempoError)
        {
            if (tempoError != null)
            {
                updateLog("Error in Tempo" + tempoError.Reason);
                return;
            }
            updateLog("Tempo start success");
        }

        public void OnResetFinished(BDError error)
        {
            if (error != null)
            {
                updateLog("Error in Reset" + error.Reason);
                return;
            }
            updateLog("Reset success");
        }
    }
}

