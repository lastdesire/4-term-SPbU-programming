namespace StudentsExamsLibTests;
using NUnit.Framework;
using StudentsExamsLib;
using StudentsExamsLib.Hashing;

public class StudentsExamsLibTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestSetsForAllOperations()
    {
        var cHashSet = new CoarseHashSet<Exam>(50, new ExamComparator());
        var cExamSystem = new ExamSystem(cHashSet);

        var sHashSet = new StripedCuckooHashSet<Exam>(50, new ExamComparator());
        var sExamSystem = new ExamSystem(sHashSet);
        
        cExamSystem.Add(5,6);
        sExamSystem.Add(5, 6);
        Assert.Multiple(() =>
        {
            Assert.That(cExamSystem.Count, Is.EqualTo(1));
            Assert.That(sExamSystem.Count, Is.EqualTo(1));
        });
        
        cExamSystem.Add(5,6);
        sExamSystem.Add(5, 6);
        Assert.Multiple(() =>
        {
            Assert.That(cExamSystem.Count, Is.EqualTo(1));
            Assert.That(sExamSystem.Count, Is.EqualTo(1));
        });
        
        cExamSystem.Add(6,7);
        sExamSystem.Add(6, 7);
        Assert.Multiple(() =>
        {
            Assert.That(cExamSystem.Count, Is.EqualTo(2));
            Assert.That(sExamSystem.Count, Is.EqualTo(2));
        });

        cExamSystem.Add(1,1);
        sExamSystem.Add(1, 1);
        Assert.Multiple(() =>
        {
            Assert.That(cExamSystem.Count, Is.EqualTo(3));
            Assert.That(sExamSystem.Count, Is.EqualTo(3));
        });
        
        
        cExamSystem.Add(111,1111);
        sExamSystem.Add(111, 1111);
        Assert.Multiple(() =>
        {
            Assert.That(cExamSystem.Count, Is.EqualTo(4));
            Assert.That(sExamSystem.Count, Is.EqualTo(4));
        });
        
        cExamSystem.Add(5,6);
        sExamSystem.Add(5, 6);
        cExamSystem.Remove(5,6);
        sExamSystem.Remove(5, 6);
        cExamSystem.Remove(5,6);
        sExamSystem.Remove(5, 6);
        Assert.Multiple(() =>
        {
            Assert.That(cExamSystem.Count, Is.EqualTo(3));
            Assert.That(sExamSystem.Count, Is.EqualTo(3));
        });

        cExamSystem.Remove(111,1111);
        sExamSystem.Remove(111, 1111);
        Assert.Multiple(() =>
        {
            Assert.That(cExamSystem.Count, Is.EqualTo(2));
            Assert.That(sExamSystem.Count, Is.EqualTo(2));
        });
        
        cExamSystem.Remove(6,7);
        sExamSystem.Remove(6, 7);
        Assert.Multiple(() =>
        {
            Assert.That(cExamSystem.Count, Is.EqualTo(1));
            Assert.That(sExamSystem.Count, Is.EqualTo(1));
        });
        
        cExamSystem.Remove(0,100);
        sExamSystem.Remove(0, 100);
        Assert.Multiple(() =>
        {
            Assert.That(cExamSystem.Count, Is.EqualTo(1));
            Assert.That(sExamSystem.Count, Is.EqualTo(1));
        });
    }
}
