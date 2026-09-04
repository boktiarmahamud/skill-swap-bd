document.addEventListener("DOMContentLoaded", function () {
    // Little pop animation when Like is clicked
    var likeForm = document.querySelector('form[asp-action="ToggleLike"], .btn-like')?.closest("form");
    var likeBtn = document.querySelector(".btn-like");

    if (likeBtn) {
        likeBtn.addEventListener("click", function () {
            likeBtn.style.transform = "scale(0.92)";
            setTimeout(function () {
                likeBtn.style.transform = "scale(1)";
            }, 120);
        });
    }

    // Auto-focus reply input when a reply form appears in view
    document.querySelectorAll(".reply-form input[type='text']").forEach(function (input) {
        input.addEventListener("focus", function () {
            input.closest(".reply-form").style.boxShadow = "0 0 0 3px rgba(79, 70, 229, 0.08)";
        });
        input.addEventListener("blur", function () {
            input.closest(".reply-form").style.boxShadow = "none";
        });
    });
});