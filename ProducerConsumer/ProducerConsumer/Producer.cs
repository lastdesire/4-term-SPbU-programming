using ProducerConsumer.Lock;

namespace ProducerConsumer;

public class Producer
{
    private Thread _thread;
    private ILock _locker;
    private List<PCObject> _listObjects;
    private int _producerIndex;
    private bool _isWorking;

    public Producer(ILock locker, List<PCObject> listObjects, int producerIndex)
    {
        _locker = locker;
        _listObjects = listObjects;
        _producerIndex = producerIndex;
        _isWorking = true;

        _thread = new Thread(Add);
        _thread.Start();
    }

    private void Add()
    {
        while (_isWorking)
        {
            var item = new PCObject();
            _locker.Lock();
            _listObjects.Add(item);
            Console.WriteLine($">>>>> The object ({item.Data}) was added by the producer with the index {_producerIndex}.");
            _locker.Unlock();

            Thread.Sleep(1500);
        }
    }
    
    public void Join()
    {
        _isWorking = false;
        _thread.Join();
    }
}
