// Base URL of the ASP.NET Core API.
// Change the port if you run the API on a different one.
const API_BASE = "http://localhost:5080/api";

const tokenKey = "camerashop_token";
const userKey = "camerashop_user";

// ---------- Auth helpers ----------
function getToken() { return localStorage.getItem(tokenKey); }
function getUser()  { return JSON.parse(localStorage.getItem(userKey) || "null"); }

function setSession(auth) {
  localStorage.setItem(tokenKey, auth.token);
  localStorage.setItem(userKey, JSON.stringify({
    username: auth.username,
    role: auth.role
  }));
  renderAuthArea();
}

function clearSession() {
  localStorage.removeItem(tokenKey);
  localStorage.removeItem(userKey);
  renderAuthArea();
}

function renderAuthArea() {
  const user = getUser();
  const label = document.getElementById("user-label");
  const logoutBtn = document.getElementById("logout-btn");
  const loginSection = document.getElementById("login-section");

  if (user) {
    label.textContent = `Signed in as ${user.username} (${user.role})`;
    label.classList.remove("hidden");
    logoutBtn.classList.remove("hidden");
    loginSection.classList.add("hidden");
  } else {
    label.classList.add("hidden");
    logoutBtn.classList.add("hidden");
    loginSection.classList.remove("hidden");
  }
}

// ---------- API endpoint: GET /api/cameras ----------
// This is the JS endpoint hookup the brief asks for.
async function loadCameras() {
  const list = document.getElementById("camera-list");
  list.textContent = "Loading...";

  try {
    const res = await fetch(`${API_BASE}/cameras`);
    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    const cameras = await res.json();

    if (!cameras.length) {
      list.textContent = "No cameras yet.";
      return;
    }

    list.innerHTML = "";
    for (const cam of cameras) {
      const card = document.createElement("article");
      card.className = "card";
      card.innerHTML = `
        <img src="${cam.imageUrl}" alt="${cam.model}" />
        <div class="body">
          <h3>${cam.model}</h3>
          <div class="brand">${cam.brandName} • ${cam.megaPixels} MP • ${cam.sensorType}</div>
          <div class="price">$${Number(cam.price).toFixed(2)}</div>
          <div>${cam.stockQuantity} in stock</div>
          <div class="tags">
            ${cam.categories.map(c => `<span class="tag">${c}</span>`).join("")}
          </div>
        </div>
      `;
      list.appendChild(card);
    }
  } catch (err) {
    list.innerHTML = `
      <p class="error">
        Could not reach the API at ${API_BASE}. Make sure CameraShop.API is running.
        (${err.message})
      </p>`;
  }
}

// ---------- Login flow ----------
async function handleLogin(event) {
  event.preventDefault();
  const errBox = document.getElementById("login-error");
  errBox.textContent = "";

  const form = event.target;
  const payload = {
    usernameOrEmail: form.usernameOrEmail.value.trim(),
    password: form.password.value
  };

  try {
    const res = await fetch(`${API_BASE}/Auth/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    });
    const data = await res.json();
    if (!res.ok) throw new Error(data.message || "Login failed");
    setSession(data);
    form.reset();
  } catch (err) {
    errBox.textContent = err.message;
  }
}

// ---------- Wire up ----------
document.getElementById("login-form").addEventListener("submit", handleLogin);
document.getElementById("logout-btn").addEventListener("click", clearSession);

renderAuthArea();
loadCameras();
