using FundManager.Common.Constants;
using FundManager.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace FundManager.DataAccess.Initalizer
{
    /// <summary>
    /// Seeds the Spa Consultation Form template with all 10 questions
    /// exactly as shown in the design mockup.
    /// </summary>
    public static class FormTemplateSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var seedAt = new DateTime(2026, 05, 19, 0, 0, 0, DateTimeKind.Utc);

            // ── FormTemplate ──────────────────────────────────────────────────
            modelBuilder.Entity<FormTemplate>().HasData(
            new FormTemplate
            {
                Id = 1,
                Title = "THE GRAND SPA CONSULTATION",
                Description = "Please take a moment to complete this form carefully, as your answers will help us provide your treatment safely and effectively:",
                LogoUrl = "/ApplicationImages/ed572294-335b-46bd-8df8-99de63864904.png",
                FooterText = "I acknowledge that treatments at The Grand Spa are non-medical. I confirm that I have accurately completed this consultation form and released The Grand Spa, the hotel, and its employees, from any liability or claims.\r\n\r\nI understand that cancellations or rescheduling must be made at least 24 hours in advance to avoid being charged 100% of the cost of the selected treatment.",
                AgreementText = "I agree to the Personal data processing notice",
                Version = 1,
                IsActive = true,
                IsDelete = false,
                CreatedAt = seedAt,
                UpdatedAt = seedAt,
                CreatedBy = CommonConstants.SystemUser,
                UpdatedBy = CommonConstants.SystemUser
            },
            new FormTemplate
            {
                Id = 2,
                Title = "THE MAIA CONSULTATION",
                Description = "Please take a moment to complete this form carefully, as your answers will help us provide your treatment safely and effectively:",
                LogoUrl = "/ApplicationImages/ed572294-335b-46bd-8df8-99de63864904.png",
                FooterText = "I acknowledge that treatments at The Maia Spa are non-medical. I confirm that I have accurately completed this consultation form and released The Maia Spa, the hotel, and its employees, from any liability or claims.\r\n\r\nI understand that cancellations or rescheduling must be made at least 24 hours in advance to avoid being charged 100% of the cost of the selected treatment.",
                AgreementText = "I agree to the Personal data processing notice",
                Version = 1,
                IsActive = true,
                IsDelete = false,
                CreatedAt = seedAt,
                UpdatedAt = seedAt,
                CreatedBy = CommonConstants.SystemUser,
                UpdatedBy = CommonConstants.SystemUser
            },
            new FormTemplate
            {
                Id = 3,
                Title = "THE LOTUS SPA CONSULTATION",
                Description = "Please take a moment to complete this form carefully, as your answers will help us provide your treatment safely and effectively:",
                LogoUrl = "/ApplicationImages/ed572294-335b-46bd-8df8-99de63864904.png",
                FooterText = "I acknowledge that treatments at The Lotus Spa are non-medical. I confirm that I have accurately completed this consultation form and released The Lotus Spa, the hotel, and its employees, from any liability or claims.\r\n\r\nI understand that cancellations or rescheduling must be made at least 24 hours in advance to avoid being charged 100% of the cost of the selected treatment.",
                AgreementText = "I agree to the Personal data processing notice",
                Version = 1,
                IsActive = true,
                IsDelete = false,
                CreatedAt = seedAt,
                UpdatedAt = seedAt,
                CreatedBy = CommonConstants.SystemUser,
                UpdatedBy = CommonConstants.SystemUser
            }
            );

            // ── FormQuestions ─────────────────────────────────────────────────
            modelBuilder.Entity<FormQuestion>().HasData(
                // Q1 – Have you experienced spa treatments before? (SingleChoice: Yes / No)
                new FormQuestion
                {
                    Id = 1,
                    FormTemplateId = 1,
                    SortOrder = 1,
                    QuestionText = "Have you experienced spa treatments before?",
                    QuestionType = QuestionType.SingleChoice,
                    IsRequired = true,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q2 – How are you feeling right now? (MultipleChoice)
                new FormQuestion
                {
                    Id = 2,
                    FormTemplateId = 1,
                    SortOrder = 2,
                    QuestionText = "How are you feeling right now?",
                    QuestionType = QuestionType.MultipleChoice,
                    IsRequired = false,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q3 – How do you want to feel after the treatment? (MultipleChoice)
                new FormQuestion
                {
                    Id = 3,
                    FormTemplateId = 1,
                    SortOrder = 3,
                    QuestionText = "How do you want to feel after the treatment?",
                    QuestionType = QuestionType.MultipleChoice,
                    IsRequired = false,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q4 – What treatment pressure do you prefer? (MultipleChoice)
                new FormQuestion
                {
                    Id = 4,
                    FormTemplateId = 1,
                    SortOrder = 4,
                    QuestionText = "For massages: what treatment pressure do you prefer?",
                    QuestionType = QuestionType.MultipleChoice,
                    IsRequired = false,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q5 – Body massage focus areas? + follow-up for facial massage (SingleChoice with follow-up)
                new FormQuestion
                {
                    Id = 5,
                    FormTemplateId = 1,
                    SortOrder = 5,
                    QuestionText = "For a body massage, are there any specific areas we should focus on?",
                    QuestionType = QuestionType.SingleChoice,
                    IsRequired = false,
                    HasFollowUpText = true,
                    FollowUpLabel = "For facial massage: Do you have any special skin concerns related to your face? If yes, please briefly describe:",
                    FollowUpTriggerOption = "Yes",
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q6 – Areas to avoid (TextInput)
                new FormQuestion
                {
                    Id = 6,
                    FormTemplateId = 1,
                    SortOrder = 6,
                    QuestionText = "For massages, are there any areas we should avoid?",
                    QuestionType = QuestionType.TextInput,
                    IsRequired = false,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q7 – Medical conditions (MultipleChoice)
                new FormQuestion
                {
                    Id = 7,
                    FormTemplateId = 1,
                    SortOrder = 7,
                    QuestionText = "Have you ever, or are you suffering from any of the following?",
                    QuestionType = QuestionType.MultipleChoice,
                    IsRequired = false,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q8 – Allergies (MultipleChoice)
                new FormQuestion
                {
                    Id = 8,
                    FormTemplateId = 1,
                    SortOrder = 8,
                    QuestionText = "Are you sensitive or allergic to any of the following?",
                    QuestionType = QuestionType.MultipleChoice,
                    IsRequired = false,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q9 – Recent operation? (SingleChoice + follow-up)
                new FormQuestion
                {
                    Id = 9,
                    FormTemplateId = 1,
                    SortOrder = 9,
                    QuestionText = "Have you recently had an operation?",
                    QuestionType = QuestionType.SingleChoice,
                    IsRequired = false,
                    HasFollowUpText = true,
                    FollowUpLabel = "If yes, please briefly describe:",
                    FollowUpTriggerOption = "Yes",
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q10 – Pregnant? (SingleChoice + follow-up)
                new FormQuestion
                {
                    Id = 10,
                    FormTemplateId = 1,
                    SortOrder = 10,
                    QuestionText = "For women: Are you pregnant?",
                    QuestionType = QuestionType.SingleChoice,
                    IsRequired = false,
                    HasFollowUpText = true,
                    FollowUpLabel = "If yes, which trimester?",
                    FollowUpTriggerOption = "Yes",
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // MAIA Spa
                // Q1 – Have you experienced spa treatments before? (SingleChoice: Yes / No)
                new FormQuestion
                {
                    Id = 21,
                    FormTemplateId = 3,
                    SortOrder = 1,
                    QuestionText = "Have you experienced spa treatments before?",
                    QuestionType = QuestionType.SingleChoice,
                    IsRequired = true,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q2 – How are you feeling right now? (MultipleChoice)
                new FormQuestion
                {
                    Id = 22,
                    FormTemplateId = 3,
                    SortOrder = 2,
                    QuestionText = "How are you feeling right now?",
                    QuestionType = QuestionType.MultipleChoice,
                    IsRequired = false,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q3 – How do you want to feel after the treatment? (MultipleChoice)
                new FormQuestion
                {
                    Id = 23,
                    FormTemplateId = 3,
                    SortOrder = 3,
                    QuestionText = "How do you want to feel after the treatment?",
                    QuestionType = QuestionType.MultipleChoice,
                    IsRequired = false,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q4 – What treatment pressure do you prefer? (MultipleChoice)
                new FormQuestion
                {
                    Id = 24,
                    FormTemplateId = 3,
                    SortOrder = 4,
                    QuestionText = "For massages: what treatment pressure do you prefer?",
                    QuestionType = QuestionType.MultipleChoice,
                    IsRequired = false,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q5 – Body massage focus areas? + follow-up for facial massage (SingleChoice with follow-up)
                new FormQuestion
                {
                    Id = 25,
                    FormTemplateId = 3,
                    SortOrder = 5,
                    QuestionText = "For a body massage, are there any specific areas we should focus on?",
                    QuestionType = QuestionType.SingleChoice,
                    IsRequired = false,
                    HasFollowUpText = true,
                    FollowUpLabel = "For facial massage: Do you have any special skin concerns related to your face? If yes, please briefly describe:",
                    FollowUpTriggerOption = "Yes",
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q6 – Areas to avoid (TextInput)
                new FormQuestion
                {
                    Id = 26,
                    FormTemplateId = 3,
                    SortOrder = 6,
                    QuestionText = "For massages, are there any areas we should avoid?",
                    QuestionType = QuestionType.TextInput,
                    IsRequired = false,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q7 – Medical conditions (MultipleChoice)
                new FormQuestion
                {
                    Id = 27,
                    FormTemplateId = 3,
                    SortOrder = 7,
                    QuestionText = "Have you ever, or are you suffering from any of the following?",
                    QuestionType = QuestionType.MultipleChoice,
                    IsRequired = false,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q8 – Allergies (MultipleChoice)
                new FormQuestion
                {
                    Id = 28,
                    FormTemplateId = 3,
                    SortOrder = 8,
                    QuestionText = "Are you sensitive or allergic to any of the following?",
                    QuestionType = QuestionType.MultipleChoice,
                    IsRequired = false,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q9 – Recent operation? (SingleChoice + follow-up)
                new FormQuestion
                {
                    Id = 29,
                    FormTemplateId = 3,
                    SortOrder = 9,
                    QuestionText = "Have you recently had an operation?",
                    QuestionType = QuestionType.SingleChoice,
                    IsRequired = false,
                    HasFollowUpText = true,
                    FollowUpLabel = "If yes, please briefly describe:",
                    FollowUpTriggerOption = "Yes",
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q10 – Pregnant? (SingleChoice + follow-up)
                new FormQuestion
                {
                    Id = 30,
                    FormTemplateId = 3,
                    SortOrder = 10,
                    QuestionText = "For women: Are you pregnant?",
                    QuestionType = QuestionType.SingleChoice,
                    IsRequired = false,
                    HasFollowUpText = true,
                    FollowUpLabel = "If yes, which trimester?",
                    FollowUpTriggerOption = "Yes",
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Lotus Spa
                // Q1 – Have you experienced spa treatments before? (SingleChoice: Yes / No)
                new FormQuestion
                {
                    Id = 11,
                    FormTemplateId = 2,
                    SortOrder = 1,
                    QuestionText = "Have you experienced spa treatments before?",
                    QuestionType = QuestionType.SingleChoice,
                    IsRequired = true,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q2 – How are you feeling right now? (MultipleChoice)
                new FormQuestion
                {
                    Id = 12,
                    FormTemplateId = 2,
                    SortOrder = 2,
                    QuestionText = "How are you feeling right now?",
                    QuestionType = QuestionType.MultipleChoice,
                    IsRequired = false,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q3 – How do you want to feel after the treatment? (MultipleChoice)
                new FormQuestion
                {
                    Id = 13,
                    FormTemplateId = 2,
                    SortOrder = 3,
                    QuestionText = "How do you want to feel after the treatment?",
                    QuestionType = QuestionType.MultipleChoice,
                    IsRequired = false,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q4 – What treatment pressure do you prefer? (MultipleChoice)
                new FormQuestion
                {
                    Id = 14,
                    FormTemplateId = 2,
                    SortOrder = 4,
                    QuestionText = "For massages: what treatment pressure do you prefer?",
                    QuestionType = QuestionType.MultipleChoice,
                    IsRequired = false,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q5 – Body massage focus areas? + follow-up for facial massage (SingleChoice with follow-up)
                new FormQuestion
                {
                    Id = 15,
                    FormTemplateId = 2,
                    SortOrder = 5,
                    QuestionText = "For a body massage, are there any specific areas we should focus on?",
                    QuestionType = QuestionType.SingleChoice,
                    IsRequired = false,
                    HasFollowUpText = true,
                    FollowUpLabel = "For facial massage: Do you have any special skin concerns related to your face? If yes, please briefly describe:",
                    FollowUpTriggerOption = "Yes",
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q6 – Areas to avoid (TextInput)
                new FormQuestion
                {
                    Id = 16,
                    FormTemplateId = 2,
                    SortOrder = 6,
                    QuestionText = "For massages, are there any areas we should avoid?",
                    QuestionType = QuestionType.TextInput,
                    IsRequired = false,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q7 – Medical conditions (MultipleChoice)
                new FormQuestion
                {
                    Id = 17,
                    FormTemplateId = 2,
                    SortOrder = 7,
                    QuestionText = "Have you ever, or are you suffering from any of the following?",
                    QuestionType = QuestionType.MultipleChoice,
                    IsRequired = false,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q8 – Allergies (MultipleChoice)
                new FormQuestion
                {
                    Id = 18,
                    FormTemplateId = 2,
                    SortOrder = 8,
                    QuestionText = "Are you sensitive or allergic to any of the following?",
                    QuestionType = QuestionType.MultipleChoice,
                    IsRequired = false,
                    HasFollowUpText = false,
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q9 – Recent operation? (SingleChoice + follow-up)
                new FormQuestion
                {
                    Id = 19,
                    FormTemplateId = 2,
                    SortOrder = 9,
                    QuestionText = "Have you recently had an operation?",
                    QuestionType = QuestionType.SingleChoice,
                    IsRequired = false,
                    HasFollowUpText = true,
                    FollowUpLabel = "If yes, please briefly describe:",
                    FollowUpTriggerOption = "Yes",
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },
                // Q10 – Pregnant? (SingleChoice + follow-up)
                new FormQuestion
                {
                    Id = 20,
                    FormTemplateId = 2,
                    SortOrder = 10,
                    QuestionText = "For women: Are you pregnant?",
                    QuestionType = QuestionType.SingleChoice,
                    IsRequired = false,
                    HasFollowUpText = true,
                    FollowUpLabel = "If yes, which trimester?",
                    FollowUpTriggerOption = "Yes",
                    IsDelete = false,
                    IsActive = true,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                }
            );

            // ── FormQuestionOptions ───────────────────────────────────────────
            modelBuilder.Entity<FormQuestionOption>().HasData(
                // Q1 options
                new FormQuestionOption { Id = 1, FormQuestionId = 1, OptionText = "Yes", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 2, FormQuestionId = 1, OptionText = "No", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q2 options
                new FormQuestionOption { Id = 3, FormQuestionId = 2, OptionText = "Tired", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 4, FormQuestionId = 2, OptionText = "Stressed", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 5, FormQuestionId = 2, OptionText = "Muscle Tension", SortOrder = 3, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 6, FormQuestionId = 2, OptionText = "Calm", SortOrder = 4, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q3 options
                new FormQuestionOption { Id = 7, FormQuestionId = 3, OptionText = "Peaceful", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 8, FormQuestionId = 3, OptionText = "Refreshed", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 9, FormQuestionId = 3, OptionText = "Vibrant", SortOrder = 3, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 10, FormQuestionId = 3, OptionText = "Energized", SortOrder = 4, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q4 options
                new FormQuestionOption { Id = 11, FormQuestionId = 4, OptionText = "Strong", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 12, FormQuestionId = 4, OptionText = "Medium", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 13, FormQuestionId = 4, OptionText = "Light", SortOrder = 3, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 14, FormQuestionId = 4, OptionText = "Don't know", SortOrder = 4, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q5 options (Yes / No)
                new FormQuestionOption { Id = 15, FormQuestionId = 5, OptionText = "Yes", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 16, FormQuestionId = 5, OptionText = "No", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q7 options – medical conditions
                new FormQuestionOption { Id = 17, FormQuestionId = 7, OptionText = "Diabetes", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 18, FormQuestionId = 7, OptionText = "Epilepsy", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 19, FormQuestionId = 7, OptionText = "Asthma", SortOrder = 3, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 20, FormQuestionId = 7, OptionText = "Fainting", SortOrder = 4, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 21, FormQuestionId = 7, OptionText = "Muscle Aches", SortOrder = 5, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 22, FormQuestionId = 7, OptionText = "Digestive Problems", SortOrder = 6, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 23, FormQuestionId = 7, OptionText = "High/ Low Blood Pressure", SortOrder = 7, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 24, FormQuestionId = 7, OptionText = "Skin Diseases", SortOrder = 8, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q8 options – allergens
                new FormQuestionOption { Id = 25, FormQuestionId = 8, OptionText = "Food", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 26, FormQuestionId = 8, OptionText = "Medication", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 27, FormQuestionId = 8, OptionText = "Essential Oils", SortOrder = 3, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q9 options
                new FormQuestionOption { Id = 28, FormQuestionId = 9, OptionText = "Yes", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 29, FormQuestionId = 9, OptionText = "No", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q10 options
                new FormQuestionOption { Id = 30, FormQuestionId = 10, OptionText = "Yes", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 31, FormQuestionId = 10, OptionText = "No", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Maia Spa Q1 options
                // Q1 options
                new FormQuestionOption { Id = 32, FormQuestionId = 11, OptionText = "Yes", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 33, FormQuestionId = 11, OptionText = "No", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q2 options
                new FormQuestionOption { Id = 34, FormQuestionId = 12, OptionText = "Tired", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 35, FormQuestionId = 12, OptionText = "Stressed", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 36, FormQuestionId = 12, OptionText = "Muscle Tension", SortOrder = 3, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 37, FormQuestionId = 12, OptionText = "Calm", SortOrder = 4, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q3 options
                new FormQuestionOption { Id = 38, FormQuestionId = 13, OptionText = "Peaceful", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 39, FormQuestionId = 13, OptionText = "Refreshed", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 40, FormQuestionId = 13, OptionText = "Vibrant", SortOrder = 3, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 41, FormQuestionId = 13, OptionText = "Energized", SortOrder = 4, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q4 options
                new FormQuestionOption { Id = 42, FormQuestionId = 14, OptionText = "Strong", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 43, FormQuestionId = 14, OptionText = "Medium", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 44, FormQuestionId = 14, OptionText = "Light", SortOrder = 3, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 45, FormQuestionId = 14, OptionText = "Don't know", SortOrder = 4, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q5 options (Yes / No)
                new FormQuestionOption { Id = 46, FormQuestionId = 15, OptionText = "Yes", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 47, FormQuestionId = 15, OptionText = "No", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q7 options – medical conditions
                new FormQuestionOption { Id = 48, FormQuestionId = 17, OptionText = "Diabetes", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 49, FormQuestionId = 17, OptionText = "Epilepsy", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 50, FormQuestionId = 17, OptionText = "Asthma", SortOrder = 3, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 51, FormQuestionId = 17, OptionText = "Fainting", SortOrder = 4, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 52, FormQuestionId = 17, OptionText = "Muscle Aches", SortOrder = 5, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 53, FormQuestionId = 17, OptionText = "Digestive Problems", SortOrder = 6, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 54, FormQuestionId = 17, OptionText = "High/ Low Blood Pressure", SortOrder = 7, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 55, FormQuestionId = 17, OptionText = "Skin Diseases", SortOrder = 8, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q8 options – allergens
                new FormQuestionOption { Id = 56, FormQuestionId = 18, OptionText = "Food", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 57, FormQuestionId = 18, OptionText = "Medication", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 58, FormQuestionId = 18, OptionText = "Essential Oils", SortOrder = 3, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q9 options
                new FormQuestionOption { Id = 59, FormQuestionId = 19, OptionText = "Yes", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 60, FormQuestionId = 19, OptionText = "No", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q10 options
                new FormQuestionOption { Id = 61, FormQuestionId = 20, OptionText = "Yes", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 62, FormQuestionId = 20, OptionText = "No", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Lotus Spa Q1 options
                // Q1 options
                new FormQuestionOption { Id = 63, FormQuestionId = 21, OptionText = "Yes", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 64, FormQuestionId = 21, OptionText = "No", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q2 options
                new FormQuestionOption { Id = 65, FormQuestionId = 22, OptionText = "Tired", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 66, FormQuestionId = 22, OptionText = "Stressed", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 67, FormQuestionId = 22, OptionText = "Muscle Tension", SortOrder = 3, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 68, FormQuestionId = 22, OptionText = "Calm", SortOrder = 4, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q3 options
                new FormQuestionOption { Id = 69, FormQuestionId = 23, OptionText = "Peaceful", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 70, FormQuestionId = 23, OptionText = "Refreshed", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 71, FormQuestionId = 23, OptionText = "Vibrant", SortOrder = 3, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 72, FormQuestionId = 23, OptionText = "Energized", SortOrder = 4, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q4 options
                new FormQuestionOption { Id = 73, FormQuestionId = 24, OptionText = "Strong", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 74, FormQuestionId = 24, OptionText = "Medium", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 75, FormQuestionId = 24, OptionText = "Light", SortOrder = 3, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 76, FormQuestionId = 24, OptionText = "Don't know", SortOrder = 4, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q5 options (Yes / No)
                new FormQuestionOption { Id = 77, FormQuestionId = 25, OptionText = "Yes", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 78, FormQuestionId = 25, OptionText = "No", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q7 options – medical conditions
                new FormQuestionOption { Id = 79, FormQuestionId = 27, OptionText = "Diabetes", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 80, FormQuestionId = 27, OptionText = "Epilepsy", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 81, FormQuestionId = 27, OptionText = "Asthma", SortOrder = 3, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 82, FormQuestionId = 27, OptionText = "Fainting", SortOrder = 4, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 83, FormQuestionId = 27, OptionText = "Muscle Aches", SortOrder = 5, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 84, FormQuestionId = 27, OptionText = "Digestive Problems", SortOrder = 6, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 85, FormQuestionId = 27, OptionText = "High/ Low Blood Pressure", SortOrder = 7, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 86, FormQuestionId = 27, OptionText = "Skin Diseases", SortOrder = 8, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q8 options – allergens
                new FormQuestionOption { Id = 87, FormQuestionId = 28, OptionText = "Food", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 88, FormQuestionId = 28, OptionText = "Medication", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 89, FormQuestionId = 28, OptionText = "Essential Oils", SortOrder = 3, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q9 options
                new FormQuestionOption { Id = 90, FormQuestionId = 29, OptionText = "Yes", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 91, FormQuestionId = 29, OptionText = "No", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },

                // Q10 options
                new FormQuestionOption { Id = 92, FormQuestionId = 30, OptionText = "Yes", SortOrder = 1, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser },
                new FormQuestionOption { Id = 93, FormQuestionId = 30, OptionText = "No", SortOrder = 2, IsDelete = false, IsActive = true, CreatedAt = seedAt, UpdatedAt = seedAt, CreatedBy = CommonConstants.SystemUser, UpdatedBy = CommonConstants.SystemUser }
            );
        }
    }
}
