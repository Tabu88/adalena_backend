
namespace AdalenaEmailApp.Services.EmailService
{
    public interface IEmailService
    {
        void SendEmail(TourDTO request, string htmltemplatePath);
    }
}
