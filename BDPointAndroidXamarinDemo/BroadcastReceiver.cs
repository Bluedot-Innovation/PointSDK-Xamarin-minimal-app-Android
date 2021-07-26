using Android.App;
using Android.Content;
using Android.Widget;
using AU.Com.Bluedot.Point.Net.Engine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDPointAndroidXamarinDemo
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "io.bluedot.point.GEOTRIGGER" })]
    public class AppGeoTriggerReceiver : GeoTriggeringEventReceiver
    {
        public override void OnZoneEntryEvent(ZoneEntryEvent entryEvent, Context context)
        {
            ZoneInfo zoneInfo = entryEvent.ZoneInfo;
            string zoneData = zoneInfo.CustomData.Aggregate(new StringBuilder(),
                (sb, entry) => sb.Append($"{entry.Key}: {entry.Value}\n"),
                sb => sb.ToString());
            string entryText = $"Entered {zoneInfo.ZoneName}\n{zoneData}";
            Toast.MakeText(context, entryText, ToastLength.Short).Show();
        }

        public override void OnZoneExitEvent(ZoneExitEvent exitEvent, Context context)
        {
            ZoneInfo zoneInfo = exitEvent.ZoneInfo;
            string zoneData = zoneInfo.CustomData.Aggregate(new StringBuilder(),
                (sb, entry) => sb.Append($"{entry.Key}: {entry.Value}\n"),
                sb => sb.ToString());
            string exitText = $"Left {zoneInfo.ZoneName}\n{zoneData}";
            Toast.MakeText(context, exitText, ToastLength.Short).Show();
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
