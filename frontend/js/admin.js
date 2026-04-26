

(async function () {
  if (!Auth.requireLogin({ adminOnly: true })) return;

  const host = document.getElementById("admin-host");
  const tabs = document.querySelectorAll(".admin-tabs button");

  tabs.forEach(t => t.addEventListener("click", () => {
    tabs.forEach(x => x.classList.remove("active"));
    t.classList.add("active");
    render(t.dataset.tab);
  }));

  let brandsCache = [];
  let categoriesCache = [];

  async function ensureLookups() {
    if (!brandsCache.length)     brandsCache     = await API.get("/brands");
    if (!categoriesCache.length) categoriesCache = await API.get("/categories");
  }

  async function render(tab) {
    host.innerHTML = `<div class="loading-cell"><span class="loader"></span></div>`;
    try {
      if      (tab === "cameras")    await renderCameras();
      else if (tab === "brands")     await renderBrands();
      else if (tab === "categories") await renderCategories();
      else if (tab === "orders")     await renderOrders();
    } catch (err) {
      host.innerHTML = `<div class="empty"><h3>${err.message || "Error"}</h3></div>`;
    }
  }
  async function renderCameras() {
    await ensureLookups();
    const cameras = await API.get("/cameras");
    host.innerHTML = `
      <div class="admin-toolbar">
        <span class="count">${cameras.length} cameras</span>
        <button class="btn btn-primary btn-sm" id="add-camera">+ New camera</button>
      </div>
      <div class="admin-table-wrap">
        <table class="admin-table">
          <thead><tr>
            <th></th><th>Model</th><th>Brand</th><th>Price</th><th>Stock</th><th>Categories</th><th></th>
          </tr></thead>
          <tbody>
            ${cameras.map(c => `
              <tr>
                <td><img class="row-thumb" src="${c.imageUrl}" alt="" /></td>
                <td>${c.model}</td>
                <td>${c.brandName}</td>
                <td>${fmt.money(c.price)}</td>
                <td>${c.stockQuantity}</td>
                <td style="color:var(--text-dim);font-size:.85rem;">${c.categories.join(", ") || "—"}</td>
                <td>
                  <div class="row-actions">
                    <button class="btn btn-ghost btn-sm" data-edit="${c.id}">Edit</button>
                    <button class="btn btn-danger btn-sm" data-del="${c.id}">Delete</button>
                  </div>
                </td>
              </tr>
            `).join("")}
          </tbody>
        </table>
      </div>`;
    document.getElementById("add-camera").addEventListener("click", () => openCameraForm(null));
    host.querySelectorAll("[data-edit]").forEach(b =>
      b.addEventListener("click", () => openCameraForm(parseInt(b.dataset.edit, 10), cameras))
    );
    host.querySelectorAll("[data-del]").forEach(b =>
      b.addEventListener("click", async () => {
        if (!confirm("Delete this camera?")) return;
        try { await API.del(`/cameras/${b.dataset.del}`); toast("Deleted", "success"); renderCameras(); }
        catch (err) { toast(err.message, "error"); }
      })
    );
  }

  function openCameraForm(id, list) {
    const editing = id !== null;
    const c = editing ? list.find(x => x.id === id) : null;
    openModal(editing ? `Edit ${c.model}` : "New camera", `
      <div class="field"><label>Model</label><input id="m-model" value="${c?.model ?? ""}" /></div>
      <div class="field"><label>Description</label><textarea id="m-desc" rows="3">${c?.description ?? ""}</textarea></div>
      <div class="field-row">
        <div class="field"><label>Price (USD)</label><input id="m-price" type="number" step="0.01" value="${c?.price ?? ""}" /></div>
        <div class="field"><label>Stock</label><input id="m-stock" type="number" value="${c?.stockQuantity ?? 0}" /></div>
      </div>
      <div class="field-row">
        <div class="field"><label>Megapixels</label><input id="m-mp" type="number" value="${c?.megaPixels ?? ""}" /></div>
        <div class="field"><label>Sensor</label><input id="m-sensor" value="${c?.sensorType ?? ""}" /></div>
      </div>
      <div class="field"><label>Image URL</label><input id="m-img" value="${c?.imageUrl ?? ""}" /></div>
      <div class="field">
        <label>Brand</label>
        <select id="m-brand" style="width:100%;background:var(--surface);color:var(--text);border:1px solid var(--border);padding:.85rem 1rem;border-radius:var(--radius);font-family:var(--font-body);font-size:.95rem;">
          ${brandsCache.map(b => `<option value="${b.id}" ${b.name === c?.brandName ? "selected" : ""}>${b.name}</option>`).join("")}
        </select>
      </div>
      <div class="field">
        <label>Categories</label>
        <div class="checks" style="max-height:none;">
          ${categoriesCache.map(cat => {
            const checked = c?.categories?.includes(cat.name) ? "checked" : "";
            return `<label><input type="checkbox" value="${cat.id}" ${checked} /> ${cat.name}</label>`;
          }).join("")}
        </div>
      </div>
    `, async () => {
      const body = {
        model: document.getElementById("m-model").value.trim(),
        description: document.getElementById("m-desc").value.trim(),
        price: parseFloat(document.getElementById("m-price").value),
        stockQuantity: parseInt(document.getElementById("m-stock").value, 10),
        megaPixels: parseInt(document.getElementById("m-mp").value, 10),
        sensorType: document.getElementById("m-sensor").value.trim(),
        imageUrl: document.getElementById("m-img").value.trim(),
        brandId: parseInt(document.getElementById("m-brand").value, 10),
        categoryIds: Array.from(document.querySelectorAll("#modal .checks input:checked"))
                          .map(i => parseInt(i.value, 10))
      };
      try {
        if (editing) await API.put(`/cameras/${id}`, body);
        else         await API.post(`/cameras`, body);
        toast("Saved", "success");
        closeModal();
        renderCameras();
      } catch (err) {
        toast(err.message || "Save failed", "error");
      }
    });
  }
  async function renderBrands() {
    const brands = await API.get("/brands");
    brandsCache = brands;
    host.innerHTML = `
      <div class="admin-toolbar">
        <span class="count">${brands.length} brands</span>
        <button class="btn btn-primary btn-sm" id="add-brand">+ New brand</button>
      </div>
      <div class="admin-table-wrap">
        <table class="admin-table">
          <thead><tr><th>Name</th><th>Country</th><th>Founded</th><th></th></tr></thead>
          <tbody>
            ${brands.map(b => `
              <tr>
                <td>${b.name}</td>
                <td style="color:var(--text-dim);">${b.country || "—"}</td>
                <td style="color:var(--text-dim);">${b.foundedYear || "—"}</td>
                <td><div class="row-actions">
                  <button class="btn btn-ghost btn-sm" data-edit="${b.id}">Edit</button>
                  <button class="btn btn-danger btn-sm" data-del="${b.id}">Delete</button>
                </div></td>
              </tr>
            `).join("")}
          </tbody>
        </table>
      </div>`;
    document.getElementById("add-brand").addEventListener("click", () => openBrandForm(null, brands));
    host.querySelectorAll("[data-edit]").forEach(b => b.addEventListener("click", () => openBrandForm(parseInt(b.dataset.edit, 10), brands)));
    host.querySelectorAll("[data-del]").forEach(b => b.addEventListener("click", async () => {
      if (!confirm("Delete this brand?")) return;
      try { await API.del(`/brands/${b.dataset.del}`); toast("Deleted", "success"); renderBrands(); }
      catch (err) { toast(err.message, "error"); }
    }));
  }

  function openBrandForm(id, list) {
    const editing = id !== null;
    const b = editing ? list.find(x => x.id === id) : null;
    openModal(editing ? `Edit ${b.name}` : "New brand", `
      <div class="field"><label>Name</label><input id="b-name" value="${b?.name ?? ""}" /></div>
      <div class="field"><label>Country</label><input id="b-country" value="${b?.country ?? ""}" /></div>
      <div class="field"><label>Founded</label><input id="b-year" type="number" value="${b?.foundedYear ?? ""}" /></div>
    `, async () => {
      const body = {
        name: document.getElementById("b-name").value.trim(),
        country: document.getElementById("b-country").value.trim(),
        foundedYear: parseInt(document.getElementById("b-year").value, 10) || 0
      };
      try {
        if (editing) await API.put(`/brands/${id}`, body);
        else         await API.post(`/brands`, body);
        toast("Saved", "success");
        closeModal();
        renderBrands();
      } catch (err) { toast(err.message, "error"); }
    });
  }
  async function renderCategories() {
    const cats = await API.get("/categories");
    categoriesCache = cats;
    host.innerHTML = `
      <div class="admin-toolbar">
        <span class="count">${cats.length} categories</span>
        <button class="btn btn-primary btn-sm" id="add-cat">+ New category</button>
      </div>
      <div class="admin-table-wrap">
        <table class="admin-table">
          <thead><tr><th>Name</th><th>Description</th><th></th></tr></thead>
          <tbody>
            ${cats.map(c => `
              <tr>
                <td>${c.name}</td>
                <td style="color:var(--text-dim);">${c.description || "—"}</td>
                <td><div class="row-actions">
                  <button class="btn btn-ghost btn-sm" data-edit="${c.id}">Edit</button>
                  <button class="btn btn-danger btn-sm" data-del="${c.id}">Delete</button>
                </div></td>
              </tr>
            `).join("")}
          </tbody>
        </table>
      </div>`;
    document.getElementById("add-cat").addEventListener("click", () => openCatForm(null, cats));
    host.querySelectorAll("[data-edit]").forEach(b => b.addEventListener("click", () => openCatForm(parseInt(b.dataset.edit, 10), cats)));
    host.querySelectorAll("[data-del]").forEach(b => b.addEventListener("click", async () => {
      if (!confirm("Delete this category?")) return;
      try { await API.del(`/categories/${b.dataset.del}`); toast("Deleted", "success"); renderCategories(); }
      catch (err) { toast(err.message, "error"); }
    }));
  }

  function openCatForm(id, list) {
    const editing = id !== null;
    const c = editing ? list.find(x => x.id === id) : null;
    openModal(editing ? `Edit ${c.name}` : "New category", `
      <div class="field"><label>Name</label><input id="c-name" value="${c?.name ?? ""}" /></div>
      <div class="field"><label>Description</label><textarea id="c-desc" rows="3">${c?.description ?? ""}</textarea></div>
    `, async () => {
      const body = {
        name: document.getElementById("c-name").value.trim(),
        description: document.getElementById("c-desc").value.trim()
      };
      try {
        if (editing) await API.put(`/categories/${id}`, body);
        else         await API.post(`/categories`, body);
        toast("Saved", "success");
        closeModal();
        renderCategories();
      } catch (err) { toast(err.message, "error"); }
    });
  }
  async function renderOrders() {
    const orders = await API.get("/orders");
    host.innerHTML = `
      <div class="admin-toolbar"><span class="count">${orders.length} orders</span></div>
      <div class="admin-table-wrap">
        <table class="admin-table">
          <thead><tr><th>#</th><th>Customer</th><th>Date</th><th>Items</th><th>Total</th><th>Status</th></tr></thead>
          <tbody>
            ${orders.map(o => `
              <tr>
                <td>${o.id}</td>
                <td>${o.username}</td>
                <td style="color:var(--text-dim);">${new Date(o.orderDate).toLocaleString()}</td>
                <td>${o.items.length}</td>
                <td>${fmt.money(o.totalAmount)}</td>
                <td><span class="tag">${o.status}</span></td>
              </tr>
            `).join("")}
          </tbody>
        </table>
      </div>`;
  }
  function openModal(title, bodyHtml, onSave) {
    document.getElementById("modal-title").textContent = title;
    document.getElementById("modal-body").innerHTML = bodyHtml;
    document.getElementById("modal-backdrop").classList.add("open");
    const saveBtn = document.getElementById("modal-save");
    const cancelBtn = document.getElementById("modal-cancel");
    saveBtn.onclick = onSave;
    cancelBtn.onclick = closeModal;
  }
  function closeModal() {
    document.getElementById("modal-backdrop").classList.remove("open");
  }

  render("cameras");
})();
