using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Contracts.Services;
using SupportDesk.Application.Models.Notifications;
using System.Security.Cryptography;

namespace SupportDesk.Application.Services;

public class TwoFactorService : ITwoFactorService
{
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;

    public TwoFactorService(IUserRepository userRepository, INotificationService notificationService)
    {
        _userRepository = userRepository;
        _notificationService = notificationService;
    }

    public async Task<string> GenerateAndSendTwoFactorCodeAsync(Guid userId, string email, CancellationToken cancellationToken = default)
    {
        // Generar un código aleatorio de 6 dígitos
        var twoFactorCode = GenerateTwoFactorCode();

        // Guardar el código en el repositorio del usuario
        await _userRepository.SaveTwoFactorCodeAsync(userId, twoFactorCode, cancellationToken);

        // Crear el mensaje de notificación
        var notificationMessage = new NotificationMessage
        {
            RecipientUserIds = new List<Guid> { userId },
            Subject = "Tu codigo de autenticacion de doble factor",
            Body = $"Tu codigo 2FA es: {twoFactorCode}. Va a expirar en 10 minutos."
        };

        // Enviar el código de 2FA al usuario
        await _notificationService.SendNotificationAsync(notificationMessage, cancellationToken);

        return twoFactorCode;
    }

    public async Task<bool> ValidateTwoFactorCodeAsync(Guid userId, string code, CancellationToken cancellationToken = default)
    {
        // Validar el código utilizando el repositorio del usuario
        return await _userRepository.ValidateTwoFactorCodeAsync(userId, code, cancellationToken);
    }

    private static string GenerateTwoFactorCode()
    {
        // Generar un código numérico de 6 dígitos
        using (var rng = new RNGCryptoServiceProvider())
        {
            var buffer = new byte[4];
            rng.GetBytes(buffer);
            var random = BitConverter.ToUInt32(buffer, 0) % 1000000;
            return random.ToString("D6");
        }
    }
}
