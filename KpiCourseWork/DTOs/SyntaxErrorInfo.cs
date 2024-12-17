namespace KpiCourseWork.DTOs;

public class SyntaxErrorInfo
{
    public string Message { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }
    public string OffendingSymbol { get; set; }
}