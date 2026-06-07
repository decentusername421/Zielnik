document.getElementById('loginForm').addEventListener('submit', async (e) => {
    e.preventDefault();

    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    try {
        const response = await fetch('/api/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email: email, password: password })
        });

        if (response.ok) {
            const data = await response.json();
            const token = data.token;

            localStorage.setItem('jwtToken', token);

            alert("Zalogowano pomyślnie!");
            window.location.href = '/Home/Index';
        } else {
            alert("Niepoprawny email lub hasło.");
        }
    } catch (error) {
        console.error("Błąd połączenia:", error);
    }
});
