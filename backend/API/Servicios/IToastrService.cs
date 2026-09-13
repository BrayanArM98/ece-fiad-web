namespace Presentacion.Servicios
{
    public interface IToastrService
    {
        Task MsgExito(string message);
        Task MsgError(string message);
        Task MsgAdvertencia(string message);
        Task MsgInformacion(string message);
    }
}