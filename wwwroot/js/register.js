document.getElementById('registerForm').addEventListener('submit', async (e) => {
    e.preventDefault();

    const data = {
        Username: document.getElementById('username').value,
        Email: document.getElementById('email').value,
        Password: document.getElementById('password').value
    };

    console.log("Wysyłam do serwera:", JSON.stringify(data));

    try {
        const response = await fetch('/api/auth/register', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        const result = await response.text();
        console.log("Odpowiedź serwera:", response.status, result);

        if (response.ok) {
            alert("Udało się. Spróbuj się zalogować");
        } else {
            alert("Błąd rejestracji: " + result);
        }
    } catch (error) {
        console.error("Błąd:", error);
    }
});
