namespace SupportDesk.Application.Constants
{
    public static class RequestMessages
    {
        public const string RequestNotFound = "Solicitud no encontrada.";
        public const string InactiveRequestCannotBeProcessed = "Las solicitudes inactivas no pueden ser procesadas.";
        public const string RequestAlreadyApprovedOrRejected = "La solicitud ya fue procesada.";
        public const string RequestAlreadyBeingReviewed = "La solicitud ya está siendo revisada por otro usuario.";
        public const string UserNotFound = "Usuario no encontrado.";
        public const string UserNoZonePermission = "Usuario no tiene permisos para procesas solicitudes de esta zona.";
        public const string UserNoRequestTypePermission = "Usuario no tiene permisos para procesar este tipo de solicitudes.";
    }
}
