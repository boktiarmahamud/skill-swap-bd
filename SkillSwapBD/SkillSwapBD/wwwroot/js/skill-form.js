document.addEventListener("DOMContentLoaded", function () {
    var fileInput = document.querySelector('input[type="file"][name="Attachment"]');
    var hint = document.querySelector(".file-drop-hint");

    if (!fileInput || !hint) return;

    var defaultText = hint.textContent;

    fileInput.addEventListener("change", function () {
        if (fileInput.files && fileInput.files.length > 0) {
            var file = fileInput.files[0];
            var sizeMB = (file.size / (1024 * 1024)).toFixed(2);
            hint.textContent = "📎 Selected: " + file.name + " (" + sizeMB + " MB)";

            if (file.size > 5 * 1024 * 1024) {
                hint.style.color = "#DC2626";
                hint.textContent += " — exceeds 5MB limit!";
            } else {
                hint.style.color = "#64748B";
            }
        } else {
            hint.textContent = defaultText;
            hint.style.color = "#64748B";
        }
    });
});