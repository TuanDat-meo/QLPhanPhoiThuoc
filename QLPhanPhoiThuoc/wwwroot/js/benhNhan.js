// File: wwwroot/js/benhNhan.js

$(document).ready(function () {
    // 1. XỬ LÝ TÌM KIẾM
    // Dùng $(document).on để bắt sự kiện ngay cả khi form mới được load lại
    $(document).on('submit', '#searchForm', function (e) {
        e.preventDefault();
        const searchString = $('#patientSearchInput').val();
        // Gọi hàm load lại tab danh sách với tham số tìm kiếm
        reloadPatientGrid(1, searchString);
    });

    // 2. XỬ LÝ PHÂN TRANG
    $(document).on('click', '#patient-list .pagination a', function (e) {
        e.preventDefault();
        if ($(this).parent().hasClass('disabled')) return;

        const page = $(this).data('page');
        const searchString = $('#patientSearchInput').val(); // Lấy từ input hiện tại

        reloadPatientGrid(page, searchString);
    });

    // 3. XỬ LÝ NÚT XÓA (Delete)
    $(document).on('click', '.btn-delete', function () {
        const row = $(this).closest('tr');
        const maBenhNhan = row.find('td:first').text().trim();
        const tenBenhNhan = row.find('.fw-bold').first().text().trim();

        Swal.fire({
            title: 'Xác nhận xóa?',
            html: `Bạn có chắc chắn muốn xóa bệnh nhân:<br><strong>${tenBenhNhan}</strong> (${maBenhNhan})?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Xóa',
            cancelButtonText: 'Hủy'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: '/Admin/BenhNhan/XoaBenhNhan/' + maBenhNhan,
                    type: 'POST',
                    success: function (response) {
                        if (response.success) {
                            Swal.fire({
                                icon: 'success',
                                title: 'Đã xóa!',
                                text: 'Xóa bệnh nhân thành công',
                                timer: 2000,
                                showConfirmButton: false
                            });
                            // Reload lại trang hiện tại
                            const currentPage = $('.pagination .page-item.active a').data('page') || 1;
                            reloadPatientGrid(currentPage);
                        } else {
                            Swal.fire({ icon: 'error', title: 'Lỗi!', text: response.message });
                        }
                    },
                    error: function () {
                        Swal.fire({ icon: 'error', title: 'Lỗi!', text: 'Không thể kết nối đến máy chủ' });
                    }
                });
            }
        });
    });
});

// ================= CÁC HÀM GLOBAL (Được gọi từ onclick HTML) =================

// Hàm load lại dữ liệu vào div #patient-list
window.reloadPatientGrid = function (page = 1, searchString = '') {
    const url = `/Admin/BenhNhan/_DSBenhNhan?page=${page}&searchString=${encodeURIComponent(searchString || '')}`;

    // Hiển thị loading nhẹ hoặc làm mờ bảng cũ
    $('#patient-list').css('opacity', '0.5');

    $.get(url, function (data) {
        $('#patient-list').html(data).css('opacity', '1');
    }).fail(function () {
        Swal.fire('Lỗi', 'Không thể tải dữ liệu danh sách', 'error');
        $('#patient-list').css('opacity', '1');
    });
}

// Wrapper cho nút Reload trên giao diện
window.reloadPatientList = function () {
    reloadPatientGrid(1, '');
}

// Xem chi tiết
window.viewPatientDetail = function (id) {
    $.get('/Admin/BenhNhan/ChiTiet/' + id, function (response) {
        $('#modalContent').html(response);
        $('#patientModal').modal('show');
    }).fail(function () {
        Swal.fire({ icon: 'error', title: 'Lỗi!', text: 'Không thể tải thông tin bệnh nhân' });
    });
}

// Sửa bệnh nhân
window.editPatient = function (id) {
    $.get('/Admin/BenhNhan/LayFormSua/' + id, function (response) {
        $('#modalContent').html(response);
        $('#patientModal').modal('show');
    }).fail(function () {
        Swal.fire({ icon: 'error', title: 'Lỗi!', text: 'Không thể tải form chỉnh sửa' });
    });
}

// Xuất Excel (Client-side trigger)
window.exportPatientExcel = function () {
    const searchString = $('#patientSearchInput').val() || "";
    window.location.href = '/Admin/BenhNhan/ExportExcel?searchString=' + encodeURIComponent(searchString);
}

// Hàm ánh xạ Tab ID sang URL Controller
function loadTabContent(tabId) {
    let url = '';

    // Cấu hình URL cho từng Tab
    switch (tabId) {
        case 'patient-list':
            url = '/Admin/BenhNhan/_DSBenhNhan';
            break;
        case 'patient-add':
            url = '/Admin/BenhNhan/_ThemBenhNhan'; // Kiểm tra lại tên Action của bạn
            break;
        case 'patient-medical':
            url = '/Admin/BenhNhan/_BenhAnBenhNhan'; // Kiểm tra lại tên Action của bạn
            break;
    }

    if (url) {
        const $target = $('#' + tabId);
        // Hiển thị loading trước
        $target.html('<div class="text-center py-5"><div class="spinner-border text-primary"></div><p class="mt-2 text-muted">Đang tải dữ liệu...</p></div>');

        // Gọi AJAX
        $.get(url, function (data) {
            $target.html(data);
        }).fail(function () {
            $target.html('<div class="alert alert-danger m-3">Lỗi tải dữ liệu. Vui lòng thử lại.</div>');
        });
    }
}

// Hàm loadContent cũ (Giữ lại để tương thích nếu dùng ở chỗ khác)
function loadContent(url) {
    // Hàm này hiện tại ít dùng vì ta đã chuyển sang loadTabContent
    // Nhưng có thể dùng cho Phân trang / Tìm kiếm
    const activeTab = $('.tab-content.active');
    if (activeTab.length > 0) {
        activeTab.html('<div class="text-center py-5"><div class="spinner-border text-primary"></div></div>');
        $.get(url, function (data) {
            activeTab.html(data);
        });
    }
}