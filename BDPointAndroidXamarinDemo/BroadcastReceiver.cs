
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AU.Com.Bluedot.Point.Net.Engine;

namespace BDPointAndroidXamarinDemo
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "io.bluedot.point.GEOTRIGGER" })]
    public class AppGeoTriggerReceiver : GeoTriggeringEventReceiver
    {
        public override void OnZoneEntryEvent(ZoneEntryEvent entryEvent, Context context)
        {
            Toast.MakeText(context, "Received OnZoneEntryEvent "+entryEvent.ZoneInfo.ZoneName, ToastLength.Short).Show();
        }

        public override void OnZoneExitEvent(ZoneExitEvent exitEvent, Context context)
        {
            Toast.MakeText(context, "Received OnZoneExitEvent", ToastLength.Short).Show();
        }

        public override void OnZoneInfoUpdate(IList<ZoneInfo> zones, Context context)
        {
            Toast.MakeText(context, "Received OnZoneInfoUpdate", ToastLength.Short).Show();
        }
    }

    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "io.bluedot.point.TEMPO" })]
    public class AppTempoReceiver : TempoTrackingReceiver
    {
        public override void TempoStoppedWithError(BDError error, Context context)
        {
            Toast.MakeText(context, "Received TempoStoppedWithError "+error.Reason, ToastLength.Short).Show();
        }
    }

    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "io.bluedot.point.SERVICE" })]
    public class BluedotErrorReceiver : BluedotServiceReceiver
    {
        public override void OnBluedotServiceError(BDError error, Context context)
        {
            Toast.MakeText(context, "Received OnBluedotServiceError " + error.Reason, ToastLength.Short).Show();
        }
    }
}
