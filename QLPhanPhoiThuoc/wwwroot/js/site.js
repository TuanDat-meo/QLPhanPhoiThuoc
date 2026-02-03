// ==================== OPTIMIZED JAVASCRIPT - PERFORMANCE FOCUSED ====================

// Utility: Debounce function for better performance
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

// Utility: Throttle function
function throttle(func, limit) {
    let inThrottle;
    return function(...args) {
        if (!inThrottle) {
            func.apply(this, args);
            inThrottle = true;
            setTimeout(() => inThrottle = false, limit);
        }
    };
}

// ==================== DOCUMENT READY ====================

$(document).ready(function() {
    initializeComponents();
    setupEventListeners();
});

// Initialize all components
function initializeComponents() {
    // Initialize tooltips (if Bootstrap is used)
    if (typeof bootstrap !== 'undefined') {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
        
        // Initialize popovers
        const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
        popoverTriggerList.map(function (popoverTriggerEl) {
            return new bootstrap.Popover(popoverTriggerEl);
        });
    }
    
    // Auto-hide alerts after 5 seconds
    setTimeout(function() {
        $('.alert').fadeOut('slow');
    }, 5000);
    
    // Format currency and dates on page load
    formatPageContent();
}

// Setup event listeners
function setupEventListeners() {
    // Mobile sidebar toggle
    $('#sidebarToggle').on('click', function(e) {
        e.stopPropagation();
        $('.sidebar').toggleClass('active');
    });
    
    // Close sidebar when clicking outside on mobile
    $(document).on('click', function(event) {
        if (!$(event.target).closest('.sidebar, #sidebarToggle').length) {
            $('.sidebar').removeClass('active');
        }
    });
    
    // Smooth scroll for anchor links
    $(document).on('click', 'a[href^="#"]', function(e) {
        const target = $(this).attr('href');
        if (target !== '#' && target !== '#!') {
            const $target = $(target);
            if ($target.length) {
                e.preventDefault();
                $('html, body').animate({
                    scrollTop: $target.offset().top - 80
                }, 500);
            }
        }
    });
    
    // Form validation
    $('form').on('submit', function(e) {
        const form = this;
        if (form.checkValidity() === false) {
            e.preventDefault();
            e.stopPropagation();
        }
        $(form).addClass('was-validated');
    });
    
    // Debounced search functionality
    const debouncedSearch = debounce(function() {
        const value = $(this).val().toLowerCase();
        $('.searchable-item').each(function() {
            const text = $(this).text().toLowerCase();
            $(this).toggle(text.indexOf(value) > -1);
        });
    }, 300);
    
    $('#searchInput').on('keyup', debouncedSearch);
    
    // Confirm delete actions
    $(document).on('click', '.btn-delete', function(e) {
        if (!confirm('Bạn có chắc chắn muốn xóa?')) {
            e.preventDefault();
            return false;
        }
    });
}

// Format page content (currency, dates)
function formatPageContent() {
    // Format currency
    $('.format-currency').each(function() {
        const value = parseFloat($(this).text());
        if (!isNaN(value)) {
            $(this).text(formatCurrency(value));
        }
    });
    
    // Format dates
    $('.format-date').each(function() {
        const dateStr = $(this).text();
        const date = new Date(dateStr);
        if (!isNaN(date.getTime())) {
            $(this).text(formatDate(date));
        }
    });
    
    // Format date-time
    $('.format-datetime').each(function() {
        const dateStr = $(this).text();
        const date = new Date(dateStr);
        if (!isNaN(date.getTime())) {
            $(this).text(formatDateTime(date));
        }
    });
}

// ==================== FORMATTING UTILITIES ====================

// Format currency (Vietnamese Dong)
function formatCurrency(amount) {
    try {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(amount);
    } catch (error) {
        return amount.toLocaleString('vi-VN') + ' ₫';
    }
}

// Format date (dd/mm/yyyy)
function formatDate(date) {
    try {
        return new Intl.DateTimeFormat('vi-VN', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit'
        }).format(date);
    } catch (error) {
        const d = new Date(date);
        const day = String(d.getDate()).padStart(2, '0');
        const month = String(d.getMonth() + 1).padStart(2, '0');
        const year = d.getFullYear();
        return `${day}/${month}/${year}`;
    }
}

// Format date-time (dd/mm/yyyy hh:mm)
function formatDateTime(date) {
    try {
        return new Intl.DateTimeFormat('vi-VN', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit'
        }).format(date);
    } catch (error) {
        return formatDate(date) + ' ' + date.toLocaleTimeString('vi-VN', {
            hour: '2-digit',
            minute: '2-digit'
        });
    }
}

