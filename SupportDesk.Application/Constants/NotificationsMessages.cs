namespace SupportDesk.Application.Constants
{
    public static class NotificationsMessages
    {
        public const string FailedToSendNotificationRequestCreated = "Error al enviar notificación de nueva solicitud creada.";
        public const string FailedToSendNotificationRequestUpdate = "Error al enviar notificación de solicitud actualizada.";
        public const string FailedToSendNotificationRequestUnderReview = "Error al enviar notificación de solicitud en revision.";
        public const string FailedToSendNotificationRequestProcessed = "Error al enviar notificación de procesamiento de solicitud.";
        public const string FailedToSendNotificationRequestInactivated = "Error al enviar notificación de procesamiento de inactivada.";
        public const string NoSupervisorAssignedToUpdatedRequest = "No hay un supervisor asignado a la solicitud. La notificación no se enviará.";
    }
}
