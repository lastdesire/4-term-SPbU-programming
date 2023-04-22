namespace ProducerConsumer.Lock
{
    public interface ILock
    {
        void Lock();

        void Unlock();
    }
}
