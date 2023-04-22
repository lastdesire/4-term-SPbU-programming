// Source: https://github.com/Stanislav-Sartasov/spsu-mm-se-programming/tree/main/Fourth%20term/LocksContinued/LocksContinued
namespace ProducerConsumer.Lock
{
    public class TASLock : ILock // test-and-set / compare-and-set / compare-exchange
    {
        private volatile int _state = 0;

        public void Lock()
        {
            while (Interlocked.CompareExchange(ref _state, 1, 0) == 1) { }
        }

        public void Unlock()
        {
            _state = 0;
        }
    }
}
