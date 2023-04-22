using ProducerConsumer.Lock;

namespace ProducerConsumer
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var rnd = new Random();
            
            var numberOfProducers = rnd.Next(1, 5);
            var numberOfConsumers = rnd.Next(1, 5);
            
            Console.WriteLine($"The number of producers: {numberOfProducers}.");
            Console.WriteLine($"The number of consumers: {numberOfConsumers}.");
            
            Console.WriteLine("You can click on any button to stop the process.");
            
            var locker = new TASLock();
            
            var objectList = new List<PCObject>();

            var producers = new Producer[numberOfProducers];
            for (var i = 0; i < numberOfProducers; i++)
            {
                producers[i] = new Producer(locker, objectList, i);
            }
            
            var consumers = new Consumer[numberOfConsumers];
            for (var i = 0; i < numberOfConsumers; i++)
            {
                consumers[i] = new Consumer(locker, objectList, i);
            }
           
            Console.ReadKey();

            consumers.ToList().ForEach(consumer => consumer.Join());
            producers.ToList().ForEach(producer => producer.Join());
            
            Console.WriteLine("/////////////////////////\nRemaining objects in the list:");
            objectList.ForEach(x => Console.WriteLine(x.Data));
            
            return 0;
        }
    }
}
