using Android.App;
using Android.Widget;
using Android.OS;
using AU.Com.Bluedot.Application.Model;
using AU.Com.Bluedot.Point;
using AU.Com.Bluedot.Point.Net.Engine;
using System;
using System.Collections.Generic;

/*
 * Xamarin Activity implementing Bluedot Point SDK service interfaces(generated through the binding project) using JNI
 */

namespace BDPointAndroidXamarinDemo
{
    [Activity(Label = "BDPointXamarinDemo", MainLauncher = true, Icon = "@mipmap/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity, IServiceStatusListener, IApplicationNotificationListener
    {
        TextView textViewStatusLog;
        Button authenticateButton;
        Button logoutButton;

        ServiceManager serviceManager;

        public void OnBlueDotPointServiceError(BDError p0)
        {
            updateLog("Bluedot service returned error" + p0.Reason);
        }

        public void OnBlueDotPointServiceStartedSuccess()
        {
            updateLog("Bluedot service started successfully");
        }

        public void OnBlueDotPointServiceStop()
        {
            updateLog("Bluedot service stopped");
        }

        public void OnCheckedOutFromBeacon(BeaconInfo p0, ZoneInfo p1, int p2, IDictionary<string, string> p3)
        {
            updateLog("Bluedot service - OnCheckedOutFromFence.");
        }

        public void OnCheckedOutFromFence(FenceInfo p0, ZoneInfo p1, int p2, IDictionary<string, string> p3)
        {
            updateLog("Bluedot service - OnCheckedOutFromBeacon.");
        }

        public void OnCheckIntoBeacon(BeaconInfo p0, ZoneInfo p1, LocationInfo p2, Proximity p3, IDictionary<string, string> p4, bool p5)
        {
            updateLog("Bluedot service - OnCheckIntoBeacon.");
        }

        public void OnCheckIntoFence(FenceInfo p0, ZoneInfo p1, LocationInfo p2, IDictionary<string, string> p3, bool p4)
        {
            updateLog("Bluedot service - OnCheckIntoFence.");
        }

        public void OnRuleUpdate(IList<ZoneInfo> p0)
        {
            updateLog("Bluedot service - rules downloaded.");
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            serviceManager = ServiceManager.GetInstance(this);

            serviceManager.SetForegroundServiceNotification(Resource.Mipmap.Icon, "Title", "Message", null);
            serviceManager.SubscribeForApplicationNotification(this);
            SetContentView(Resource.Layout.Main);


            textViewStatusLog = (TextView)FindViewById(Resource.Id.tvStatusLog);

            authenticateButton = FindViewById<Button>(Resource.Id.authenticate);
            authenticateButton.Click += (sender, e) =>
            {
                startAuthentication();

            };

            logoutButton = FindViewById<Button>(Resource.Id.logout);
            logoutButton.Click += (sender, e) =>
            {
                stopService();

            };
        }

        private void startAuthentication()
        {
            if (!serviceManager.IsBlueDotPointServiceRunning)
            {
                serviceManager.SendAuthenticationRequest("performance.test.v193", "fbad63e0-e760-11e6-a1d3-b8ca3a6b879d", "hello@bluedotinnovation.com", this);
                updateLog("Authenticating..");
            }
            else
            {
                updateLog("Already Authenticated");
            }

        }

        public void stopService()
        {
            if (serviceManager != null)
            {
                if (serviceManager.IsBlueDotPointServiceRunning)
                {
                    //Call the method stopPointService in ServiceManager to stop Bluedot PointService
                    serviceManager.StopPointService();
                    updateLog("Logged Out");
                }
                else
                {
                    updateLog("Already Logged out");
                }

            }

        }


        private void updateLog(String s)
        {
            RunOnUiThread(() =>
            {
                textViewStatusLog.Append(s + "\n");
            });
        }

    }
}

