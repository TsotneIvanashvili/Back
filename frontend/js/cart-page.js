

(async function () {
  if (!Auth.requireLogin()) return;

  const host = document.getElementById("cart-host");

  async function refresh() {
    let cart;
    try {
      cart = await API.get("/cart");
    } catch (err) {
      host.innerHTML = `<div class="empty"><h3>Couldn't load your cart</h3></div>`;
      return;
    }

    if (!cart.items.length) {
      host.innerHTML = `
        <div class="empty" style="padding:6rem 0;">
          <h3 style="margin-bottom:.75rem;">Your cart is empty</h3>
          <p style="margin-bottom:1.5rem;">Pick up something to shoot with.</p>
          <a href="products.html" class="btn btn-primary">Browse cameras</a>
        </div>`;
      Nav.refreshCartBadge();
      return;
    }

    host.innerHTML = `
      <div class="cart-layout">
        <div id="lines">
          ${cart.items.map(lineHTML).join("")}
        </div>
        <aside class="cart-summary">
          <h3>Order summary</h3>
          <div class="summary-row"><span>Items</span><span>${cart.itemCount}</span></div>
          <div class="summary-row"><span>Shipping</span><span>Free</span></div>
          <div class="summary-row total"><span>Total</span><span>${fmt.money(cart.total)}</span></div>

          <label style="display:block;margin-top:1.5rem;font-size:.72rem;letter-spacing:.2em;text-transform:uppercase;color:var(--text-dim);">
            Shipping address
          </label>
          <textarea class="shipping" id="shipping-input" placeholder="Street, city, country">${prefillAddress()}</textarea>

          <button class="btn btn-primary" id="checkout-btn" style="width:100%;">Place order</button>
          <button class="btn btn-ghost btn-sm" id="clear-btn" style="width:100%;margin-top:.75rem;">Empty cart</button>
        </aside>
      </div>`;

    bindLineEvents();
    document.getElementById("checkout-btn").addEventListener("click", checkout);
    document.getElementById("clear-btn").addEventListener("click", clearCart);
    Nav.refreshCartBadge();
  }

  function lineHTML(item) {
    return `
      <div class="cart-line" data-id="${item.cameraId}">
        <div class="thumb"><img src="${item.imageUrl}" alt="${item.cameraModel}" /></div>
        <div>
          <h4 class="name">${item.cameraModel}</h4>
          <div class="price">${fmt.money(item.unitPrice)} each</div>
          <div class="qty-stepper" style="margin-top:.75rem;">
            <button class="dec" aria-label="Decrease">&minus;</button>
            <input class="qty" type="number" value="${item.quantity}" min="1" />
            <button class="inc" aria-label="Increase">+</button>
          </div>
        </div>
        <div class="actions">
          <span class="line-total">${fmt.money(item.lineTotal)}</span>
          <button class="remove">Remove</button>
        </div>
      </div>`;
  }

  function bindLineEvents() {
    document.querySelectorAll(".cart-line").forEach((row) => {
      const id = parseInt(row.dataset.id, 10);
      const qtyInput = row.querySelector(".qty");

      const update = async (newQty) => {
        if (newQty < 1) return;
        try {
          await API.put(`/cart/items/${id}`, { quantity: newQty });
          await refresh();
        } catch (err) { toast(err.message, "error"); }
      };

      row.querySelector(".dec").addEventListener("click", () => update(parseInt(qtyInput.value, 10) - 1));
      row.querySelector(".inc").addEventListener("click", () => update(parseInt(qtyInput.value, 10) + 1));
      qtyInput.addEventListener("change", () => update(parseInt(qtyInput.value, 10)));

      row.querySelector(".remove").addEventListener("click", async () => {
        try {
          await API.del(`/cart/items/${id}`);
          toast("Item removed", "success");
          refresh();
        } catch (err) { toast(err.message, "error"); }
      });
    });
  }

  async function checkout() {
    const shipping = document.getElementById("shipping-input").value.trim();
    if (shipping.length < 4) {
      toast("Please enter a shipping address.", "error");
      return;
    }
    const btn = document.getElementById("checkout-btn");
    btn.disabled = true;
    btn.textContent = "Placing order…";
    try {
      const order = await API.post("/cart/checkout", { shippingAddress: shipping });
      host.innerHTML = `
        <div class="empty" style="padding:5rem 0;">
          <p class="eyebrow">Order confirmed</p>
          <h2>Thank you. Your order is in.</h2>
          <p style="max-width:480px;margin:1rem auto 2rem;">
            Order <strong>#${order.id}</strong> for <strong>${fmt.money(order.totalAmount)}</strong>
            is now being prepared. We'll email you when it ships.
          </p>
          <a href="products.html" class="btn btn-ghost">Keep shopping</a>
        </div>`;
      Nav.refreshCartBadge();
    } catch (err) {
      btn.disabled = false;
      btn.textContent = "Place order";
      toast(err.message || "Could not place order.", "error");
    }
  }

  async function clearCart() {
    if (!confirm("Empty your cart?")) return;
    try {
      await API.del("/cart");
      toast("Cart emptied", "success");
      refresh();
    } catch (err) { toast(err.message, "error"); }
  }

  function prefillAddress() {
    const u = Auth.user();
    return u && u.address ? u.address : "";
  }

  refresh();
})();
