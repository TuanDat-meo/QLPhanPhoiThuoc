// Custom JavaScript for Hospital Management System

$(document).ready(function() {
    // Initialize tooltips
    $('[data-bs-toggle="tooltip"]').tooltip();
    
    // Initialize popovers
    $('[data-bs-toggle="popover"]').popover();
    
    // Auto-hide alerts after 5 seconds
    setTimeout(function() {
        $('.alert').fadeOut('slow');
    }, 5000);
    
    // Mobile sidebar toggle
    $('#sidebarToggle').click(function() {
        $('.sidebar').toggleClass('active');
    });
    
    // Close sidebar when clicking outside on mobile
    $(document).click(function(event) {
        if (!$(event.target).closest('.sidebar, #sidebarToggle').length) {
            $('.sidebar').removeClass('active');
        }
    });
    
    // Smooth scroll for anchor links
    $('a[href^="#"]').on('click', function(e) {
        var target = $(this).attr('href');
        if (target !== '#') {
            e.preventDefault();
            $('html, body').animate({
                scrollTop: $(target).offset().top - 80
            }, 500);
        }
    });
    
    // Form validation feedback
    $('form').submit(function(e) {
        var form = $(this);
        if (form[0].checkValidity() === false) {
            e.preventDefault();
            e.stopPropagation();
        }
        form.addClass('was-validated');
    });
    
    // Search functionality (example)
    $('#searchInput').on('keyup', function() {
        var value = $(this).val().toLowerCase();
        $('.searchable-item').filter(function() {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
        });
    });
    
    // Confirm delete actions
    $('.btn-delete').click(function(e) {
        if (!confirm('Bạn có chắc chắn muốn xóa?')) {
            e.preventDefault();
            return false;
        }
    });
    
    // Number formatting
    $('.format-currency').each(function() {
        var value = parseFloat($(this).text());
        if (!isNaN(value)) {
            $(this).text(formatCurrency(value));
        }
    });
    
    // Date formatting
    $('.format-date').each(function() {
        var date = new Date($(this).text());
        if (!isNaN(date.getTime())) {
            $(this).text(formatDate(date));
        }
    });
});

// Utility functions
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(amount);
}

function formatDate(date) {
    return new Intl.DateTimeFormat('vi-VN', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit'
    }).format(date);
}

function formatDateTime(date) {
    return new Intl.DateTimeFormat('vi-VN', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit'
    }).format(date);
}

// Show loading spinner
function showLoading() {
    $('body').append(`
        <div id="loadingOverlay" style="
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0,0,0,0.5);
            z-index: 9999;
            display: flex;
            align-items: center;
            justify-content: center;
        ">
            <div class="spinner-border text-light" role="status" style="width: 3rem; height: 3rem;">
                <span class="visually-hidden">Loading...</span>
            </div>
        </div>
    `);
}

// Hide loading spinner
function hideLoading() {
    $('#loadingOverlay').remove();
}

// Show toast notification
function showToast(message, type = 'info') {
    var bgClass = {
        'success': 'bg-success',
        'error': 'bg-danger',
        'warning': 'bg-warning',
        'info': 'bg-info'
    }[type] || 'bg-info';
    
    var icon = {
        'success': 'fa-check-circle',
        'error': 'fa-times-circle',
        'warning': 'fa-exclamation-triangle',
        'info': 'fa-info-circle'
    }[type] || 'fa-info-circle';
    
    var toastHtml = `
        <div class="toast align-items-center text-white ${bgClass} border-0" role="alert" 
             aria-live="assertive" aria-atomic="true" style="position: fixed; top: 20px; right: 20px; z-index: 9999;">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="fas ${icon} me-2"></i>${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" 
                        data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;
    
    $('body').append(toastHtml);
    var toastEl = $('.toast').last()[0];
    var toast = new bootstrap.Toast(toastEl, { delay: 3000 });
    toast.show();
    
    setTimeout(function() {
        $(toastEl).remove();
    }, 4000);
}

// Confirm dialog
function confirmDialog(message, callback) {
    if (confirm(message)) {
        callback();
    }
}

// AJAX helper
function ajaxRequest(url, method, data, successCallback, errorCallback) {
    showLoading();
    
    $.ajax({
        url: url,
        type: method,
        data: data,
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function(response) {
            hideLoading();
            if (successCallback) successCallback(response);
        },
        error: function(xhr, status, error) {
            hideLoading();
            if (errorCallback) {
                errorCallback(xhr, status, error);
            } else {
                showToast('Đã xảy ra lỗi: ' + error, 'error');
            }
        }
    });
}

// Export table to Excel (requires additional library)
function exportTableToExcel(tableId, filename) {
    var table = document.getElementById(tableId);
    var html = table.outerHTML;
    var url = 'data:application/vnd.ms-excel,' + encodeURIComponent(html);
    var downloadLink = document.createElement("a");
    document.body.appendChild(downloadLink);
    downloadLink.href = url;
    downloadLink.download = filename + '.xls';
    downloadLink.click();
    document.body.removeChild(downloadLink);
}

// Print element
function printElement(elementId) {
    var content = document.getElementById(elementId).innerHTML;
    var printWindow = window.open('', '', 'height=600,width=800');
    printWindow.document.write('<html><head><title>Print</title>');
    printWindow.document.write('<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css">');
    printWindow.document.write('</head><body>');
    printWindow.document.write(content);
    printWindow.document.write('</body></html>');
    printWindow.document.close();
    printWindow.print();
}

// Debounce function for search
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Local storage helper
const storage = {
    set: function(key, value) {
        localStorage.setItem(key, JSON.stringify(value));
    },
    get: function(key) {
        const item = localStorage.getItem(key);
        return item ? JSON.parse(item) : null;
    },
    remove: function(key) {
        localStorage.removeItem(key);
    },
    clear: function() {
        localStorage.clear();
    }
};
