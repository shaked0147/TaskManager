import { config } from './config.js';

document.getElementById('loginBtn').addEventListener('click', login);

async function login() {
    const errorDiv = document.getElementById('error');
    errorDiv.textContent = '';

    const username = document.getElementById('username').value.trim();
    const password = document.getElementById('password').value.trim();

    // Validation
    if (!username || !password) {
        errorDiv.textContent = 'Please enter both username and password.';
        return;
    }

    if (username.length < 3) {
        errorDiv.textContent = 'Username must be at least 3 characters.';
        return;
    }

    if (password.length < 6) {
        errorDiv.textContent = 'Password must be at least 6 characters.';
        return;
    }

    try {
        const response = await fetch(`${config.apiBaseUrl}/user/login`, {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({ username, password })
        });

        if (!response.ok) {
            if (response.status === 401) {
                throw new Error('Invalid username or password.');
            } else {
                throw new Error('Failed to login. Please try again later.');
            }
        }

        const data = await response.json();
        localStorage.setItem('token', data.token);
        window.location.href = 'dashboard.html';
    } catch (err) {
        errorDiv.textContent = err.message;
    }
}
