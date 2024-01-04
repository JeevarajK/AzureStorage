using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
    .Build();

var connectionString = config["appSettings:StorageConnectionString"];

QueueClient queueClient = new QueueClient(connectionString, "queue01");

await queueClient.CreateIfNotExistsAsync();

while (true)
{
    Console.WriteLine("Select operation");
    Console.WriteLine("1.Send Message");
    Console.WriteLine("2.Retrieve Message");
    Console.WriteLine("3.Exit");

    var option = Console.ReadLine();

    if (string.CompareOrdinal(option, "3") == 0)
    {
        Console.WriteLine("Exiting...\n");
        break;
    }

    switch (option)
    {
        case "1":
            Console.WriteLine("Enter Message:");
            var messageToSend = Console.ReadLine();
            //Add new message to the queue
            await queueClient.SendMessageAsync(messageToSend);

            Console.WriteLine("Message sent\n");
            break;
        case "2":

            //Read message and delete it from the queue
            var messageRecived = await queueClient.ReceiveMessageAsync();
            if (messageRecived.Value != null)
            {
                Console.WriteLine("Message retrieved : " + messageRecived.Value.Body + "\n");

                await queueClient.DeleteMessageAsync(messageRecived.Value.MessageId, messageRecived.Value.PopReceipt);
            }
            else
            {
                Console.WriteLine("No new messages\n");
            }
            break;
        default:
            Console.WriteLine("Invalid operation\n");
            break;
    }
}

//Finally delete the queue
await queueClient.DeleteIfExistsAsync();