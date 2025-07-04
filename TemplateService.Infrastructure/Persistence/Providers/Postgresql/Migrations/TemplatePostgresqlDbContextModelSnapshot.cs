﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TemplateService.Infrastructure.Persistence.Providers.Postgresql;

#nullable disable

namespace TemplateService.Infrastructure.Persistence.Providers.Postgresql.Migrations
{
    [DbContext(typeof(TemplatePostgresqlDbContext))]
    partial class TemplatePostgresqlDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("DEFAULT")
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TemplateService.Domain.Entities.EventEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uuid")
                        .HasColumnName("created_by_id");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Keywords")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("jsonb")
                        .HasColumnName("keywords")
                        .HasDefaultValueSql("'[]'::jsonb");

                    b.Property<string>("Location")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)")
                        .HasColumnName("location");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("name");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("status");

                    b.Property<DateTime?>("TimeEnd")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("time_end");

                    b.Property<DateTime>("TimeStart")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("time_start");

                    b.Property<string>("Type")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasDefaultValue("general")
                        .HasColumnName("type");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("Status");

                    b.HasIndex("Type");

                    b.ToTable("events", "DEFAULT", t =>
                        {
                            t.HasComment("Мероприятия");
                        });
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.EventFeedbackEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Comment")
                        .HasColumnType("text")
                        .HasColumnName("comment");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("now()");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid")
                        .HasColumnName("event_id");

                    b.Property<short>("Rating")
                        .HasColumnType("smallint")
                        .HasColumnName("rating");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("UserId");

                    b.ToTable("event_feedback", "DEFAULT", t =>
                        {
                            t.HasComment("Отзывы на мероприятия");

                            t.HasCheckConstraint("CK_event_feedback_rating_range", "rating BETWEEN 1 AND 5");
                        });
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.EventNotificationsEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid");

                    b.Property<string>("NotificationType")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("UserId", "EventId", "NotificationType");

                    b.ToTable("event_notifications", "DEFAULT");
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.EventParticipantEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid")
                        .HasColumnName("event_id");

                    b.Property<string>("AttendanceResponse")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("attendance_response")
                        .HasComment("Статус, принял ли пользователь приглашение или отказал, или не ответил");

                    b.Property<bool>("IsSpeaker")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("is_speaker")
                        .HasComment("Флаг, выступает ли пользователь на мероприятии");

                    b.HasKey("UserId", "EventId");

                    b.HasIndex("EventId");

                    b.ToTable("event_participants", "DEFAULT", t =>
                        {
                            t.HasComment("Связь пользователей с мероприятиями");
                        });
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.EventPhotoEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid")
                        .HasColumnName("event_id");

                    b.Property<byte[]>("Image")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("image");

                    b.Property<string>("MimeType")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("mime_type");

                    b.Property<DateTime>("UploadedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("uploaded_at")
                        .HasDefaultValueSql("now()");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.ToTable("event_photos", "DEFAULT", t =>
                        {
                            t.HasComment("Фотографии, прикреплённые к событиям");
                        });
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.EventTeamsEntity", b =>
                {
                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid")
                        .HasColumnName("event_id");

                    b.Property<Guid>("TeamId")
                        .HasColumnType("uuid")
                        .HasColumnName("group_id");

                    b.HasKey("EventId", "TeamId");

                    b.HasIndex("TeamId");

                    b.ToTable("event_groups", "DEFAULT", t =>
                        {
                            t.HasComment("Связь мероприятий с группами");
                        });
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.NotificationPreferencesEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<bool>("NotifyEmail")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("notify_email");

                    b.Property<bool>("NotifyTelegram")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("notify_telegram");

                    b.Property<bool>("ReminderBefore1Day")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true)
                        .HasColumnName("reminder_before_1day");

                    b.Property<bool>("ReminderBefore1Hour")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true)
                        .HasColumnName("reminder_before_1hour");

                    b.HasKey("Id");

                    b.ToTable("notification_preferences", "DEFAULT", t =>
                        {
                            t.HasComment("Предпочтения пользователей");
                        });
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.SpeakerApplicationEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Comment")
                        .HasColumnType("text")
                        .HasColumnName("comment");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("now()");

                    b.Property<Guid>("EventId")
                        .HasColumnType("uuid")
                        .HasColumnName("event_id");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasDefaultValue("pending")
                        .HasColumnName("status");

                    b.Property<string>("Topic")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("topic");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("Status");

                    b.HasIndex("UserId");

                    b.ToTable("speaker_applications", "DEFAULT", t =>
                        {
                            t.HasComment("Заявки на выступление");
                        });
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.TeamsEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid")
                        .HasColumnName("owner_id");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("OwnerId");

                    b.ToTable("teams", "DEFAULT", t =>
                        {
                            t.HasComment("Команды (Группы)");
                        });
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<byte[]>("Avatar")
                        .HasColumnType("bytea")
                        .HasColumnName("avatar");

                    b.Property<string>("AvatarMimeType")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("avatar_mime_type");

                    b.Property<string>("Email")
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("email");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("login");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("name");

                    b.Property<Guid>("NotificationPreferencesId")
                        .HasColumnType("uuid")
                        .HasColumnName("notification_preferences_id");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("password_hash");

                    b.Property<string>("Role")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasDefaultValue("member")
                        .HasColumnName("role");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)")
                        .HasColumnName("surname");

                    b.Property<long?>("TelegramId")
                        .HasColumnType("bigint")
                        .HasColumnName("telegram_id");

                    b.Property<string>("TelegramUsername")
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)")
                        .HasColumnName("telegram_username");

                    b.HasKey("Id");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.HasIndex("NotificationPreferencesId")
                        .IsUnique();

                    b.HasIndex("Role");

                    b.HasIndex("TelegramId")
                        .IsUnique();

                    b.HasIndex("TelegramUsername")
                        .IsUnique();

                    b.ToTable("users", "DEFAULT", t =>
                        {
                            t.HasComment("Пользователи");
                        });
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.UserTeamsEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<Guid>("TeamId")
                        .HasColumnType("uuid")
                        .HasColumnName("team_id");

                    b.HasKey("UserId", "TeamId");

                    b.HasIndex("TeamId");

                    b.ToTable("user_teams", "DEFAULT", t =>
                        {
                            t.HasComment("Связь пользователей и команд");
                        });
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.EventEntity", b =>
                {
                    b.HasOne("TemplateService.Domain.Entities.UserEntity", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.EventFeedbackEntity", b =>
                {
                    b.HasOne("TemplateService.Domain.Entities.EventEntity", "Event")
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TemplateService.Domain.Entities.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.EventNotificationsEntity", b =>
                {
                    b.HasOne("TemplateService.Domain.Entities.EventEntity", "Event")
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TemplateService.Domain.Entities.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.EventParticipantEntity", b =>
                {
                    b.HasOne("TemplateService.Domain.Entities.EventEntity", "Event")
                        .WithMany("Participants")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TemplateService.Domain.Entities.UserEntity", "User")
                        .WithMany("EventParticipants")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.EventPhotoEntity", b =>
                {
                    b.HasOne("TemplateService.Domain.Entities.EventEntity", "Event")
                        .WithMany("Photos")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.EventTeamsEntity", b =>
                {
                    b.HasOne("TemplateService.Domain.Entities.EventEntity", "Event")
                        .WithMany("EventTeams")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TemplateService.Domain.Entities.TeamsEntity", "Team")
                        .WithMany("EventTeams")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.SpeakerApplicationEntity", b =>
                {
                    b.HasOne("TemplateService.Domain.Entities.EventEntity", "Event")
                        .WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TemplateService.Domain.Entities.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.TeamsEntity", b =>
                {
                    b.HasOne("TemplateService.Domain.Entities.UserEntity", "Owner")
                        .WithMany("CreatedTeams")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.UserEntity", b =>
                {
                    b.HasOne("TemplateService.Domain.Entities.NotificationPreferencesEntity", "NotificationPreferences")
                        .WithOne()
                        .HasForeignKey("TemplateService.Domain.Entities.UserEntity", "NotificationPreferencesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("NotificationPreferences");
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.UserTeamsEntity", b =>
                {
                    b.HasOne("TemplateService.Domain.Entities.TeamsEntity", "Team")
                        .WithMany("Users")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TemplateService.Domain.Entities.UserEntity", "User")
                        .WithMany("Teams")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Team");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.EventEntity", b =>
                {
                    b.Navigation("EventTeams");

                    b.Navigation("Participants");

                    b.Navigation("Photos");
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.TeamsEntity", b =>
                {
                    b.Navigation("EventTeams");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("TemplateService.Domain.Entities.UserEntity", b =>
                {
                    b.Navigation("CreatedTeams");

                    b.Navigation("EventParticipants");

                    b.Navigation("Teams");
                });
#pragma warning restore 612, 618
        }
    }
}
