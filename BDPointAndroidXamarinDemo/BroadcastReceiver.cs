using Android.App;
using Android.Content;
using Android.Widget;
using AU.Com.Bluedot.Point.Net.Engine;
using AU.Com.Bluedot.Point.Net.Engine.Event;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using Context = Android.Content.Context;

namespace BDPointAndroidXamarinDemo
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[] { "io.bluedot.point.GEOTRIGGER" })]
    public class AppGeoTriggerReceiver : GeoTriggeringEventReceiver
    {
        public override void OnZoneEntryEvent(GeoTriggerEvent entryEvent, Context context)
        {
            string entryText = $"Entered zone: {entryEvent.ZoneInfo.Name}\n{entryEvent.ToJson()}";
            Toast.MakeText(context, entryText, ToastLength.Short).Show();
            MainActivity.Instance.UpdateLog(entryText);
        }

        public override void OnZoneExitEvent(GeoTriggerEvent exitEvent, Context context)
        {
            string exitText = $"Exited zone: {exitEvent.ZoneInfo.Name}\n{exitEvent.ToJson()}";
            Toast.MakeText(context, exitText, ToastLength.Short).Show();
            MainActivity.Instance.UpdateLog(exitText);

        }

        public override void OnZoneInfoUpdate(Context context)
        {
            IList<ZoneInfo> zonesInfos = ServiceManager.GetInstance(context).ZonesAndFences;
            if (zonesInfos != null)
            {
                string allZoneInfos = "";
                zonesInfos.ToList().ForEach(eachZoneInfo => allZoneInfos += eachZoneInfo.ToString() + "\n");
                MainActivity.Instance.UpdateLog("OnZoneInfoUpdate: " + allZoneInfos);
            }
            Toast.MakeText(context, "Received OnZoneInfoUpdate", ToastLength.Short).Show();
        }
    }

    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[] { "io.bluedot.point.TEMPO" })]
    public class AppTempoReceiver : TempoTrackingReceiver
    {
        public override void OnTempoTrackingUpdate(TempoTrackingUpdate tempoTrackingUpdate, Context context)
        {
            MainActivity.Instance.UpdateLog("tempoTrackingUpdate: " + tempoTrackingUpdate.ToJson());
            Toast.MakeText(context, "Received tempoTrackingUpdate " + tempoTrackingUpdate.ToJson(), ToastLength.Short).Show();
        }

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
