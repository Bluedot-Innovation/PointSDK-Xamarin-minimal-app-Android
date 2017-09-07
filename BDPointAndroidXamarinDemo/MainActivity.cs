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
        ToggleButton authenticateButton;

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
            authenticateButton.Checked = false;
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

            // Modify Title and Message to deliver a meaningful message to user.
            serviceManager.SetForegroundServiceNotification(Resource.Mipmap.Icon, "Title", "Message", null);
            serviceManager.SubscribeForApplicationNotification(this);
            SetContentView(Resource.Layout.Main);


            textViewStatusLog = (TextView)FindViewById(Resource.Id.tvStatusLog);

            authenticateButton = FindViewById<ToggleButton>(Resource.Id.authenticate);
            authenticateButton.Click += (sender, e) =>
            {
                if (authenticateButton.Checked)
                    startAuthentication();
                else
                    stopService();

            };
        }

        private void startAuthentication()
        {
            if (!serviceManager.IsBlueDotPointServiceRunning)
            {
				/* Start the Bluedot Point Service by providing with the credentials and a ServiceStatusListener, 
				 * the app will be notified via the status listener if the Bluedot Point Service started successful.
				 * 
				 * Parameters
				 * packageName  The package name of your app created in the Bluedot Point Access
				 * apiKey       The API key generated for your app in the Bluedot Point Access
				 * userName     The user name you used to login to the Bluedot Point Access
				 * listener     A Service Status Listener
                 */
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

