namespace StudentsExamsLib;

public class Exam
{
    public long StudentId { get; }
    public long CourseId { get; }

    public Exam(long studentId, long courseId)
    {
        StudentId = studentId;
        CourseId = courseId;
    }
}

public class ExamComparator : IEqualityComparer<Exam>
{
    public bool Equals(Exam? exam, Exam? exam1)
    {
        if (exam == null || exam1 == null)
        {
            return false;
        }

        return (exam.StudentId == exam1.StudentId) && (exam.CourseId == exam1.CourseId);
    }

    public int GetHashCode(Exam exam)
    {
        return (int)exam.CourseId;
    }
}
