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

            // Debug: sprawdzamy co dokładnie dostał skrypt
            console.log("ODPOWIEDŹ Z SERWERA:", data);

            // Jeśli konsola pokaże, że token jest w 'data.token', to zadziała.
            // Jeśli nie, zobaczysz nazwę pola w konsoli i zmienimy to jedną literą.
            const token = data.token || data.accessToken || data;

            localStorage.setItem('jwtToken', token);

            alert("Zalogowano pomyślnie!");
            window.location.href = '/Home/Index';
        } else {
            alert("Błąd logowania.");
        }
    } catch (error) {
        console.error("Błąd połączenia:", error);
    }
});