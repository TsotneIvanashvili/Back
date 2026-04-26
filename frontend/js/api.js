
const API_BASE = "http://localhost:5080/api";

function authHeader() {
  const token = localStorage.getItem("cc_token");
  return token ? { Authorization: `Bearer ${token}` } : {};
}

async function request(method, path, body) {
  const opts = {
    method,
    headers: { "Content-Type": "application/json", ...authHeader() }
  };
  if (body !== undefined) opts.body = JSON.stringify(body);

  const res = await fetch(`${API_BASE}${path}`, opts);
  if (res.status === 204) return null;

  let data;
  try { data = await res.json(); } catch { data = null; }

  if (!res.ok) {
    const msg = (data && (data.message || data.title)) || `HTTP ${res.status}`;
    const err = new Error(msg);
    err.status = res.status;
    err.payload = data;
    throw err;
  }
  return data;
}

window.API = {
  get:  (p)        => request("GET", p),
  post: (p, body)  => request("POST", p, body),
  put:  (p, body)  => request("PUT", p, body),
  del:  (p)        => request("DELETE", p),
  BASE: API_BASE
};


window.toast = function (message, type = "info") {
  const node = document.createElement("div");
  node.className = `toast ${type}`;
  node.textContent = message;
  document.body.appendChild(node);
  setTimeout(() => node.remove(), 3500);
};


window.fmt = {
  money: (n) => `$${Number(n).toLocaleString("en-US", { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`
};


document.addEventListener("error", (e) => {
  const t = e.target;
  if (t && t.tagName === "IMG" && !t.dataset.fallback) {
    t.dataset.fallback = "1";
    const seed = encodeURIComponent(t.alt || "camera");
    t.src = `https://picsum.photos/seed/${seed}/800/600`;
  }
}, true);
