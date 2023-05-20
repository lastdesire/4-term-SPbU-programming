using StudentsExamsLib.Hashing;

namespace StudentsExamsLib;

public class ExamSystem : IExamSystem
{
    private IHashSet<Exam> _hashSet;
    private const int Capacity = 32;
    private volatile int _count;
    public int Count => _count;

    public ExamSystem(IHashSet<Exam> hashSet)
    {
        this._hashSet = hashSet;
        _count = 0;
    }

    public void Add(long studentId, long courseId)
    {
        var student = new Exam(studentId, courseId);
        if (_hashSet.Add(student))
        {
            Interlocked.Increment(ref _count);
        }
    }

    public void Remove(long studentId, long courseId)
    {
        var exam = new Exam(studentId, courseId);
        if (_hashSet.Remove(exam))
        {
            Interlocked.Decrement(ref _count);
        }
    }

    public bool Contains(long studentId, long courseId)
    {
        var exam = new Exam(studentId, courseId);
        return _hashSet.Contains(exam);
    }
}