// ==================== LOADING OVERLAY ====================

// Show loading spinner
function showLoading(message = 'Đang xử lý...') {
    // Remove existing overlay first
    hideLoading();
    
    const overlay = $(`
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
            flex-direction: column;
            gap: 20px;
        ">
            <div class="spinner-border text-light" role="status" style="width: 3rem; height: 3rem;">
                <span class="visually-hidden">Loading...</span>
            </div>
            <div style="color: white; font-size: 1.1rem; font-weight: 500;">
                ${message}
            </div>
        </div>
    `);
    
    $('body').append(overlay);
}

// Hide loading spinner
function hideLoading() {
    $('#loadingOverlay').remove();
}

// ==================== TOAST NOTIFICATIONS ====================

// Show toast notification
function showToast(message, type = 'info', duration = 3000) {
    const bgClass = {
        'success': 'bg-success',
        'error': 'bg-danger',
        'warning': 'bg-warning',
        'info': 'bg-info'
    }[type] || 'bg-info';
    
    const icon = {
        'success': 'fa-check-circle',
        'error': 'fa-times-circle',
        'warning': 'fa-exclamation-triangle',
        'info': 'fa-info-circle'
    }[type] || 'fa-info-circle';
    
    const toastId = 'toast-' + Date.now();
    
    const toastHtml = `
        <div id="${toastId}" class="toast align-items-center text-white ${bgClass} border-0" role="alert" 
             aria-live="assertive" aria-atomic="true" 
             style="position: fixed; top: 20px; right: 20px; z-index: 10000; min-width: 300px;">
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
    
    const toastEl = document.getElementById(toastId);
    
    // Show toast with fade in
    $(toastEl).fadeIn(200);
    
    // Auto remove after duration
    setTimeout(function() {
        $(toastEl).fadeOut(200, function() {
            $(this).remove();
        });
    }, duration);
    
    // Remove on close button click
    $(`#${toastId} .btn-close`).on('click', function() {
        $(toastEl).fadeOut(200, function() {
            $(this).remove();
        });
    });
}

// ==================== CONFIRM DIALOG ====================

// Confirm dialog with callback
function confirmDialog(message, onConfirm, onCancel) {
    if (confirm(message)) {
        if (typeof onConfirm === 'function') {
            onConfirm();
        }
    } else {
        if (typeof onCancel === 'function') {
            onCancel();
        }
    }
}

// ==================== AJAX HELPER ====================

// AJAX request helper with loading indicator
function ajaxRequest(url, method, data, successCallback, errorCallback) {
    showLoading();
    
    const ajaxConfig = {
        url: url,
        type: method || 'GET',
        data: data,
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function(response) {
            hideLoading();
            if (typeof successCallback === 'function') {
                successCallback(response);
            }
        },
        error: function(xhr, status, error) {
            hideLoading();
            if (typeof errorCallback === 'function') {
                errorCallback(xhr, status, error);
            } else {
                const errorMsg = xhr.responseJSON?.message || error || 'Đã xảy ra lỗi';
                showToast(errorMsg, 'error');
            }
        }
    };
    
    $.ajax(ajaxConfig);
}

// ==================== EXPORT FUNCTIONS ====================

// Export table to Excel (simple method)
function exportTableToExcel(tableId, filename) {
    const table = document.getElementById(tableId);
    if (!table) {
        showToast('Không tìm thấy bảng dữ liệu', 'error');
        return;
    }
    
    const html = table.outerHTML;
    const url = 'data:application/vnd.ms-excel,' + encodeURIComponent(html);
    const downloadLink = document.createElement("a");
    
    document.body.appendChild(downloadLink);
    downloadLink.href = url;
    downloadLink.download = (filename || 'export') + '.xls';
    downloadLink.click();
    document.body.removeChild(downloadLink);
    
    showToast('Xuất file thành công', 'success');
}

// Print element content
function printElement(elementId) {
    const content = document.getElementById(elementId);
    if (!content) {
        showToast('Không tìm thấy nội dung cần in', 'error');
        return;
    }
    
    const printWindow = window.open('', '', 'height=600,width=800');
    printWindow.document.write('<html><head><title>In tài liệu</title>');
    printWindow.document.write('<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css">');
    printWindow.document.write('<style>body { padding: 20px; } @media print { .no-print { display: none; } }</style>');
    printWindow.document.write('</head><body>');
    printWindow.document.write(content.innerHTML);
    printWindow.document.write('</body></html>');
    printWindow.document.close();
    
    // Wait for content to load before printing
    setTimeout(() => {
        printWindow.print();
    }, 250);
}

