using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TemplateService.Application.NotificationPreferences.Dtos;

namespace TemplateService.Application.NotificationPreferences.Queries
{
    public class GetNotificationPreferencesQuery : IRequest<NotificationPreferencesDto>
    {
        public Guid Id { get; set; }

        public GetNotificationPreferencesQuery(Guid id)
        {
            Id = id;
        }

        public GetNotificationPreferencesQuery() { }
    }
}
