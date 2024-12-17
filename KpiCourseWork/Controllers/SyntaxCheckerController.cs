using Antlr4.Runtime;
using BLL;
using KpiCourseWork.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace KpiCourseWork.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyntaxCheckerController : ControllerBase
    {
        [HttpPost("check-syntax-file")]
        public async Task<IActionResult> CheckSyntaxFromFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            string fileContent;
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                fileContent = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            }

            ICharStream inputStream = CharStreams.fromString(fileContent); 
            JavaScriptLexer lexer = new JavaScriptLexer(inputStream); 
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            JavaScriptParser parser = new JavaScriptParser(commonTokenStream);  
            
            var syntaxErrors = new List<SyntaxErrorInfo>();

            var errorListener = new SyntaxErrorListener(syntaxErrors);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(errorListener);

            try
            {
                parser.program();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while parsing: {ex.Message}");
            }

            if (syntaxErrors.Count > 0)
            {
                return Ok(new { errors = syntaxErrors });
            }

            return Ok("No syntax errors detected.");
        }
    }

    public class SyntaxErrorListener : IAntlrErrorListener<IToken>
    {
        private readonly List<SyntaxErrorInfo> _errors;

        public SyntaxErrorListener(List<SyntaxErrorInfo> errors)
        {
            _errors = errors;
        }

        public void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            _errors.Add(new SyntaxErrorInfo()
            {
                Message = msg,
                Line = line,
                Column = charPositionInLine,
                OffendingSymbol = offendingSymbol?.Text
                
            });
        }
    }

    
}