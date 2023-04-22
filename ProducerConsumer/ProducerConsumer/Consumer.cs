using ProducerConsumer.Lock;

namespace ProducerConsumer;

public class Consumer
{
    private Thread _thread;
    private ILock _locker;
    private List<PCObject> _listObjects;
    private int _consumerIndex;
    private bool _isWorking;

    public Consumer(ILock locker, List<PCObject> listObjects, int consumerIndex)
    {
        _locker = locker;
        _listObjects = listObjects;
        _consumerIndex = consumerIndex;
        _isWorking = true;

        _thread = new Thread(Remove);
        _thread.Start();
    }

    private void Remove()
    {
        while (_isWorking)
        {
            _locker.Lock();
            if (0 != _listObjects.Count)
            {
                Console.WriteLine($"The object ({_listObjects.First().Data}) was removed by the consumer with the index {_consumerIndex}.");
                _listObjects.RemoveAt(0);
            }
            else
            {
                Console.WriteLine($"Consumer with the index {_consumerIndex} wanted to remove something but there is an empty list.");
            }
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
