

window.Auth = {
  saveSession(payload) {
    localStorage.setItem("cc_token", payload.token);
    localStorage.setItem("cc_user", JSON.stringify({
      username: payload.username,
      role: payload.role,
      expiresAt: payload.expiresAt
    }));
  },
  user() {
    try { return JSON.parse(localStorage.getItem("cc_user") || "null"); }
    catch { return null; }
  },
  isLoggedIn() {
    const u = this.user();
    if (!u) return false;
    if (u.expiresAt && new Date(u.expiresAt) < new Date()) {
      this.logout();
      return false;
    }
    return true;
  },
  isAdmin() { return this.isLoggedIn() && this.user()?.role === "Admin"; },
  logout() {
    localStorage.removeItem("cc_token");
    localStorage.removeItem("cc_user");
  },
  
  requireLogin(opts = {}) {
    if (!this.isLoggedIn()) {
      const here = encodeURIComponent(location.pathname.split("/").pop() || "");
      location.href = `auth.html?next=${here}`;
      return false;
    }
    if (opts.adminOnly && !this.isAdmin()) {
      location.href = "index.html";
      return false;
    }
    return true;
  }
};
