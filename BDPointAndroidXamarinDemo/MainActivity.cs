﻿using Android.App;
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
        ToggleButton bgGeoButton;
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
                        StartInit();
                    else
                        Reset();
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
                if (tempoButton.Checked)
                    StartTempo();
                else
                    StopTempo();

            };
        }

        private void StartInit()
        {
            projectId = editTextProjectId.Text;
            if (!serviceManager.IsBluedotServiceInitialized)
            {
                serviceManager.Initialize(projectId, this);
                UpdateLog("Initializing..");
            }
            else
            {
                UpdateLog("Already Initialized");
            }

        }

        private void StartGeoTrigger()
        {

            GeoTriggeringService.Builder()
               .InvokeNotification(CreateNotification())
               .Start(this, this);
        }

        private void StartBgGeoTrigger()
        {
            GeoTriggeringService.Builder()
                .Start(this, this);
        }

        private void StopGeoTrigger()
        {
            GeoTriggeringService.Stop(this, this);
        }

        private void StartTempo()
        {
            String destinationId = editTextDestId.Text;
            TempoService.Builder()
                .InvokeDestinationId(destinationId)
                .InvokeNotification(CreateNotification())
                .Start(this, this);
        }

        private void StopTempo()
        {
            BDError error = TempoService.Stop(this);
            if (error == null)
                UpdateLog("Tempo Stop success");
            else
                UpdateLog("Error in stopping Tempo" + error.Reason);
        }

        private void Reset()
        {
            if (serviceManager.IsBluedotServiceInitialized)
            {
                serviceManager.Reset(this);
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



       private void UpdateLog(String s)
        {
            RunOnUiThread(() =>
            {
                textViewStatusLog.Append(s + "\n");
            });
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
                UpdateLog("Initialized Success");
                return;
            }
            UpdateLog("Error " + error.Reason);
        }

        public void OnGeoTriggeringResult(BDError geoTriggerError)
        {
            if (geoTriggerError != null)
            {
                UpdateLog("Error in GeoTrigger" + geoTriggerError.Reason);
                return;
            }
            UpdateLog("GeoTrigger action success");

        }

        public void OnTempoResult(BDError tempoError)
        {
            if (tempoError != null)
            {
                UpdateLog("Error in Tempo" + tempoError.Reason);
                return;
            }
            UpdateLog("Tempo start success");
        }

        public void OnResetFinished(BDError error)
        {
            if (error != null)
            {
                UpdateLog("Error in Reset" + error.Reason);
                return;
            }
            UpdateLog("Reset success");
        }
    }
}

