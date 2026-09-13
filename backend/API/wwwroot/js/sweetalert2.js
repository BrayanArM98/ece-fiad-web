window.SweetAlert = {
    showAlert: (title, message, icon) => {
        Swal.fire(title, message, icon);
    },
    showConfirm: (title, message, icon, confirmButtonText) => {
        return Swal.fire({
            title: title,
            text: message,
            icon: icon,
            showCancelButton: true,
            confirmButtonText: confirmButtonText
        }).then((result) => {
            return result.isConfirmed;
        });
    }
};