window.addEventListener('scroll', () => {
    const header = document.querySelector('.header');
    if (window.scrollY > 20) {
        header.style.boxShadow = '0 10px 15px -3px rgba(0,0,0,0.1)';
        header.style.background = 'rgba(255, 255, 255, 0.98)';
    } else {
        header.style.boxShadow = 'var(--shadow)';
        header.style.background = 'rgba(255, 255, 255, 0.9)';
    }
});

const catItems = document.querySelectorAll('.cat-item');
catItems.forEach(item => {
    item.addEventListener('click', () => {
        catItems.forEach(i => i.classList.remove('active'));
        item.classList.add('active');
    });
});

function toggleLike(button) {
    const span = button.querySelector('span');
    let currentLikes = parseInt(span.innerText);
    
    if (button.classList.contains('liked')) {
        button.classList.remove('liked');
        span.innerText = currentLikes - 1;
        button.style.transform = 'scale(1)';
    } else {
        button.classList.add('liked');
        span.innerText = currentLikes + 1;
        button.style.transform = 'scale(1.15)';
        setTimeout(() => button.style.transform = 'scale(1)', 200);
    }
}

const modal = document.getElementById('reportModal');
const btnClose = document.getElementById('btnCloseModal');
const btnCancel = document.getElementById('btnCancelModal');
const reportForm = document.getElementById('reportForm');
const dropZone = document.getElementById('dropZone');
const fileInput = document.getElementById('fileInput');

const btnHeader = document.getElementById('btnOpenReportHeader');
const btnHero = document.getElementById('btnOpenReportHero');

function openModal(e) {
    e.preventDefault();
    if(modal) {
        modal.classList.add('active');
        document.body.style.overflow = 'hidden';
    }
}

if (btnHeader) btnHeader.addEventListener('click', openModal);
if (btnHero) btnHero.addEventListener('click', openModal);

function closeModal() {
    if(modal) {
        modal.classList.remove('active');
        document.body.style.overflow = 'auto';
        setTimeout(() => {
            if(reportForm) reportForm.reset();
            updateDropZoneText([]);
        }, 400);
    }
}

if (btnClose) btnClose.addEventListener('click', closeModal);
if (btnCancel) btnCancel.addEventListener('click', closeModal);

if (modal) {
    modal.addEventListener('click', (e) => {
        if (e.target === modal) {
            closeModal();
        }
    });
}

if (dropZone && fileInput) {
    dropZone.addEventListener('click', () => {
        fileInput.click();
    });

    dropZone.addEventListener('dragover', (e) => {
        e.preventDefault();
        dropZone.style.borderColor = '#0ea5e9';
        dropZone.style.background = '#f0f9ff';
    });

    dropZone.addEventListener('dragleave', (e) => {
        e.preventDefault();
        dropZone.style.borderColor = '#cbd5e1';
        dropZone.style.background = '#f8fafc';
    });

    dropZone.addEventListener('drop', (e) => {
        e.preventDefault();
        dropZone.style.borderColor = '#cbd5e1';
        dropZone.style.background = '#f8fafc';
        if (e.dataTransfer.files.length) {
            fileInput.files = e.dataTransfer.files;
            updateDropZoneText(e.dataTransfer.files);
        }
    });

    fileInput.addEventListener('change', function() {
        if (this.files.length) {
            updateDropZoneText(this.files);
        }
    });
}

function updateDropZoneText(files) {
    if (!dropZone) return;
    const textSpan = dropZone.querySelector('span:nth-of-type(2)');
    if (files.length > 0) {
        textSpan.innerText = `Đã chọn ${files.length} ảnh. Nhấn để thay đổi.`;
        textSpan.style.color = '#0ea5e9';
    } else {
        textSpan.innerText = `Nhấn để chọn ảnh hoặc kéo thả vào đây`;
        textSpan.style.color = '#0f172a';
    }
}

if (reportForm) {
    reportForm.addEventListener('submit', (e) => {
        e.preventDefault();
        const btnSubmit = reportForm.querySelector('button[type="submit"]');
        const originalText = btnSubmit.innerHTML;
        
        btnSubmit.innerHTML = '⏳ Đang xử lý...';
        btnSubmit.style.opacity = '0.7';
        
        setTimeout(() => {
            closeModal();
            btnSubmit.innerHTML = originalText;
            btnSubmit.style.opacity = '1';
            alert('Cảm ơn bạn! Phản ánh đã được gửi thành công và đang chờ kiểm duyệt.');
        }, 1500);
    });
}