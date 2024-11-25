document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('registerForm');
    const inputs = {
        name: document.getElementById('name'),
        email: document.getElementById('email'),
        password: document.getElementById('password'),
        confirmPassword: document.getElementById('confirmPassword')
    };

    const validateEmail = (email) => {
        const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(email);
    };

    const validatePassword = (password) => {
        return password.length >= 8;
    };

    const showError = (element, message) => {
        element.classList.add('error');
        const errorDiv = document.createElement('div');
        errorDiv.className = 'error-message';
        errorDiv.textContent = message;

        const existingError = element.parentElement.querySelector('.error-message');
        if (!existingError) {
            element.parentElement.appendChild(errorDiv);
        }
    };

    const removeError = (element) => {
        element.classList.remove('error');
        const errorDiv = element.parentElement.querySelector('.error-message');
        if (errorDiv) {
            errorDiv.remove();
        }
    };

    // Real-time validation
    Object.keys(inputs).forEach(key => {
        inputs[key].addEventListener('input', (e) => {
            removeError(e.target);
        });
    });

    form.addEventListener('submit', (e) => {
        e.preventDefault();
        let isValid = true;

        // Validate name
        if (inputs.name.value.trim() === '') {
            showError(inputs.name, 'Name is required');
            isValid = false;
        }

        // Validate email
        if (!validateEmail(inputs.email.value)) {
            showError(inputs.email, 'Please enter a valid email address');
            isValid = false;
        }

        // Validate password
        if (!validatePassword(inputs.password.value)) {
            showError(inputs.password, 'Password must be at least 8 characters long');
            isValid = false;
        }

        // Validate password confirmation
        if (inputs.password.value !== inputs.confirmPassword.value) {
            showError(inputs.confirmPassword, 'Passwords do not match');
            isValid = false;
        }

        if (isValid) {
            // Here you would typically send the data to a server
            alert('Registration successful!');
            form.reset();
        }
    });
});