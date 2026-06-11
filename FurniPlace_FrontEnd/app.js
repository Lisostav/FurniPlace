const API_URL = 'http://localhost:5000/api'; 

let currentUserId = localStorage.getItem('userId');
let currentLogin = localStorage.getItem('login');

function init() {
    updateNav();
    showTab('catalog');
}

function showTab(tabName) {
    document.getElementById('tab-catalog').style.display = 'none';
    document.getElementById('tab-auth').style.display = 'none';
    document.getElementById('tab-myItems').style.display = 'none';
    document.getElementById(`tab-${tabName}`).style.display = 'block';

    if (tabName === 'catalog') loadCatalog();
    if (tabName === 'myItems') loadMyItems();
}

function updateNav() {
    if (currentUserId) {
        document.getElementById('nav-auth').style.display = 'none';
        document.getElementById('nav-my-items').style.display = 'inline-block';
        document.getElementById('nav-logout').style.display = 'inline-block';
        document.getElementById('userNameDisplay').innerText = currentLogin;
    } else {
        document.getElementById('nav-auth').style.display = 'inline-block';
        document.getElementById('nav-my-items').style.display = 'none';
        document.getElementById('nav-logout').style.display = 'none';
    }
}

async function login() {
    const data = {
        login: document.getElementById('loginInput').value,
        password: document.getElementById('passwordInput').value
    };
    try {
        const res = await fetch(`${API_URL}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });
        if (res.ok) {
            const user = await res.json();
            loginSuccess(user);
        } else alert("Невірний логін або пароль!");
    } catch (e) { console.error(e); }
}

async function register() {
    const data = {
        login: document.getElementById('loginInput').value,
        password: document.getElementById('passwordInput').value
    };
    try {
        const res = await fetch(`${API_URL}/auth/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });
        if (res.ok) {
            const user = await res.json();
            loginSuccess(user);
        } else alert("Помилка реєстрації. Можливо логін зайнятий.");
    } catch (e) { console.error(e); }
}

function loginSuccess(user) {
    currentUserId = user.userId;
    currentLogin = user.login;
    localStorage.setItem('userId', currentUserId);
    localStorage.setItem('login', currentLogin);
    updateNav();
    showTab('catalog');
}

function logout() {
    currentUserId = null;
    currentLogin = null;
    localStorage.removeItem('userId');
    localStorage.removeItem('login');
    updateNav();
    showTab('catalog');
}

async function loadCatalog() {
    const searchQuery = document.getElementById('searchInput').value;
    const categoryFilter = document.getElementById('categoryFilter').value;
    
    let url = `${API_URL}/furniture?`;
    if (searchQuery) url += `search=${encodeURIComponent(searchQuery)}&`;
    if (categoryFilter !== 'Всі') url += `category=${encodeURIComponent(categoryFilter)}`;

    try {
        const response = await fetch(url);
        const data = await response.json();
        renderCards(data, 'catalog', false);
    } catch (e) { console.error(e); }
}

async function loadMyItems() {
    try {
        const response = await fetch(`${API_URL}/furniture/user/${currentUserId}`);
        const data = await response.json();
        renderCards(data, 'myCatalog', true);
    } catch (e) { console.error(e); }
}

function renderCards(data, containerId, isMyItems) {
    const container = document.getElementById(containerId);
    container.innerHTML = '';
    
    data.forEach(item => {
        const imageUrl = item.imageUrl ? `http://localhost:5000${item.imageUrl}` : 'https://via.placeholder.com/200?text=Немає+фото';
        
        let extraHtml = isMyItems 
            ? `<button onclick="deleteFurniture(${item.id})" style="background-color: #e74c3c; width: 100%; margin-top: 10px;">Видалити</button>`
            : `<div class="contact-badge">📞 Контакти: ${item.contactInfo}</div>`;

        container.innerHTML += `
            <div class="card">
                <img src="${imageUrl}" style="width:100%; border-radius:8px; height:150px; object-fit:cover;">
                <h3>${item.title}</h3>
                <p style="color: #7f8c8d; font-size: 0.9em;">📂 ${item.category || 'Інше'}</p>
                <p>${item.description}</p>
                <p class="price">${item.price} грн</p>
                ${extraHtml}
            </div>
        `;
    });
}

document.getElementById('addFurnitureForm').addEventListener('submit', async function(event) {
    event.preventDefault();
    const formData = new FormData();
    formData.append('title', document.getElementById('title').value);
    formData.append('category', document.getElementById('category').value); 
    formData.append('description', document.getElementById('description').value);
    formData.append('price', document.getElementById('price').value);
    formData.append('condition', document.getElementById('condition').value);
    formData.append('contactInfo', document.getElementById('contactInfo').value);
    formData.append('userId', currentUserId); 
    
    const imageFile = document.getElementById('image').files[0];
    if (imageFile) formData.append('image', imageFile);

    try {
        await fetch(`${API_URL}/furniture`, { method: 'POST', body: formData });
        document.getElementById('addFurnitureForm').reset();
        loadMyItems(); 
    } catch (e) { console.error(e); }
});

async function deleteFurniture(id) {
    if (!confirm('Точно видалити?')) return; 
    try {
        const response = await fetch(`${API_URL}/furniture/${id}/user/${currentUserId}`, { method: 'DELETE' });
        if (response.ok) loadMyItems();
        else alert('Помилка видалення!');
    } catch (e) { console.error(e); }
}

init();