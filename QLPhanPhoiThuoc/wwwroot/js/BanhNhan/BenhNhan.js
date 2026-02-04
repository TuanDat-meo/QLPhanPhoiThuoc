function loadPatientContent(url) {
    showLoading();
    showSection('patients');
    closeAllPanels(); // Đóng tất cả panels

    $.ajax({
        url: url,
        method: 'GET',
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        },
        success: function (data) {
            $('#patientContent').html(data);
            hideLoading();

            // Re-initialize events if needed
            initializePatientEvents();
        },
        error: function (xhr, status, error) {
            console.error('Error loading patient content:', error);
            $('#patientContent').html('<div class="alert alert-danger">Lỗi khi tải nội dung: ' + error + '</div>');
            hideLoading();
        }
    });
}

// Initialize patient-specific events
function initializePatientEvents() {
    console.log('Patient events initialized');

    // Xử lý pagination nếu có
    $(document).on('click', '.pagination a', function (e) {
        e.preventDefault();
        var url = $(this).attr('href');
        if (url) {
            loadPatientContent(url);
        }
    });

    // Xử lý search form
    $(document).on('submit', '#searchForm', function (e) {
        e.preventDefault();
        var formData = $(this).serialize();
        loadPatientContent('/BenhNhan/_DSBenhNhan?' + formData);
    });
}

// ==================== PANEL FUNCTIONS FOR BỆNH NHÂN ====================

// Xem chi tiết bệnh nhân trong panel
function viewPatientDetails(maBenhNhan) {
    openPanel(
        `/BenhNhan/ChiTiet/${maBenhNhan}`,
        'Chi tiết bệnh nhân',
        'fas fa-user-injured'
    );
}

// Sửa bệnh nhân trong panel
function editPatient(maBenhNhan) {
    openPanel(
        `/BenhNhan/LayFormSua/${maBenhNhan}`,
        'Chỉnh sửa bệnh nhân',
        'fas fa-user-edit'
    );
}

// Lưu thông tin bệnh nhân sau khi sửa
function savePatient(formId) {
    const form = $('#' + formId);
    const formData = form.serializeArray();

    // Convert form data to object
    const data = {};
    $.each(formData, function (i, field) {
        data[field.name] = field.value;
    });

    $.ajax({
        url: '/BenhNhan/SuaBenhNhan',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            if (response.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Thành công!',
                    text: response.message,
                    timer: 2000,
                    showConfirmButton: false
                });

                // Đóng panel và reload danh sách
                closeAllPanels();
                loadPatientContent('/BenhNhan/_DSBenhNhan');
            } else {
                Swal.fire('Lỗi!', response.message, 'error');
            }
        },
        error: function (xhr) {
            Swal.fire('Lỗi!', 'Không thể cập nhật thông tin', 'error');
        }
    });
}

// Xóa bệnh nhân
function deletePatient(maBenhNhan, tenBenhNhan) {
    Swal.fire({
        title: 'Xác nhận xóa',
        text: `Bạn có chắc chắn muốn xóa bệnh nhân "${tenBenhNhan}"?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Xóa',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: `/BenhNhan/XoaBenhNhan/${maBenhNhan}`,
                method: 'POST',
                success: function (response) {
                    if (response.success) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Đã xóa!',
                            text: response.message,
                            timer: 2000,
                            showConfirmButton: false
                        });

                        // Reload danh sách
                        loadPatientContent('/BenhNhan/_DSBenhNhan');
                    } else {
                        Swal.fire('Lỗi!', response.message, 'error');
                    }
                },
                error: function (xhr) {
                    Swal.fire('Lỗi!', 'Không thể xóa bệnh nhân', 'error');
                }
            });
        }
    });
}

// Export Excel danh sách bệnh nhân
function exportPatientExcel() {
    window.location.href = '/BenhNhan/ExportExcel';

    // Show success message
    setTimeout(function () {
        showToast('Đang xuất file Excel...', 'info');
    }, 500);
}