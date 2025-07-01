using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateService.Application.NotificationPreferences.Dtos;


public record NotificationPreferencesDto(
     Guid Id,
     bool NotifyTelegram,
     bool NotifyEmail,
     bool ReminderBefore1Day,
     bool ReminderBefore1Hour );