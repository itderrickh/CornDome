(() => {
    const fileInput = document.getElementById("fileInput");
    const preview = document.getElementById("imagePreview");
    const placeholder = document.getElementById("previewPlaceholder");
    const previewContainer = document.getElementById("newPreviewContainer");

    let currentUrl = null;

    fileInput.addEventListener("change", event => {
        const file = event.target.files?.[0];

        if (currentUrl) {
            URL.revokeObjectURL(currentUrl);
            currentUrl = null;
        }

        if (!file) {
            preview.src = "";
            preview.classList.add("d-none");
            placeholder.classList.remove("d-none");
            previewContainer.classList.remove("has-image");
            return;
        }

        if (!file.type.startsWith("image/")) {
            preview.src = "";
            preview.classList.add("d-none");
            placeholder.classList.remove("d-none");
            previewContainer.classList.remove("has-image");
            return;
        }

        currentUrl = URL.createObjectURL(file);

        preview.onload = () => {
            preview.classList.remove("d-none");
            placeholder.classList.add("d-none");
            previewContainer.classList.add("has-image");
        };

        preview.onerror = () => {
            preview.classList.add("d-none");
            placeholder.classList.remove("d-none");
            previewContainer.classList.remove("has-image");
        };

        preview.src = currentUrl;
    });

    window.addEventListener("beforeunload", () => {
        if (currentUrl) {
            URL.revokeObjectURL(currentUrl);
        }
    });
})();