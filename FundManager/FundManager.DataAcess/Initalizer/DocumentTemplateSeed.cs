using DigitalDocumentPlatform.Common.Constants;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigitalDocumentPlatform.DataAccess.Initalizer
{
    /// <summary>
    /// Seeds the default DocumentTemplate records:
    /// PDP (Personal Data Processing), HTP (Hotel Terms & Policies), and Spa Acknowledgement.
    /// </summary>
    public static class DocumentTemplateSeed
    {
        public static void Seed(EntityTypeBuilder<DocumentTemplate> builder)
        {
            var seedAt = new DateTime(2026, 05, 19, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                // ── PDP – Personal Data Processing Consent ────────────────────
                new DocumentTemplate
                {
                    Id = 1,
                    Title = "Personal Data Processing Consent (PDP)",
                    DocumentType = DocumentType.PDP,
                    Description = "Consent form for collecting and processing personal data in accordance with applicable data protection regulations.",
                    Content = """
<h2>Personal Data Processing Consent</h2>
<p>I, the undersigned, hereby consent to The Grand Ho Tram Strip collecting, storing, and processing my personal data for the purpose of delivering hotel and spa services, loyalty programme management, and regulatory compliance.</p>
<h3>Data collected</h3>
<ul>
  <li>Full name, date of birth, nationality</li>
  <li>Contact information (email, phone number)</li>
  <li>Health information relevant to spa treatments</li>
  <li>Transaction and service history</li>
</ul>
<h3>Your rights</h3>
<p>You have the right to access, correct, or request deletion of your personal data at any time by contacting our Data Protection Officer at <a href="mailto:dpo@thegrandhotram.com">dpo@thegrandhotram.com</a>.</p>
<p>By signing below you confirm that you have read and understood this consent form.</p>
""",
                    Version = 1,
                    OutletId = null,
                    IsActive = true,
                    IsDelete = false,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },

                // ── HTP – Hotel Terms & Policies ─────────────────────────────
                new DocumentTemplate
                {
                    Id = 2,
                    Title = "Hotel Terms & Policies (HTP)",
                    DocumentType = DocumentType.HTP,
                    Description = "Standard hotel terms and policies that guests must acknowledge upon check-in.",
                    Content = """
<h2>Hotel Terms &amp; Policies</h2>
<h3>Check-in / Check-out</h3>
<p>Standard check-in time is 15:00 and check-out time is 12:00. Early check-in and late check-out are subject to availability and may incur additional charges.</p>
<h3>Cancellation Policy</h3>
<p>Reservations cancelled within 48 hours of arrival will be charged one night's room rate. No-shows will be charged the full reservation amount.</p>
<h3>Property Rules</h3>
<p>Smoking is prohibited in all indoor areas. Pets are not permitted on the property. Guests are responsible for any damage caused to hotel property during their stay.</p>
<h3>Liability</h3>
<p>The hotel is not responsible for the loss or damage of personal belongings. Guests are encouraged to use the in-room safe or the hotel's safety deposit box service.</p>
<p>I confirm that I have read and agree to the above terms and policies.</p>
""",
                    Version = 1,
                    OutletId = null,
                    IsActive = true,
                    IsDelete = false,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                },

                // ── Spa Acknowledgement ────────────────────────────────────────
                new DocumentTemplate
                {
                    Id = 3,
                    Title = "Spa Liability Release & Acknowledgement",
                    DocumentType = DocumentType.SpaAcknowledgement,
                    Description = "Liability release form for spa treatments. Patron acknowledges the non-medical nature of treatments and cancellation policy.",
                    Content = """
<h2>Spa Liability Release &amp; Acknowledgement</h2>
<p>I acknowledge that treatments at The Grand Spa are non-medical. I confirm that I have accurately completed the spa consultation form and hereby release The Grand Spa, the hotel, and its employees from any liability or claims arising from my spa treatment.</p>
<h3>Cancellation Policy</h3>
<p>I understand that cancellations or rescheduling must be made at least 24 hours in advance to avoid being charged 100% of the cost of the selected treatment.</p>
<h3>Health Declaration</h3>
<p>I declare that the health information provided in the spa consultation form is accurate and complete to the best of my knowledge. I will inform the therapist of any changes to my health status before each treatment.</p>
<p>By signing below I confirm my understanding and acceptance of the above terms.</p>
""",
                    Version = 1,
                    OutletId = null,
                    IsActive = true,
                    IsDelete = false,
                    CreatedAt = seedAt,
                    UpdatedAt = seedAt,
                    CreatedBy = CommonConstants.SystemUser,
                    UpdatedBy = CommonConstants.SystemUser
                }
            );
        }
    }
}
