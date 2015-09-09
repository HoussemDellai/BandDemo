using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;
using Microsoft.WindowsAzure.Messaging;

namespace BandDemo.PushNotifications
{
   public static class BandDemoPush
    {

        public async static Task RegisterForPush()
        {
            var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

            NotificationHub hub = new NotificationHub(
              "banddemonotificationhub",
              "Endpoint=sb://banddemonotificationhub-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=f+Z7H8h+qauNbeck/QJUe6dAinbLZztCvu+3usaD/Fk=");

            await hub.RegisterNativeAsync(channel.Uri);
        }
    }
}
