using Android.App;
using Android.Widget;
using Android.OS;
using AU.Com.Bluedot.Application.Model;
using AU.Com.Bluedot.Point;
using AU.Com.Bluedot.Point.Net.Engine;
using System;
using System.Collections.Generic;

namespace BDPointAndroidXamarinDemo
{
    [Activity(Label = "BDPointXamarinDemo", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity, IServiceStatusListener, IApplicationNotificationListener
    {
        int count = 1;

        public void OnBlueDotPointServiceError(BDError p0)
        {
            Console.WriteLine("Bluedot service returned error");
        }

        public void OnBlueDotPointServiceStartedSuccess()
        {
            Console.WriteLine("Bluedot service started successfully");
        }

        public void OnBlueDotPointServiceStop()
        {
            Console.WriteLine("Bluedot service stopped");
        }

        public void OnCheckedOutFromBeacon(BeaconInfo p0, ZoneInfo p1, int p2, IDictionary<string, string> p3)
        {
            Console.WriteLine("Bluedot service - OnCheckedOutFromFence.");
        }

        public void OnCheckedOutFromFence(FenceInfo p0, ZoneInfo p1, int p2, IDictionary<string, string> p3)
        {
            Console.WriteLine("Bluedot service - OnCheckedOutFromBeacon.");
        }

        public void OnCheckIntoBeacon(BeaconInfo p0, ZoneInfo p1, LocationInfo p2, Proximity p3, IDictionary<string, string> p4, bool p5)
        {
            Console.WriteLine("Bluedot service - OnCheckIntoBeacon.");
        }

        public void OnCheckIntoFence(FenceInfo p0, ZoneInfo p1, LocationInfo p2, IDictionary<string, string> p3, bool p4)
        {
           Console.WriteLine("Bluedot service - OnCheckIntoFence.");
        }

        public void OnRuleUpdate(IList<ZoneInfo> p0)
        {
            Console.WriteLine("Bluedot service - rules downloaded.");
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ServiceManager serviceManager = ServiceManager.GetInstance(this);

            serviceManager.SetForegroundServiceNotification(Resource.Id.icon, "Title", "Message", null);
            serviceManager.SubscribeForApplicationNotification(this);
            if (!serviceManager.IsBlueDotPointServiceRunning)
			{
                serviceManager.SendAuthenticationRequest("performance.test.v193", "fbad63e0-e760-11e6-a1d3-b8ca3a6b879d", "hello@bluedotinnovation.com", this);
			}
            SetContentView(Resource.Layout.Main);
        }
    }
}

