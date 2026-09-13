using Microsoft.JSInterop;

namespace Presentacion.Servicios
{
    public class ToastrService : IToastrService
    {
        private readonly IJSRuntime _jsRuntime;

        public ToastrService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task MsgExito(string message)
        {
            await _jsRuntime.InvokeVoidAsync("toastr.success", message);
        }

        public async Task MsgError(string message)
        {
            await _jsRuntime.InvokeVoidAsync("toastr.error", message);
        }

        public async Task MsgAdvertencia(string message)
        {
            await _jsRuntime.InvokeVoidAsync("toastr.warning", message);
        }

        public async Task MsgInformacion(string message)
        {
            await _jsRuntime.InvokeVoidAsync("toastr.info", message);
        }
    }
}