// ==================== LOCAL STORAGE HELPER ====================

const storage = {
    // Set item in localStorage
    set: function(key, value) {
        try {
            localStorage.setItem(key, JSON.stringify(value));
            return true;
        } catch (error) {
            console.error('Storage error:', error);
            return false;
        }
    },
    
    // Get item from localStorage
    get: function(key) {
        try {
            const item = localStorage.getItem(key);
            return item ? JSON.parse(item) : null;
        } catch (error) {
            console.error('Storage error:', error);
            return null;
        }
    },
    
    // Remove item from localStorage
    remove: function(key) {
        try {
            localStorage.removeItem(key);
            return true;
        } catch (error) {
            console.error('Storage error:', error);
            return false;
        }
    },
    
    // Clear all localStorage
    clear: function() {
        try {
            localStorage.clear();
            return true;
        } catch (error) {
            console.error('Storage error:', error);
            return false;
        }
    }
};

// ==================== DATA TABLE HELPER ====================

// Initialize DataTable (if plugin is loaded)
function initDataTable(tableId, options = {}) {
    if (typeof $.fn.DataTable === 'undefined') {
        console.warn('DataTable plugin not loaded');
        return null;
    }
    
    const defaultOptions = {
        language: {
            url: '//cdn.datatables.net/plug-ins/1.13.4/i18n/vi.json'
        },
        pageLength: 10,
        responsive: true,
        order: [[0, 'desc']]
    };
    
    const finalOptions = $.extend({}, defaultOptions, options);
    
    return $(`#${tableId}`).DataTable(finalOptions);
}

// ==================== FORM VALIDATION HELPER ====================

// Validate form fields
function validateForm(formId) {
    const form = document.getElementById(formId);
    if (!form) return false;
    
    form.classList.add('was-validated');
    return form.checkValidity();
}

// Reset form validation
function resetFormValidation(formId) {
    const form = document.getElementById(formId);
    if (form) {
        form.classList.remove('was-validated');
    }
}

// ==================== NUMBER FORMATTING ====================

// Format number with thousand separators
function formatNumber(number) {
    return number.toLocaleString('vi-VN');
}

// Parse formatted number string to float
function parseFormattedNumber(str) {
    return parseFloat(str.replace(/[.,]/g, ''));
}

// ==================== DATE UTILITIES ====================

// Get today's date in yyyy-mm-dd format
function getTodayDate() {
    const today = new Date();
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const day = String(today.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
}

// Calculate age from date of birth
function calculateAge(birthDate) {
    const today = new Date();
    const birth = new Date(birthDate);
    let age = today.getFullYear() - birth.getFullYear();
    const monthDiff = today.getMonth() - birth.getMonth();
    
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birth.getDate())) {
        age--;
    }
    
    return age;
}

// ==================== COPY TO CLIPBOARD ====================

// Copy text to clipboard
function copyToClipboard(text) {
    if (navigator.clipboard) {
        navigator.clipboard.writeText(text).then(function() {
            showToast('Đã sao chép vào clipboard', 'success', 2000);
        }).catch(function() {
            showToast('Không thể sao chép', 'error');
        });
    } else {
        // Fallback for older browsers
        const textArea = document.createElement('textarea');
        textArea.value = text;
        textArea.style.position = 'fixed';
        textArea.style.left = '-999999px';
        document.body.appendChild(textArea);
        textArea.select();
        try {
            document.execCommand('copy');
            showToast('Đã sao chép vào clipboard', 'success', 2000);
        } catch (err) {
            showToast('Không thể sao chép', 'error');
        }
        document.body.removeChild(textArea);
    }
}

// ==================== PERFORMANCE MONITORING ====================

// Log performance metrics (for debugging)
function logPerformance() {
    if (window.performance && window.performance.timing) {
        const timing = window.performance.timing;
        const loadTime = timing.loadEventEnd - timing.navigationStart;
        const domReadyTime = timing.domContentLoadedEventEnd - timing.navigationStart;
        
        console.log('Page Load Time:', loadTime + 'ms');
        console.log('DOM Ready Time:', domReadyTime + 'ms');
    }
}

// Call on window load
$(window).on('load', function() {
    // Uncomment to see performance metrics
    // logPerformance();
});
