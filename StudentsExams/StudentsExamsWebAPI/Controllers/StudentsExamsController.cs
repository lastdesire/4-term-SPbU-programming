// Usage:
// http://localhost/StudentsExams/Count
// http://localhost/StudentsExams/add?studentId=0&courseId=0
// http://localhost/StudentsExams/remove?studentId=0&courseId=0
// http://localhost/StudentsExams/contains?studentId=0&courseId=0

using Microsoft.AspNetCore.Mvc;
using StudentsExamsLib;
using StudentsExamsLib.Hashing;

namespace StudentsExamsWebAPI.Controllers;

[Route("StudentsExams")]
[ApiController]
public class ExamSystemController : ControllerBase
{
    private static IExamSystem? _examSystem = null;

    public ExamSystemController()
    {
        if (_examSystem != null) return;
        // You can change CoarseHashSet to StripedCuckooHashSet right here. 
        var x = new CoarseHashSet<Exam>(50, new ExamComparator());
        _examSystem = new ExamSystem(x);
    }

    [HttpGet("contains", Name = "Contains")]
    public ActionResult<bool> Contains(long studentId, long courseId)
    {
        bool result;
        try
        {
            result = _examSystem!.Contains(studentId, courseId);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }

        return Ok(result);
    }

    [HttpGet("add", Name = "Add")]
    public ActionResult Add(long studentId, long courseId)
    {
        try
        {
            var savedCount = _examSystem!.Count;
            _examSystem!.Add(studentId, courseId);
            return Ok(savedCount != _examSystem.Count ? "Added." : "Exist.");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("remove", Name = "Remove")]
    public ActionResult Remove(long studentId, long courseId)
    {
        try
        {
            var savedCount = _examSystem!.Count;
            _examSystem!.Remove(studentId, courseId);
            return Ok(savedCount != _examSystem.Count ? "Removed." : "Does not exist.");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("count", Name = "Count")]
    public ActionResult<int> Count()
    {
        int count;
        try
        {
            count = _examSystem!.Count;
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }

        return Ok(count);
    }
}
