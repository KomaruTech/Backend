using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateService.Application.Event.DTOs;

namespace TemplateService.Application.Event.Queries;
public record GetEventsForRemindersQuery(DateTime CurrentTime) : IRequest<List<EventDto>>;