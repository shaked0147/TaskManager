document.getElementById('loginBtn').addEventListener('click', login);

async function login() {
    const errorDiv = document.getElementById('error');
    errorDiv.textContent = '';
    const username = document.getElementById('username').value.trim();
    const password = document.getElementById('password').value.trim();

    if (!username || !password) {
        errorDiv.textContent = 'Please enter both username and password.';
        return;
    }

    try {
        const response = await fetch('https://localhost:7016/api/user/login', {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({ username, password })
        });

        if (!response.ok) {
            throw new Error('Invalid credentials.');
        }

        const data = await response.json();
        localStorage.setItem('token', data.token);
        window.location.href = 'dashboard.html';
    } catch (err) {
        errorDiv.textContent = err.message;
    }
}
