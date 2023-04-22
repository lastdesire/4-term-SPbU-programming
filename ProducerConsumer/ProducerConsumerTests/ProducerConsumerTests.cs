using ProducerConsumer;
using ProducerConsumer.Lock;
using NUnit.Framework;

namespace ProducerConsumerTests
{
    public class ProducerConsumerTests
    {
        private List<PCObject> _objectList;
        private TASLock _locker;
        
        public ProducerConsumerTests()
        {
            _objectList = new List<PCObject>();
            _locker = new TASLock();
        }
        
        [Test]
        public void ProducerTest()
        {
            _objectList.Clear();

            var producer = new Producer(_locker, _objectList, 0);
            Thread.Sleep(1);
            producer.Join();
            
            Assert.That(_objectList, Has.Count.EqualTo(1));
        }
        
        [Test]
        public void ConsumerTest()
        {
            _objectList.Clear();

            var consumer = new Consumer(_locker, _objectList, 0);
            Thread.Sleep(1);
            consumer.Join();
            
            Assert.That(_objectList, Has.Count.EqualTo(0));
        }
        
        [Test]
        public void ProducerConsumerTest()
        {
            _objectList.Clear();

            var producer = new Producer(_locker, _objectList, 0);
            
            
            var consumer = new Consumer(_locker, _objectList, 0);
            
            Thread.Sleep(1);
            producer.Join();
            consumer.Join();
            
            Assert.That(_objectList, Has.Count.EqualTo(0));
        }
    }
}
