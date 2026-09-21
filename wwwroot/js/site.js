// مكان جاهز لأي تفاعل إضافي في الموقع (فلاتر المشاريع، السلايدر... إلخ)
document.addEventListener('DOMContentLoaded', function () {
    console.log('الموقع جاهز.');
});
document.addEventListener('DOMContentLoaded', function () {
    const slides = document.querySelectorAll('.hero-slide');
    if (slides.length > 1) {
        let current = 0;
        setInterval(function () {
            slides[current].classList.remove('active');
            current = (current + 1) % slides.length;
            slides[current].classList.add('active');
        }, 2000);
    }
});

function zoomImage(event, src) {
    event.preventDefault();
    event.stopPropagation();
    document.getElementById('imgLightboxContent').src = src;
    document.getElementById('imgLightbox').classList.add('show');
}
function closeZoom() {
    document.getElementById('imgLightbox').classList.remove('show');
}