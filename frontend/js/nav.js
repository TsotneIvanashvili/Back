

(function () {
  const page = (document.body.dataset.page || "").toLowerCase();

  function navHTML() {
    const user = Auth.user();
    const loggedIn = Auth.isLoggedIn();
    const isAdmin = Auth.isAdmin();

    const link = (href, label, key) =>
      `<li><a href="${href}" class="${page === key ? "active" : ""}">${label}</a></li>`;

    const userBlock = loggedIn
      ? `<div class="user-menu" id="user-menu">
           <button class="user-menu-trigger" id="user-menu-trigger">${user.username}</button>
           <div class="user-menu-dropdown">
             ${isAdmin ? '<a href="admin.html">Admin Dashboard</a>' : ""}
             <button id="logout-btn">Sign out</button>
           </div>
         </div>`
      : `<a href="auth.html" class="btn-link">Sign in</a>`;

    return `
      <nav class="nav">
        <a href="index.html" class="brand">Capture<span>Core</span></a>
        <ul class="nav-links">
          ${link("index.html", "Home", "home")}
          ${link("products.html", "Cameras", "products")}
        </ul>
        <div class="nav-actions">
          <a href="cart.html" class="cart-btn" aria-label="Cart">
            Cart <span class="cart-badge hidden" id="cart-badge">0</span>
          </a>
          ${userBlock}
        </div>
      </nav>
    `;
  }

  function footerHTML() {
    return `
      <div class="container">
        <div class="footer-cols">
          <div>
            <a href="index.html" class="brand" style="font-size:1.6rem;">Capture<span>Core</span></a>
            <p style="margin-top:1rem;max-width:340px;font-size:.9rem;">
              Cameras, lenses, and gear curated for people who actually shoot.
              Free shipping in 2&ndash;3 business days on every order.
            </p>
          </div>
          <div>
            <h4>Shop</h4>
            <ul>
              <li><a href="products.html">All cameras</a></li>
              <li><a href="products.html?cat=Mirrorless">Mirrorless</a></li>
              <li><a href="products.html?cat=DSLR">DSLR</a></li>
              <li><a href="products.html?cat=Action">Action</a></li>
            </ul>
          </div>
          <div>
            <h4>Support</h4>
            <ul>
              <li><a href="#">Shipping &amp; returns</a></li>
              <li><a href="#">Warranty</a></li>
              <li><a href="#">Contact</a></li>
            </ul>
          </div>
          <div>
            <h4>Company</h4>
            <ul>
              <li><a href="#">About</a></li>
              <li><a href="#">Press</a></li>
              <li><a href="#">Careers</a></li>
            </ul>
          </div>
        </div>
        <div class="copy">
          <span>&copy; ${new Date().getFullYear()} CaptureCore. All rights reserved.</span>
          <span>Made for people who shoot.</span>
        </div>
      </div>
    `;
  }

  function mount() {
    const header = document.getElementById("site-header");
    const footer = document.getElementById("site-footer");
    if (header) header.innerHTML = navHTML();
    if (footer) footer.innerHTML = footerHTML();
    const trigger = document.getElementById("user-menu-trigger");
    if (trigger) {
      const menu = document.getElementById("user-menu");
      trigger.addEventListener("click", (e) => {
        e.stopPropagation();
        menu.classList.toggle("open");
      });
      document.addEventListener("click", () => menu.classList.remove("open"));
    }
    const logoutBtn = document.getElementById("logout-btn");
    if (logoutBtn) {
      logoutBtn.addEventListener("click", () => {
        Auth.logout();
        location.href = "index.html";
      });
    }

    refreshCartBadge();
  }

  async function refreshCartBadge() {
    if (!Auth.isLoggedIn()) return;
    try {
      const cart = await API.get("/cart");
      const badge = document.getElementById("cart-badge");
      if (!badge) return;
      if (cart.itemCount > 0) {
        badge.textContent = cart.itemCount;
        badge.classList.remove("hidden");
      } else {
        badge.classList.add("hidden");
      }
    } catch (_) {  }
  }

  window.Nav = { mount, refreshCartBadge };

  document.addEventListener("DOMContentLoaded", mount);
})();
