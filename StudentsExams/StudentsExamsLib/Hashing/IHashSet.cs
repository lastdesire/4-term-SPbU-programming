namespace StudentsExamsLib.Hashing;

public interface IHashSet<T>
{
    public bool Contains(T x);
    public bool Add(T x);
    public bool Remove(T x);
}
