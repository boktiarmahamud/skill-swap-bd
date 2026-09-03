document.addEventListener("DOMContentLoaded", function () {
    // Render numeric ratings as star icons
    document.querySelectorAll("[data-rating]").forEach(function (el) {
        var rating = Math.round(parseFloat(el.getAttribute("data-rating")) || 0);
        var stars = "★".repeat(rating) + "☆".repeat(5 - rating);
        el.textContent = stars;
    });

    // Simple fade-in for skill cards
    var cards = document.querySelectorAll(".skill-card, .review-card");
    cards.forEach(function (card, i) {
        card.style.opacity = "0";
        card.style.transform = "translateY(8px)";
        setTimeout(function () {
            card.style.transition = "opacity 0.35s ease, transform 0.35s ease";
            card.style.opacity = "1";
            card.style.transform = "translateY(0)";
        }, i * 60);
    });
});