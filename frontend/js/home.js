

(async function () {
  const grid = document.getElementById("featured-grid");
  const mosaic = document.getElementById("mosaic-grid");

  let cameras = [];
  try {
    cameras = await API.get("/cameras");
  } catch (err) {
    if (grid) grid.innerHTML = `
      <div class="empty" style="grid-column:1/-1;">
        <h3 style="margin-bottom:.5rem;">Catalog unavailable</h3>
        <p>The API at <code>${API.BASE}</code> isn't responding.</p>
      </div>`;
    return;
  }
  if (grid) {
    const sorted = [...cameras].sort((a, b) => a.price - b.price);
    const picks = sorted.length >= 3
      ? [sorted[0], sorted[Math.floor(sorted.length / 2)], sorted[sorted.length - 1]]
      : sorted;
    grid.innerHTML = "";
    picks.forEach((c) => grid.appendChild(productCard(c)));
  }
  if (mosaic) {
    const selected = cameras.slice(0, 7);
    const sizes = ["feat", "tall", "wide", "square", "huge", "wide", "square"];
    mosaic.innerHTML = "";
    selected.forEach((c, i) => mosaic.appendChild(mosaicTile(c, sizes[i] || "square")));
  }
})();

function productCard(c) {
  const a = document.createElement("a");
  a.href = `product.html?id=${c.id}`;
  a.className = "product-card";
  const stockClass = c.stockQuantity === 0 ? "out"
                    : c.stockQuantity <= 3 ? "low" : "in";
  const stockText  = c.stockQuantity === 0 ? "Sold out"
                    : c.stockQuantity <= 3 ? `Only ${c.stockQuantity} left`
                    : "In stock";
  a.innerHTML = `
    <div class="img-wrap"><img src="${c.imageUrl}" alt="${c.model}" loading="lazy"/></div>
    <div class="body">
      <div class="brand-line">${c.brandName}</div>
      <h3>${c.model}</h3>
      <div class="meta">${c.megaPixels} MP &middot; ${c.sensorType}</div>
      <div class="price-row">
        <span class="price">${fmt.money(c.price)}</span>
        <span class="stock ${stockClass}">${stockText}</span>
      </div>
    </div>`;
  return a;
}
window.productCard = productCard;

function mosaicTile(c, size) {
  const a = document.createElement("a");
  a.href = `product.html?id=${c.id}`;
  a.className = `mosaic-tile ${size}`;
  a.innerHTML = `
    <img src="${c.imageUrl}" alt="${c.model}" loading="lazy" />
    <div class="scrim"></div>
    <div class="label">
      <div class="brand-line">${c.brandName}</div>
      <h3 class="name">${c.model}</h3>
      <div class="price">${fmt.money(c.price)}</div>
    </div>`;
  return a;
}


(function initScrollVideo() {
  const section = document.getElementById("scroll-video");
  if (!section) return;

  const video = section.querySelector("video");
  const captions = Array.from(section.querySelectorAll(".caption"));
  if (!video) return;

  const TRIM_END_SECONDS = 1;
  let maxProgress = 1;
  let target = 0;
  let current = 0;
  let smoothedProgress = 0;
  let videoDuration = 0;
  let ready = false;
  const onMeta = () => {
    const fullDuration = video.duration || 0;
    videoDuration = Math.max(0, fullDuration - TRIM_END_SECONDS);
    maxProgress = fullDuration > 0 ? videoDuration / fullDuration : 1;
    ready = videoDuration > 0;
    video.pause();
    video.currentTime = 0;
  };
  if (video.readyState >= 1) onMeta();
  else video.addEventListener("loadedmetadata", onMeta);
  function progress() {
    const rect = section.getBoundingClientRect();
    const scrollable = section.offsetHeight - window.innerHeight;
    const scrolled = -rect.top;
    return Math.max(0, Math.min(1, scrolled / scrollable));
  }

  function easeInOutCubic(t) {
    return t < 0.5
      ? 4 * t * t * t
      : 1 - Math.pow(-2 * t + 2, 3) / 2;
  }

  function updateCaptions(p) {
    captions.forEach((cap) => {
      const at = parseFloat(cap.dataset.at);
      const span = parseFloat(cap.dataset.span || ".18");
      const visible = p >= (at - span) && p <= (at + span);
      cap.classList.toggle("visible", visible);
    });
  }
  function tick() {
    if (ready) {
      const rawProgress = Math.min(progress(), maxProgress);
      smoothedProgress += (rawProgress - smoothedProgress) * 0.08;

      const easedProgress = easeInOutCubic(smoothedProgress);
      target = easedProgress * videoDuration;
      updateCaptions(smoothedProgress);

      current += (target - current) * 0.08;
      const delta = Math.abs(current - video.currentTime);
      if (delta > 0.02) {
        try { video.currentTime = current; }
        catch (_) {  }
      }
    }
    requestAnimationFrame(tick);
  }

  requestAnimationFrame(tick);
})();
