using Android.App;
using Android.Content;
using Android.Widget;
using AU.Com.Bluedot.Point.Net.Engine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BDPointAndroidXamarinDemo
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
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
            MainActivity.Instance.UpdateLog($"Entered Zone: {zoneInfo.ZoneName}");
        }

        public override void OnZoneExitEvent(ZoneExitEvent exitEvent, Context context)
        {
            ZoneInfo zoneInfo = exitEvent.ZoneInfo;
            string zoneData = zoneInfo.CustomData.Aggregate(new StringBuilder(),
                (sb, entry) => sb.Append($"{entry.Key}: {entry.Value}\n"),
                sb => sb.ToString());
            string exitText = $"Left {zoneInfo.ZoneName}\n{zoneData}";
            Toast.MakeText(context, exitText, ToastLength.Short).Show();
            MainActivity.Instance.UpdateLog($"Exited Zone: {zoneInfo.ZoneName}");
        }

        public override void OnZoneInfoUpdate(IList<ZoneInfo> zones, Context context)
        {
            MainActivity.Instance.UpdateLog("OnZoneInfoUpdate");
            Toast.MakeText(context, "Received OnZoneInfoUpdate", ToastLength.Short).Show();
        }
    }

    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[] { "io.bluedot.point.TEMPO" })]
    public class AppTempoReceiver : TempoTrackingReceiver
    {
        public override void TempoStoppedWithError(BDError error, Context context)
        {
            MainActivity.Instance.resetTempoButton();
            MainActivity.Instance.UpdateLog("TempoStoppedWithError: " + error.Reason);
            Toast.MakeText(context, "Received TempoStoppedWithError " + error.Reason, ToastLength.Short).Show();
        }
    }

    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[] { "io.bluedot.point.SERVICE" })]
    public class BluedotErrorReceiver : BluedotServiceReceiver
    {
        public override void OnBluedotServiceError(BDError error, Context context)
        {
            MainActivity.Instance.UpdateLog("TempoStoppedWithError: " + error.Reason);
            Toast.MakeText(context, "Received OnBluedotServiceError " + error.Reason, ToastLength.Short).Show();
        }
    }
}
