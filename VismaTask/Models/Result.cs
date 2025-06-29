using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VismaTask.Models;

public class Result
{
    public bool Success { get; }
    public string Message { get; }

    private Result(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public static Result Ok(string message = "") => new Result(true, message);
    public static Result Fail(string message) => new Result(false, message);
}
