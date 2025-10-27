window.onload = () => {
    const form = document.querySelector('form');
    if (!form) return;

    const inputs = form.querySelectorAll('input, select, textarea');

    inputs.forEach(input => {
        input.addEventListener('input', () => {
            input.classList.remove('is-valid', 'is-invalid');
        });
        input.addEventListener('change', () => {
            input.classList.remove('is-valid', 'is-invalid');
        });
    });

    form.addEventListener('submit', e => {
        setTimeout(() => {
            inputs.forEach(input => {
                const errorSpan = input.closest('div').querySelector('.invalid-feedback');

                if (errorSpan) {
                    const hasError = errorSpan.textContent.trim().length > 0;

                    if (hasError) {
                        input.classList.add('is-invalid');
                        input.classList.remove('is-valid');
                    } else {
                        input.classList.add('is-valid');
                        input.classList.remove('is-invalid');
                    }
                }
            });
        }, 0);
    });
};