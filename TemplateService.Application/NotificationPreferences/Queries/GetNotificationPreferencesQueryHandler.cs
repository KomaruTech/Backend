using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TemplateService.Application.NotificationPreferences.Dtos;

namespace TemplateService.Application.NotificationPreferences.Queries
{
    public class GetNotificationPreferencesQueryHandler : IRequestHandler<GetNotificationPreferencesQuery, NotificationPreferencesDto>
    {
        public async Task<NotificationPreferencesDto> Handle(GetNotificationPreferencesQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new NotificationPreferencesDto { Id = request.Id });
        }
    }
}
