

(async function () {
  const host = document.getElementById("detail-host");
  const id = new URLSearchParams(location.search).get("id");

  if (!id) {
    host.innerHTML = `<div class="empty"><h3>Missing product</h3><p><a href="products.html">Back to catalog</a></p></div>`;
    return;
  }

  let camera;
  try {
    camera = await API.get(`/cameras/${id}`);
  } catch (err) {
    host.innerHTML = `<div class="empty"><h3>Camera not found</h3><p><a href="products.html">Back to catalog</a></p></div>`;
    return;
  }

  document.title = `${camera.model} — CaptureCore`;

  const stockOut = camera.stockQuantity <= 0;
  const stockClass = stockOut ? "out" : (camera.stockQuantity <= 3 ? "low" : "in");
  const stockText = stockOut
    ? "Sold out"
    : (camera.stockQuantity <= 3 ? `Only ${camera.stockQuantity} left in stock` : `${camera.stockQuantity} in stock`);

  host.innerHTML = `
    <div class="product-detail">
      <div class="gallery">
        <img src="${camera.imageUrl}" alt="${camera.model}" />
      </div>

      <div class="info">
        <div class="brand-line">${camera.brandName}</div>
        <h1>${camera.model}</h1>
        <p class="stock ${stockClass}" style="margin: -.5rem 0 1rem;">${stockText}</p>

        <div class="price">${fmt.money(camera.price)}</div>

        <p class="desc">${camera.description}</p>

        <table class="spec-table">
          <tr><th>Brand</th><td>${camera.brandName}</td></tr>
          <tr><th>Sensor</th><td>${camera.sensorType}</td></tr>
          <tr><th>Resolution</th><td>${camera.megaPixels} megapixels</td></tr>
          <tr><th>Categories</th><td>${camera.categories.join(", ") || "&mdash;"}</td></tr>
        </table>

        <div class="qty-row">
          <div class="qty-stepper">
            <button id="qty-down" aria-label="Decrease">&minus;</button>
            <input type="number" id="qty" value="1" min="1" max="${camera.stockQuantity || 1}" />
            <button id="qty-up" aria-label="Increase">+</button>
          </div>
          <button class="btn btn-primary" id="add-btn" ${stockOut ? "disabled" : ""}>
            ${stockOut ? "Sold out" : "Add to cart"}
          </button>
        </div>

        <div class="tags">
          ${camera.categories.map(c => `<span class="tag">${c}</span>`).join("")}
        </div>
      </div>
    </div>
  `;

  const qtyInput = document.getElementById("qty");
  document.getElementById("qty-down").addEventListener("click", () => {
    qtyInput.value = Math.max(1, parseInt(qtyInput.value || "1", 10) - 1);
  });
  document.getElementById("qty-up").addEventListener("click", () => {
    const max = camera.stockQuantity || 1;
    qtyInput.value = Math.min(max, parseInt(qtyInput.value || "1", 10) + 1);
  });

  document.getElementById("add-btn").addEventListener("click", async () => {
    if (!Auth.isLoggedIn()) {
      toast("Please sign in to add items to your cart.", "info");
      setTimeout(() => location.href = `auth.html?next=${encodeURIComponent("product.html?id=" + id)}`, 800);
      return;
    }
    const qty = parseInt(qtyInput.value || "1", 10);
    try {
      await API.post("/cart/items", { cameraId: camera.id, quantity: qty });
      toast(`Added ${qty} × ${camera.model} to your cart.`, "success");
      Nav.refreshCartBadge();
    } catch (err) {
      toast(err.message || "Could not add to cart.", "error");
    }
  });
})();
