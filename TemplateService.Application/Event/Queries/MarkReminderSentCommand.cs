using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateService.Application.Event.Queries;


public record MarkReminderSentCommand(Guid EventId, bool Is1DayReminder): IRequest;