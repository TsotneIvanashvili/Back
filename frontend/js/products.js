/* Products page — loads cameras, brands, categories, then filters client-side. */

(async function () {
  const grid = document.getElementById("products-grid");
  const countEl = document.getElementById("result-count");

  let allCameras = [];
  const filters = {
    brands: new Set(),
    categories: new Set(),
    minPrice: null,
    maxPrice: null,
    minMp: null,
    inStockOnly: false,
    sort: "newest"
  };

  // Pre-select category from URL (?cat=Mirrorless)
  const urlCat = new URLSearchParams(location.search).get("cat");

  try {
    const [cameras, brands, categories] = await Promise.all([
      API.get("/cameras"),
      API.get("/brands"),
      API.get("/categories")
    ]);
    allCameras = cameras;

    renderChecks("brand-checks", brands, "name", (val, on) => {
      if (on) filters.brands.add(val); else filters.brands.delete(val);
      apply();
    });
    renderChecks("category-checks", categories, "name", (val, on) => {
      if (on) filters.categories.add(val); else filters.categories.delete(val);
      apply();
    }, urlCat ? [urlCat] : []);

    if (urlCat) filters.categories.add(urlCat);

    apply();
  } catch (err) {
    grid.innerHTML = `<div class="empty" style="grid-column:1/-1;">
      <h3>Couldn't load catalog</h3>
      <p>API at <code>${API.BASE}</code> isn't responding.</p>
    </div>`;
    countEl.textContent = "";
  }

  // ---- filter inputs ----
  const bind = (id, ev, handler) =>
    document.getElementById(id).addEventListener(ev, handler);

  bind("price-min", "input", (e) => { filters.minPrice = numberOrNull(e.target.value); apply(); });
  bind("price-max", "input", (e) => { filters.maxPrice = numberOrNull(e.target.value); apply(); });
  bind("mp-min",    "input", (e) => { filters.minMp    = numberOrNull(e.target.value); apply(); });
  bind("instock-only", "change", (e) => { filters.inStockOnly = e.target.checked; apply(); });
  bind("sort-select",  "change", (e) => { filters.sort = e.target.value; apply(); });
  bind("reset-filters", "click", () => {
    filters.brands.clear();
    filters.categories.clear();
    filters.minPrice = filters.maxPrice = filters.minMp = null;
    filters.inStockOnly = false;
    filters.sort = "newest";

    document.querySelectorAll("#brand-checks input, #category-checks input")
      .forEach(i => i.checked = false);
    document.getElementById("price-min").value = "";
    document.getElementById("price-max").value = "";
    document.getElementById("mp-min").value = "";
    document.getElementById("instock-only").checked = false;
    document.getElementById("sort-select").value = "newest";
    apply();
  });

  function apply() {
    let list = allCameras.filter((c) => {
      if (filters.brands.size && !filters.brands.has(c.brandName)) return false;
      if (filters.categories.size && !c.categories.some(cat => filters.categories.has(cat))) return false;
      if (filters.minPrice !== null && c.price < filters.minPrice) return false;
      if (filters.maxPrice !== null && c.price > filters.maxPrice) return false;
      if (filters.minMp !== null && c.megaPixels < filters.minMp) return false;
      if (filters.inStockOnly && c.stockQuantity <= 0) return false;
      return true;
    });

    switch (filters.sort) {
      case "price-asc":  list.sort((a,b) => a.price - b.price); break;
      case "price-desc": list.sort((a,b) => b.price - a.price); break;
      case "name":       list.sort((a,b) => a.model.localeCompare(b.model)); break;
      case "newest":
      default:           list.sort((a,b) => b.id - a.id); break;
    }

    countEl.textContent =
      `${list.length} of ${allCameras.length} ${allCameras.length === 1 ? "camera" : "cameras"}`;

    grid.innerHTML = "";
    if (list.length === 0) {
      grid.innerHTML = `<div class="empty" style="grid-column:1/-1;">
        <h3>No cameras match those filters</h3>
        <p>Try loosening the price range or unchecking a brand.</p>
      </div>`;
      return;
    }
    list.forEach((c) => grid.appendChild(productCard(c)));
  }

  function renderChecks(targetId, items, prop, onChange, preChecked = []) {
    const target = document.getElementById(targetId);
    target.innerHTML = "";
    items.forEach((item) => {
      const id = `${targetId}-${item.id}`;
      const checked = preChecked.includes(item[prop]);
      const wrap = document.createElement("label");
      wrap.innerHTML = `<input type="checkbox" id="${id}" ${checked ? "checked" : ""}/> ${item[prop]}`;
      wrap.querySelector("input").addEventListener("change", (e) => onChange(item[prop], e.target.checked));
      target.appendChild(wrap);
    });
  }

  function numberOrNull(v) {
    const n = parseFloat(v);
    return Number.isFinite(n) ? n : null;
  }
})();
