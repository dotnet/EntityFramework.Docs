using System;
using Notification;
using NotificationWithBase;
using Proxies;
using Snapshot;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Samples for _Change Detection and Notifications_");
        Console.WriteLine();

        SnapshotSamples.Snapshot_change_tracking_1();
        SnapshotSamples.Snapshot_change_tracking_2();

        NotificationEntitiesSamples.Notification_entities_1();
        NotificationWithBaseSamples.Notification_entities_2();

        ChangeTrackingProxiesSamples.Change_tracking_proxies_1();
    }
}
