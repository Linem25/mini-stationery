using Microsoft.AspNetCore.Mvc;
using MiniStationery.Mvc.ViewModels;
using System.Text.RegularExpressions;

namespace MiniStationery.Mvc.Controllers;

public class AuditLogsController : Controller
{
    public IActionResult Index()
    {
        var logs = new List<AuditLogItemViewModel>();
        var path = $"logs/stationery-{DateTime.Now:yyyyMMdd}.txt";

        if (System.IO.File.Exists(path))
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(stream);

            string? line;
            var pattern = @"^\[(?<time>[\d\-:. ]+)\s(?<level>\w+)\]\s(?<message>.+)$";

            while ((line = reader.ReadLine()) != null)
            {
                var match = Regex.Match(line, pattern);
                if (match.Success)
                {
                    logs.Add(new AuditLogItemViewModel
                    {
                        Time = match.Groups["time"].Value.Trim(),
                        Level = match.Groups["level"].Value.Trim(),
                        Message = match.Groups["message"].Value.Trim()
                    });
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    logs.Add(new AuditLogItemViewModel
                    {
                        Time = "",
                        Level = "Info",
                        Message = line
                    });
                }
            }
        }

        logs.Reverse();
        return View(logs);
    }
}