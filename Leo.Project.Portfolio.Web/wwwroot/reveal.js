// scrollReveal.js
window.scrollReveal = {
    observer: null,
    init: function () {
        // Initialize observer only once if possible
        if (!this.observer) {
            const options = {
                root: null,
                rootMargin: '0px',
                threshold: 0.15
            };

            this.observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        entry.target.classList.add('revealed');
                        this.observer.unobserve(entry.target);
                    }
                });
            }, options);
        }

        // Periodically check for new elements or on call
        // Specifically for Blazor's dynamic page rendering
        const elements = document.querySelectorAll('.reveal:not(.revealed)');
        elements.forEach(el => this.observer.observe(el));
    }
};

// Auto-run once to catch early elements
document.addEventListener('DOMContentLoaded', () => window.scrollReveal.init());
