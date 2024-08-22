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
        public const string InvalidUser = "Usuario inválido.";
        public const string InvalidRequestType = "Tipo de solicitud inválida.";
        public const string InvalidZone = "Zona inválida.";
        public const string RequiredComments = "Los comentarios son requeridos.";
        public const string CommentsMinLenght = "Los comentarios deben tener al menos 15 caracteres.";
        public const string CommentsMaxLenght = "Los comentarios no deben tener mas de 800 caracteres.";
        public const string RequestNotFoundOrIsInactive = "Los comentarios no deben tener mas de 800 caracteres.";
        public const string RequestHasBeenInactive = "La solicitud ha sido inactivada exitosamente.";
        public const string InvalidRequestId = "Solicitud ID Inválida.";
        
        public const string RequestAlreadyProccessed = "No se puede actualizar una solicitud que ya ha sido procesada.";
        public const string CannotUpdateRequestTypeOrZone = "No se puede actualizar una solicitud que ya ha sido procesada.";
        public const string RequestHasBeenUpdated = "No se puede actualizar una solicitud que ya ha sido procesada.";


    }
}
