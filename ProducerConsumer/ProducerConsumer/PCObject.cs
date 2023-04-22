namespace ProducerConsumer;

public class PCObject
{
    public string Data { get; }

    public PCObject()
    {
        var rnd = new Random();
        Data = $"Just a string that contains a random number [{rnd.Next(1, 1000)}]";
    }
}
