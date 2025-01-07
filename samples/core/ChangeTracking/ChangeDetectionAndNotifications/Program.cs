using System;
using System.Threading.Tasks;
using Notification;
using NotificationWithBase;
using Proxies;
using Snapshot;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Samples for _Change Detection and Notifications_");
        Console.WriteLine();

        await SnapshotSamples.Snapshot_change_tracking_1();
        await SnapshotSamples.Snapshot_change_tracking_2();

        await NotificationEntitiesSamples.Notification_entities_1();
        await NotificationWithBaseSamples.Notification_entities_2();

        await ChangeTrackingProxiesSamples.Change_tracking_proxies_1();
    }
}
