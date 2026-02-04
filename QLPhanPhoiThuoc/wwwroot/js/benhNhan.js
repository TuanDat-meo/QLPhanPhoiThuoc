function loadContent(url) {
    // Hiển thị loading
    $('#patients').html('<div style="text-align:center; padding:50px;"><i class="fas fa-spinner fa-spin fa-3x"></i><p>Đang tải...</p></div>');

    // Show section patients
    $('.content-section').removeClass('active');
    $('#patients').addClass('active');

    // Update menu active state
    $('.menu-group-header').removeClass('active');
    $('[data-section="patients"]').addClass('active');

    // Gọi AJAX để load nội dung
    fetch(url)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.text();
        })
        .then(html => {
            // Load nội dung vào section patients
            $('#patients').html(html);
        })
        .catch(error => {
            console.error('Lỗi:', error);
            $('#patients').html('<div class="alert alert-danger">Có lỗi xảy ra khi tải nội dung!</div>');
        });
}