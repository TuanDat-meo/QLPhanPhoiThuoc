// ==================== MEDICINE MANAGEMENT SYSTEM ====================

/**
 * Load nội dung thuốc vào content area
 * @param {string} url - URL để load nội dung
 */
function loadThuocContent(url) {
    // Nếu là Details hoặc Edit, mở trong panel
    if (url.includes('/Details/')) {
        const maThuoc = url.split('/Details/')[1];
        loadThuocDetailsPanel(maThuoc);
    } else if (url.includes('/Edit/')) {
        const maThuoc = url.split('/Edit/')[1];
        loadThuocEditPanel(maThuoc);
    } else if (url.includes('/PhieuNhap/')) {
        loadPhieuNhapPanel();
    } else {
        // Các trang khác load bình thường
        showLoading();
        showSection('medicines');

        $.ajax({
            url: url,
            method: 'GET',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            },
            success: function (data) {
                $('#medicineContent').html(data);
                hideLoading();
            },
            error: function (xhr, status, error) {
                console.error('❌ Error loading medicine content:', error);
                $('#medicineContent').html(
                    '<div class="alert alert-danger" style="margin: 20px;">' +
                    '<h4><i class="fas fa-exclamation-triangle"></i> Lỗi tải dữ liệu thuốc</h4>' +
                    '<p><strong>Chi tiết lỗi:</strong> ' + error + '</p>' +
                    '<button class="btn btn-primary" onclick="loadThuocContent(\'' + url + '\')">Thử lại</button>' +
                    '</div>'
                );
                hideLoading();
            }
        });
    }
}

/**
 * Mở panel chi tiết thuốc
 * @param {string} maThuoc - Mã thuốc cần xem chi tiết
 */
function loadThuocDetailsPanel(maThuoc) {
    openPanel(
        `/Admin/Thuoc/Details/${maThuoc}`,
        'Chi tiết thuốc',
        'fas fa-pills'
    );
}

/**
 * Mở panel chỉnh sửa thuốc
 * @param {string} maThuoc - Mã thuốc cần chỉnh sửa
 */
function loadThuocEditPanel(maThuoc) {
    openPanel(
        `/Admin/Thuoc/Edit/${maThuoc}`,
        'Chỉnh sửa thuốc',
        'fas fa-edit'
    );
}

/**
 * Mở panel tạo phiếu nhập
 */
function loadPhieuNhapPanel() {
    openPanel(
        '/Admin/PhieuNhap/Create',
        'Tạo phiếu nhập',
        'fas fa-file-import'
    );
}

/**
 * Mở panel thêm thuốc mới
 */
function loadThuocCreatePanel() {
    openPanel(
        '/Admin/Thuoc/Create',
        'Thêm thuốc mới',
        'fas fa-plus-circle'
    );
}

/**
 * Xóa thuốc với xác nhận
 * @param {string} maThuoc - Mã thuốc cần xóa
 */
function deleteThuoc(maThuoc) {
    if (confirm('Bạn có chắc chắn muốn xóa thuốc này?')) {
        showLoading();

        $.ajax({
            url: `/Admin/Thuoc/Delete/${maThuoc}`,
            method: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            },
            success: function (response) {
                hideLoading();

                if (response.success) {
                    alert('Xóa thuốc thành công!');
                    // Reload danh sách thuốc
                    loadThuocContent('/Admin/Thuoc/Index');
                } else {
                    alert('Lỗi: ' + (response.message || 'Không thể xóa thuốc'));
                }
            },
            error: function (xhr, status, error) {
                hideLoading();
                console.error('❌ Error deleting medicine:', error);
                alert('Lỗi khi xóa thuốc: ' + error);
            }
        });
    }
}

/**
 * Refresh danh sách thuốc
 */
function refreshThuocList() {
    loadThuocContent('/Admin/Thuoc/Index');
}

/**
 * Tìm kiếm thuốc
 * @param {string} keyword - Từ khóa tìm kiếm
 */
function searchThuoc(keyword) {
    showLoading();

    $.ajax({
        url: '/Admin/Thuoc/Search',
        method: 'GET',
        data: { keyword: keyword },
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        },
        success: function (data) {
            $('#medicineContent').html(data);
            hideLoading();
        },
        error: function (xhr, status, error) {
            console.error('❌ Error searching medicine:', error);
            hideLoading();
            alert('Lỗi khi tìm kiếm thuốc: ' + error);
        }
    });
}