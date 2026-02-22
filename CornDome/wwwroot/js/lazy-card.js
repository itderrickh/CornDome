document.addEventListener("DOMContentLoaded", function () {
    const images = document.querySelectorAll(".lazy-image");

    const observer = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (!entry.isIntersecting) return;

            const img = entry.target;
            const fullSrc = img.dataset.src;

            // Create new image to preload
            const tempImg = new Image();
            tempImg.src = fullSrc;

            tempImg.onload = () => {
                img.src = fullSrc;
                img.classList.add("loaded");
            };

            observer.unobserve(img);
        });
    }, {
        rootMargin: "100px"
    });

    images.forEach(img => observer.observe(img));
});