async function loadPlants() {
    const token = localStorage.getItem('jwtToken'); 

    try {
        const response = await fetch('/api/plants', {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + token, 

                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            const data = await response.json();
            console.log("Dane pobrane:", data);

        } else if (response.status === 401) {
            alert("Błąd: Nie masz uprawnień. Zaloguj się ponownie.");
        } else {
            alert("Błąd pobierania danych.");
        }
    } catch (error) {
        console.error("Błąd połączenia:", error);
    }
}


loadPlants();