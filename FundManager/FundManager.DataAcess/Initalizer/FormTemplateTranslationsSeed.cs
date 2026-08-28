using DigitalDocumentPlatform.Common.Constants;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace DigitalDocumentPlatform.DataAccess.Initalizer
{
    /// <summary>
    /// Seeds Vietnamese (vi) translations for the three Spa Consultation Form Templates.
    /// The QuestionsTranslation JSON follows the schema:
    ///   [{ "questionId": N, "questionText": "...", "options": [{ "optionId": N, "optionText": "..." }] }]
    /// Question/Option IDs must match the IDs seeded in FormTemplateSeed.cs.
    ///   Template 1 (The Grand Spa)  : Questions 1-10,  Options 1-31
    ///   Template 2 (The Maia)       : Questions 11-20, Options 32-62
    ///   Template 3 (The Lotus Spa)  : Questions 21-30, Options 63-93
    /// </summary>
    public static class FormTemplateTranslationsSeed
    {
        // Shared follow-up labels in Vietnamese
        private const string FollowUpDescribeBriefly = "Nếu có, vui lòng miêu tả ngắn gọn:";
        private const string FollowUpFacialConcern =
            "Với Massage mặt: Bạn có đang gặp phải bất kỳ vấn đề gì về da không? Nếu có, vui lòng miêu tả ngắn gọn:";
        private const string FollowUpTrimester = "Nếu có, Bạn đang ở quý thai kỳ nào?";

        // ── Vietnamese QuestionsTranslation JSON for Template 1 (Questions 1–10, Options 1–31) ──
        private const string ViQuestionsTemplate1 = """
            [
              {"questionId":1,"questionText":"Bạn đã từng có trải nghiệm trị liệu Spa trước đây chưa?","options":[{"optionId":1,"optionText":"Có"},{"optionId":2,"optionText":"Không"}]},
              {"questionId":2,"questionText":"Hiện tại bạn đang cảm thấy thế nào?","options":[{"optionId":3,"optionText":"Mệt Mỏi"},{"optionId":4,"optionText":"Căng Thẳng"},{"optionId":5,"optionText":"Nhức Cơ Bắp"},{"optionId":6,"optionText":"Bình Thường"}]},
              {"questionId":3,"questionText":"Bạn muốn cảm thấy thế nào sau khi trị liệu?","options":[{"optionId":7,"optionText":"Yên Bình"},{"optionId":8,"optionText":"Tươi Mới"},{"optionId":9,"optionText":"Phấn Khởi"},{"optionId":10,"optionText":"Đầy Năng Lượng"}]},
              {"questionId":4,"questionText":"Với Massage: Bạn mong muốn dùng lực thế nào?","options":[{"optionId":11,"optionText":"Mạnh"},{"optionId":12,"optionText":"Trung Bình"},{"optionId":13,"optionText":"Nhẹ"},{"optionId":14,"optionText":"Cần Thử Lực"}]},
              {"questionId":5,"questionText":"Với Massage thân thể: Có khu vực nào chúng tôi nên tập trung không?","followUpLabel":"Với Massage mặt: Bạn có đang gặp phải bất kỳ vấn đề gì về da không? Nếu có, vui lòng miêu tả ngắn gọn:","options":[{"optionId":15,"optionText":"Có"},{"optionId":16,"optionText":"Không"}]},
              {"questionId":6,"questionText":"Với massage: Bạn có muốn tránh bất cứ khu vực nào không?","options":[]},
              {"questionId":7,"questionText":"Bạn đã bao giờ hay đang gặp phải các vấn đề về sức khỏe dưới đây?","options":[{"optionId":17,"optionText":"Tiểu Đường"},{"optionId":18,"optionText":"Động Kinh"},{"optionId":19,"optionText":"Hen Suyễn"},{"optionId":20,"optionText":"Ngất Xỉu"},{"optionId":21,"optionText":"Đau Cơ"},{"optionId":22,"optionText":"Vấn Đề Tiêu Hóa"},{"optionId":23,"optionText":"Huyết Áp Cao/ Thấp"},{"optionId":24,"optionText":"Bệnh Về Da"}]},
              {"questionId":8,"questionText":"Bạn có bị dị ứng hoặc nhạy cảm với các thứ dưới đây không?","options":[{"optionId":25,"optionText":"Thức Ăn"},{"optionId":26,"optionText":"Thuốc"},{"optionId":27,"optionText":"Tinh Dầu"}]},
              {"questionId":9,"questionText":"Gần đây bạn có phẫu thuật không?","followUpLabel":"Nếu có, vui lòng miêu tả ngắn gọn:","options":[{"optionId":28,"optionText":"Có"},{"optionId":29,"optionText":"Không"}]},
              {"questionId":10,"questionText":"Với Phụ Nữ: Bạn đang có thai không?","followUpLabel":"Nếu có, Bạn đang ở quý thai kỳ nào?","options":[{"optionId":30,"optionText":"Có"},{"optionId":31,"optionText":"Không"}]}
            ]
            """;

        private const string EnQuestionsTemplate1 = """
            [
              {"questionId":1,"questionText":"Have you had a Spa treatment experience before?","options":[{"optionId":1,"optionText":"Yes"},{"optionId":2,"optionText":"No"}]},
              {"questionId":2,"questionText":"How are you feeling right now?","options":[{"optionId":3,"optionText":"Tired"},{"optionId":4,"optionText":"Stressed"},{"optionId":5,"optionText":"Muscle Aches"},{"optionId":6,"optionText":"Normal"}]},
              {"questionId":3,"questionText":"How would you like to feel after the treatment?","options":[{"optionId":7,"optionText":"Peaceful"},{"optionId":8,"optionText":"Refreshed"},{"optionId":9,"optionText":"Invigorated"},{"optionId":10,"optionText":"Energized"}]},
              {"questionId":4,"questionText":"For Massage: How would you like the pressure to be?","options":[{"optionId":11,"optionText":"Strong"},{"optionId":12,"optionText":"Medium"},{"optionId":13,"optionText":"Light"},{"optionId":14,"optionText":"Need to Test"}]},
              {"questionId":5,"questionText":"For Body Massage: Are there any areas we should focus on?","followUpLabel":"For Facial Massage: Are you currently experiencing any skin concerns? If yes, please describe briefly:","options":[{"optionId":15,"optionText":"Yes"},{"optionId":16,"optionText":"No"}]},
              {"questionId":6,"questionText":"For massage: Are there any areas you would like to avoid?","options":[]},
              {"questionId":7,"questionText":"Have you ever had or are currently experiencing any of the following health issues?","options":[{"optionId":17,"optionText":"Diabetes"},{"optionId":18,"optionText":"Epilepsy"},{"optionId":19,"optionText":"Asthma"},{"optionId":20,"optionText":"Fainting"},{"optionId":21,"optionText":"Muscle Pain"},{"optionId":22,"optionText":"Digestive Issues"},{"optionId":23,"optionText":"High/Low Blood Pressure"},{"optionId":24,"optionText":"Skin Conditions"}]},
              {"questionId":8,"questionText":"Are you allergic or sensitive to any of the following?","options":[{"optionId":25,"optionText":"Food"},{"optionId":26,"optionText":"Medication"},{"optionId":27,"optionText":"Essential Oils"}]},
              {"questionId":9,"questionText":"Have you had any recent surgeries?","followUpLabel":"If yes, please describe briefly:","options":[{"optionId":28,"optionText":"Yes"},{"optionId":29,"optionText":"No"}]},
              {"questionId":10,"questionText":"For Women: Are you currently pregnant?","followUpLabel":"If yes, which trimester are you in?","options":[{"optionId":30,"optionText":"Yes"},{"optionId":31,"optionText":"No"}]}
            ]
            """;

        // ── Vietnamese QuestionsTranslation JSON for Template 2 (Maia – Questions 11–20, Options 32–62) ──
        private const string ViQuestionsTemplate2 = """
            [
              {"questionId":11,"questionText":"Bạn đã từng có trải nghiệm trị liệu Spa trước đây chưa?","options":[{"optionId":32,"optionText":"Có"},{"optionId":33,"optionText":"Không"}]},
              {"questionId":12,"questionText":"Hiện tại bạn đang cảm thấy thế nào?","options":[{"optionId":34,"optionText":"Mệt Mỏi"},{"optionId":35,"optionText":"Căng Thẳng"},{"optionId":36,"optionText":"Nhức Cơ Bắp"},{"optionId":37,"optionText":"Bình Thường"}]},
              {"questionId":13,"questionText":"Bạn muốn cảm thấy thế nào sau khi trị liệu?","options":[{"optionId":38,"optionText":"Yên Bình"},{"optionId":39,"optionText":"Tươi Mới"},{"optionId":40,"optionText":"Phấn Khởi"},{"optionId":41,"optionText":"Đầy Năng Lượng"}]},
              {"questionId":14,"questionText":"Với Massage: Bạn mong muốn dùng lực thế nào?","options":[{"optionId":42,"optionText":"Mạnh"},{"optionId":43,"optionText":"Trung Bình"},{"optionId":44,"optionText":"Nhẹ"},{"optionId":45,"optionText":"Cần Thử Lực"}]},
              {"questionId":15,"questionText":"Với Massage thân thể: Có khu vực nào chúng tôi nên tập trung không?","followUpLabel":"Với Massage mặt: Bạn có đang gặp phải bất kỳ vấn đề gì về da không? Nếu có, vui lòng miêu tả ngắn gọn:","options":[{"optionId":46,"optionText":"Có"},{"optionId":47,"optionText":"Không"}]},
              {"questionId":16,"questionText":"Với massage: Bạn có muốn tránh bất cứ khu vực nào không?","options":[]},
              {"questionId":17,"questionText":"Bạn đã bao giờ hay đang gặp phải các vấn đề về sức khỏe dưới đây?","options":[{"optionId":48,"optionText":"Tiểu Đường"},{"optionId":49,"optionText":"Động Kinh"},{"optionId":50,"optionText":"Hen Suyễn"},{"optionId":51,"optionText":"Ngất Xỉu"},{"optionId":52,"optionText":"Đau Cơ"},{"optionId":53,"optionText":"Vấn Đề Tiêu Hóa"},{"optionId":54,"optionText":"Huyết Áp Cao/ Thấp"},{"optionId":55,"optionText":"Bệnh Về Da"}]},
              {"questionId":18,"questionText":"Bạn có bị dị ứng hoặc nhạy cảm với các thứ dưới đây không?","options":[{"optionId":56,"optionText":"Thức Ăn"},{"optionId":57,"optionText":"Thuốc"},{"optionId":58,"optionText":"Tinh Dầu"}]},
              {"questionId":19,"questionText":"Gần đây bạn có phẫu thuật không?","followUpLabel":"Nếu có, vui lòng miêu tả ngắn gọn:","options":[{"optionId":59,"optionText":"Có"},{"optionId":60,"optionText":"Không"}]},
              {"questionId":20,"questionText":"Với Phụ Nữ: Bạn đang có thai không?","followUpLabel":"Nếu có, Bạn đang ở quý thai kỳ nào?","options":[{"optionId":61,"optionText":"Có"},{"optionId":62,"optionText":"Không"}]}
            ]
            """;

        private const string EnQuestionsTemplate2 = """
            [
             {"questionId":11,"questionText":"Have you had a Spa treatment experience before?","options":[{"optionId":32,"optionText":"Yes"},{"optionId":33,"optionText":"No"}]},
             {"questionId":12,"questionText":"How are you feeling right now?","options":[{"optionId":34,"optionText":"Tired"},{"optionId":35,"optionText":"Stressed"},{"optionId":36,"optionText":"Muscle Aches"},{"optionId":37,"optionText":"Normal"}]},
             {"questionId":13,"questionText":"How would you like to feel after the treatment?","options":[{"optionId":38,"optionText":"Peaceful"},{"optionId":39,"optionText":"Refreshed"},{"optionId":40,"optionText":"Invigorated"},{"optionId":41,"optionText":"Energized"}]},
             {"questionId":14,"questionText":"For Massage: How would you like the pressure to be?","options":[{"optionId":42,"optionText":"Strong"},{"optionId":43,"optionText":"Medium"},{"optionId":44,"optionText":"Light"},{"optionId":45,"optionText":"Need to Test"}]},
             {"questionId":15,"questionText":"For Body Massage: Are there any areas we should focus on?","followUpLabel":"For Facial Massage: Are you currently experiencing any skin concerns? If yes, please describe briefly:","options":[{"optionId":46,"optionText":"Yes"},{"optionId":47,"optionText":"No"}]},
             {"questionId":16,"questionText":"For massage: Are there any areas you would like to avoid?","options":[]},
             {"questionId":17,"questionText":"Have you ever had or are currently experiencing any of the following health issues?","options":[{"optionId":48,"optionText":"Diabetes"},{"optionId":49,"optionText":"Epilepsy"},{"optionId":50,"optionText":"Asthma"},{"optionId":51,"optionText":"Fainting"},{"optionId":52,"optionText":"Muscle Pain"},{"optionId":53,"optionText":"Digestive Issues"},{"optionId":54,"optionText":"High/Low Blood Pressure"},{"optionId":55,"optionText":"Skin Conditions"}]},
             {"questionId":18,"questionText":"Are you allergic or sensitive to any of the following?","options":[{"optionId":56,"optionText":"Food"},{"optionId":57,"optionText":"Medication"},{"optionId":58,"optionText":"Essential Oils"}]},
             {"questionId":19,"questionText":"Have you had any recent surgeries?","followUpLabel":"If yes, please describe briefly:","options":[{"optionId":59,"optionText":"Yes"},{"optionId":60,"optionText":"No"}]},
             {"questionId":20,"questionText":"For Women: Are you currently pregnant?","followUpLabel":"If yes, which trimester are you in?","options":[{"optionId":61,"optionText":"Yes"},{"optionId":62,"optionText":"No"}]}
            ]
            """;

        // ── Vietnamese QuestionsTranslation JSON for Template 3 (Lotus – Questions 21–30, Options 63–93) ──
        private const string ViQuestionsTemplate3 = """
            [
              {"questionId":21,"questionText":"Bạn đã từng có trải nghiệm trị liệu Spa trước đây chưa?","options":[{"optionId":63,"optionText":"Có"},{"optionId":64,"optionText":"Không"}]},
              {"questionId":22,"questionText":"Hiện tại bạn đang cảm thấy thế nào?","options":[{"optionId":65,"optionText":"Mệt Mỏi"},{"optionId":66,"optionText":"Căng Thẳng"},{"optionId":67,"optionText":"Nhức Cơ Bắp"},{"optionId":68,"optionText":"Bình Thường"}]},
              {"questionId":23,"questionText":"Bạn muốn cảm thấy thế nào sau khi trị liệu?","options":[{"optionId":69,"optionText":"Yên Bình"},{"optionId":70,"optionText":"Tươi Mới"},{"optionId":71,"optionText":"Phấn Khởi"},{"optionId":72,"optionText":"Đầy Năng Lượng"}]},
              {"questionId":24,"questionText":"Với Massage: Bạn mong muốn dùng lực thế nào?","options":[{"optionId":73,"optionText":"Mạnh"},{"optionId":74,"optionText":"Trung Bình"},{"optionId":75,"optionText":"Nhẹ"},{"optionId":76,"optionText":"Cần Thử Lực"}]},
              {"questionId":25,"questionText":"Với Massage thân thể: Có khu vực nào chúng tôi nên tập trung không?","followUpLabel":"Với Massage mặt: Bạn có đang gặp phải bất kỳ vấn đề gì về da không? Nếu có, vui lòng miêu tả ngắn gọn:","options":[{"optionId":77,"optionText":"Có"},{"optionId":78,"optionText":"Không"}]},
              {"questionId":26,"questionText":"Với massage: Bạn có muốn tránh bất cứ khu vực nào không?","options":[]},
              {"questionId":27,"questionText":"Bạn đã bao giờ hay đang gặp phải các vấn đề về sức khỏe dưới đây?","options":[{"optionId":79,"optionText":"Tiểu Đường"},{"optionId":80,"optionText":"Động Kinh"},{"optionId":81,"optionText":"Hen Suyễn"},{"optionId":82,"optionText":"Ngất Xỉu"},{"optionId":83,"optionText":"Đau Cơ"},{"optionId":84,"optionText":"Vấn Đề Tiêu Hóa"},{"optionId":85,"optionText":"Huyết Áp Cao/ Thấp"},{"optionId":86,"optionText":"Bệnh Về Da"}]},
              {"questionId":28,"questionText":"Bạn có bị dị ứng hoặc nhạy cảm với các thứ dưới đây không?","options":[{"optionId":87,"optionText":"Thức Ăn"},{"optionId":88,"optionText":"Thuốc"},{"optionId":89,"optionText":"Tinh Dầu"}]},
              {"questionId":29,"questionText":"Gần đây bạn có phẫu thuật không?","followUpLabel":"Nếu có, vui lòng miêu tả ngắn gọn:","options":[{"optionId":90,"optionText":"Có"},{"optionId":91,"optionText":"Không"}]},
              {"questionId":30,"questionText":"Với Phụ Nữ: Bạn đang có thai không?","followUpLabel":"Nếu có, Bạn đang ở quý thai kỳ nào?","options":[{"optionId":92,"optionText":"Có"},{"optionId":93,"optionText":"Không"}]}
            ]
            """;

        private const string EnQuestionsTemplate3 = """
              [
              {"questionId":21,"questionText":"Have you had a Spa treatment experience before?","options":[{"optionId":63,"optionText":"Yes"},{"optionId":64,"optionText":"No"}]},
              {"questionId":22,"questionText":"How are you feeling right now?","options":[{"optionId":65,"optionText":"Tired"},{"optionId":66,"optionText":"Stressed"},{"optionId":67,"optionText":"Muscle Aches"},{"optionId":68,"optionText":"Normal"}]},
              {"questionId":23,"questionText":"How would you like to feel after the treatment?","options":[{"optionId":69,"optionText":"Peaceful"},{"optionId":70,"optionText":"Refreshed"},{"optionId":71,"optionText":"Invigorated"},{"optionId":72,"optionText":"Energized"}]},
              {"questionId":24,"questionText":"For Massage: How would you like the pressure to be?","options":[{"optionId":73,"optionText":"Strong"},{"optionId":74,"optionText":"Medium"},{"optionId":75,"optionText":"Light"},{"optionId":76,"optionText":"Need to Test"}]},
              {"questionId":25,"questionText":"For Body Massage: Are there any areas we should focus on?","followUpLabel":"For Facial Massage: Are you currently experiencing any skin concerns? If yes, please describe briefly:","options":[{"optionId":77,"optionText":"Yes"},{"optionId":78,"optionText":"No"}]},
              {"questionId":26,"questionText":"For massage: Are there any areas you would like to avoid?","options":[]},
              {"questionId":27,"questionText":"Have you ever had or are currently experiencing any of the following health issues?","options":[{"optionId":79,"optionText":"Diabetes"},{"optionId":80,"optionText":"Epilepsy"},{"optionId":81,"optionText":"Asthma"},{"optionId":82,"optionText":"Fainting"},{"optionId":83,"optionText":"Muscle Pain"},{"optionId":84,"optionText":"Digestive Issues"},{"optionId":85,"optionText":"High/Low Blood Pressure"},{"optionId":86,"optionText":"Skin Conditions"}]},
              {"questionId":28,"questionText":"Are you allergic or sensitive to any of the following?","options":[{"optionId":87,"optionText":"Food"},{"optionId":88,"optionText":"Medication"},{"optionId":89,"optionText":"Essential Oils"}]},
              {"questionId":29,"questionText":"Have you had any recent surgeries?","followUpLabel":"If yes, please describe briefly:","options":[{"optionId":90,"optionText":"Yes"},{"optionId":91,"optionText":"No"}]},
              {"questionId":30,"questionText":"For Women: Are you currently pregnant?","followUpLabel":"If yes, which trimester are you in?","options":[{"optionId":92,"optionText":"Yes"},{"optionId":93,"optionText":"No"}]}
              ]
              """;

        public static void Seed(ModelBuilder modelBuilder)
        {
            var seedAt = new DateTime(2026, 05, 19, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<FormTemplateTranslation>().HasData(

                // ── Template 1: THE GRAND SPA CONSULTATION ──────────────────────────
                new FormTemplateTranslation
                {
                    Id = 1,
                    FormTemplateId = 1,
                    LanguageCode = "vi",
                    Title = "TƯ VẤN SPA THE GRAND",
                    Description = "Vui lòng dành một chút thời gian để điền đầy đủ vào mẫu này, vì câu trả lời của bạn sẽ giúp chúng tôi cung cấp dịch vụ điều trị an toàn và hiệu quả:",
                    FooterText = "Tôi xác nhận rằng các liệu trình tại The Grand Spa không mang tính y tế. Tôi xác nhận rằng tôi đã điền đầy đủ và chính xác vào mẫu tư vấn này và miễn trừ trách nhiệm cho The Grand Spa, khách sạn và nhân viên của khách sạn đối với bất kỳ trách nhiệm pháp lý hoặc khiếu nại nào.\r\n\r\nTôi hiểu rằng việc hủy hoặc đổi lịch phải được thực hiện ít nhất 24 giờ trước để tránh bị tính phí 100% chi phí của liệu trình đã chọn.",
                    AgreementText = "Tôi đồng ý với các điều khoản và điều kiện trên.",
                    QuestionsTranslation = ViQuestionsTemplate1,
                    UpdatedAt = seedAt,
                    UpdatedBy = CommonConstants.SystemUser,
                },
                new FormTemplateTranslation
                {
                    Id = 2,
                    FormTemplateId = 1,
                    LanguageCode = CommonConstants.DefaultLanguage,
                    Title = "THE GRAND SPA CONSULTATION",
                    Description = "Please take a moment to fill out this form completely, as your answers will help us provide safe and effective treatment services:",
                    FooterText = "I acknowledge that the treatments at The Grand Spa are not medical in nature. I confirm that I have completed this consultation form fully and accurately and release The Grand Spa, the hotel, and the hotel's staff from any legal liability or claims.\r\n\r\nI understand that cancellations or rescheduling must be made at least 24 hours in advance to avoid being charged 100% of the selected treatment cost.",
                    AgreementText = "I agree to the above terms and conditions.",
                    QuestionsTranslation = EnQuestionsTemplate1,
                    UpdatedAt = seedAt,
                    UpdatedBy = CommonConstants.SystemUser,
                },

                // ── Template 2: THE MAIA CONSULTATION ───────────────────────────────
                new FormTemplateTranslation
                {
                    Id = 3,
                    FormTemplateId = 2,
                    LanguageCode = "vi",
                    Title = "TƯ VẤN SPA THE MAIA",
                    Description = "Vui lòng dành một chút thời gian để điền đầy đủ vào mẫu này, vì câu trả lời của bạn sẽ giúp chúng tôi cung cấp dịch vụ điều trị an toàn và hiệu quả:",
                    FooterText = "Tôi xác nhận rằng các liệu trình tại The Maia Spa không mang tính y tế. Tôi xác nhận rằng tôi đã điền đầy đủ và chính xác vào mẫu tư vấn này và miễn trừ trách nhiệm cho The Maia Spa, khách sạn và nhân viên của khách sạn đối với bất kỳ trách nhiệm pháp lý hoặc khiếu nại nào.\r\n\r\nTôi hiểu rằng việc hủy hoặc đổi lịch phải được thực hiện ít nhất 24 giờ trước để tránh bị tính phí 100% chi phí của liệu trình đã chọn.",
                    QuestionsTranslation = ViQuestionsTemplate2,
                    AgreementText = "Tôi đồng ý với các điều khoản và điều kiện trên.",
                    UpdatedAt = seedAt,
                    UpdatedBy = CommonConstants.SystemUser,
                },
                new FormTemplateTranslation
                {
                    Id = 4,
                    FormTemplateId = 2,
                    LanguageCode = CommonConstants.DefaultLanguage,
                    Title = "THE MAIA CONSULTATION",
                    Description = "Please take a moment to fill out this form completely, as your answers will help us provide safe and effective treatment services:",
                    FooterText = "I acknowledge that the treatments at The Maia Spa are not medical in nature. I confirm that I have completed this consultation form fully and accurately and release The Maia Spa, the hotel, and the hotel's staff from any legal liability or claims.\r\n\r\nI understand that cancellations or rescheduling must be made at least 24 hours in advance to avoid being charged 100% of the selected treatment cost.",
                    QuestionsTranslation = EnQuestionsTemplate2,
                    AgreementText = "I agree to the above terms and conditions.",
                    UpdatedAt = seedAt,
                    UpdatedBy = CommonConstants.SystemUser,
                },

                // ── Template 3: THE LOTUS SPA CONSULTATION ──────────────────────────
                new FormTemplateTranslation
                {
                    Id = 5,
                    FormTemplateId = 3,
                    LanguageCode = "vi",
                    Title = "TƯ VẤN SPA THE LOTUS",
                    Description = "Vui lòng dành một chút thời gian để điền đầy đủ vào mẫu này, vì câu trả lời của bạn sẽ giúp chúng tôi cung cấp dịch vụ điều trị an toàn và hiệu quả:",
                    FooterText = "Tôi xác nhận rằng các liệu trình tại The Lotus Spa không mang tính y tế. Tôi xác nhận rằng tôi đã điền đầy đủ và chính xác vào mẫu tư vấn này và miễn trừ trách nhiệm cho The Lotus Spa, khách sạn và nhân viên của khách sạn đối với bất kỳ trách nhiệm pháp lý hoặc khiếu nại nào.\r\n\r\nTôi hiểu rằng việc hủy hoặc đổi lịch phải được thực hiện ít nhất 24 giờ trước để tránh bị tính phí 100% chi phí của liệu trình đã chọn.",
                    QuestionsTranslation = ViQuestionsTemplate3,
                    AgreementText = "Tôi đồng ý với các điều khoản và điều kiện trên.",
                    UpdatedAt = seedAt,
                    UpdatedBy = CommonConstants.SystemUser,
                },

                new FormTemplateTranslation
                {
                    Id = 6,
                    FormTemplateId = 3,
                    LanguageCode = CommonConstants.DefaultLanguage,
                    Title = "THE LOTUS SPA CONSULTATION",
                    Description = "Please take a moment to fill out this form completely, as your answers will help us provide safe and effective treatment services:",
                    FooterText = "I acknowledge that the treatments at The Lotus Spa are not medical in nature. I confirm that I have completed this consultation form fully and accurately and release The Lotus Spa, the hotel, and the hotel's staff from any legal liability or claims.\r\n\r\nI understand that cancellations or rescheduling must be made at least 24 hours in advance to avoid being charged 100% of the selected treatment cost.",
                    QuestionsTranslation = EnQuestionsTemplate3,
                    AgreementText = "I agree to the above terms and conditions.",
                    UpdatedAt = seedAt,
                    UpdatedBy = CommonConstants.SystemUser,
                }
            );
        }
    }
}