using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TemplateService.Application.EventFeedback.DTOs;
using TemplateService.Application.EventFeedback.Queries;

namespace TemplateService.API.Controllers
{
    public class EventFeedbackController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
