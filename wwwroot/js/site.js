// File: wwwroot/js/site.js

document.addEventListener('DOMContentLoaded', function () {
    // --- 3D Hover Effect for Product Cards ---
    const cards = document.querySelectorAll('.product-card');
    cards.forEach(card => {
        card.addEventListener('mousemove', e => {
            const rect = card.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;
            const centerX = rect.width / 2;
            const centerY = rect.height / 2;
            const rotateX = (y - centerY) / centerY * 4;
            const rotateY = (x - centerX) / centerX * -4;
            card.style.transform = `perspective(1000px) rotateX(${rotateX}deg) rotateY(${rotateY}deg) scale(1.02)`;
            card.style.boxShadow = `0 6px 20px rgba(0, 0, 0, 0.1)`;
        });
        card.addEventListener('mouseleave', () => {
            card.style.transform = `perspective(1000px) rotateX(0deg) rotateY(0deg) scale(1)`;
            card.style.boxShadow = `0 2px 8px rgba(0, 0, 0, 0.08)`;
        });
    });

    // --- Custom Loading Animation (Integrated) ---
    const productLinks = document.querySelectorAll('.product-link');
    const loadingAnimation = document.querySelector('.loading');
    if (loadingAnimation) {
        productLinks.forEach(link => {
            link.addEventListener('click', function (event) {
                event.preventDefault(); 
                const destinationUrl = this.href;
                loadingAnimation.style.display = 'block';
                setTimeout(function () {
                    window.location.href = destinationUrl;
                }, 2000); // 2-second loading time
            });
        });
    }

    // --- Dark Mode Theme Switcher ---
    const themeSwitch = document.getElementById('themeSwitch');
    if (themeSwitch) {
        function setTheme(isDark) {
            if (isDark) {
                document.body.classList.add('dark-mode');
                localStorage.setItem('theme', 'dark');
            } else {
                document.body.classList.remove('dark-mode');
                localStorage.setItem('theme', 'light');
            }
        }

        const savedTheme = localStorage.getItem('theme');
        if (savedTheme === 'dark') {
            themeSwitch.checked = true;
            setTheme(true);
        } else {
            themeSwitch.checked = false;
            setTheme(false);
        }

        themeSwitch.addEventListener('change', function () {
            setTheme(this.checked);
        });
    }
});