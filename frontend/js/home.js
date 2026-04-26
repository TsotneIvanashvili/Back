/* Home page logic.
   Includes: featured grid, mosaic showcase, scroll-driven video scrubbing,
   and IntersectionObserver-based caption fade-ins. */

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

  // --- featured grid (3 picks across price tiers) ---
  if (grid) {
    const sorted = [...cameras].sort((a, b) => a.price - b.price);
    const picks = sorted.length >= 3
      ? [sorted[0], sorted[Math.floor(sorted.length / 2)], sorted[sorted.length - 1]]
      : sorted;
    grid.innerHTML = "";
    picks.forEach((c) => grid.appendChild(productCard(c)));
  }

  // --- mosaic grid (asymmetric, image-led) ---
  if (mosaic) {
    // Pick up to 7 cameras; assign tile sizes in a fixed pattern that fills the grid neatly
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

/* ----------------------------------------------------------
   Scroll-driven video scrubbing.
   Tied to a tall container — the video timeline maps 1:1 to
   the user's scroll progress through the section. We lerp the
   currentTime each frame so seeks feel smooth instead of snappy.
---------------------------------------------------------- */
(function initScrollVideo() {
  const section = document.getElementById("scroll-video");
  if (!section) return;

  const video = section.querySelector("video");
  const captions = Array.from(section.querySelectorAll(".caption"));
  if (!video) return;

  let target = 0;     // where we want currentTime to be
  let current = 0;    // where currentTime actually is (lerped)
  let videoDuration = 0;
  let ready = false;
  let lastSeek = 0;

  // Defer until enough metadata loaded
  const onMeta = () => {
    videoDuration = video.duration || 0;
    ready = videoDuration > 0;
    video.pause();
    video.currentTime = 0;
  };
  if (video.readyState >= 1) onMeta();
  else video.addEventListener("loadedmetadata", onMeta);

  // Compute scroll progress through the section [0..1]
  function progress() {
    const rect = section.getBoundingClientRect();
    const scrollable = section.offsetHeight - window.innerHeight;
    const scrolled = -rect.top;
    return Math.max(0, Math.min(1, scrolled / scrollable));
  }

  function updateCaptions(p) {
    captions.forEach((cap) => {
      const at = parseFloat(cap.dataset.at);
      const span = parseFloat(cap.dataset.span || ".18");
      const visible = p >= (at - span) && p <= (at + span);
      cap.classList.toggle("visible", visible);
    });
  }

  // Smooth-seek loop. Reads scroll position every frame so it works whether
  // the page scrolls natively or via Lenis (which suppresses native scroll events).
  function tick() {
    if (ready) {
      const p = progress();
      target = p * videoDuration;
      updateCaptions(p);

      current += (target - current) * 0.15;
      const delta = Math.abs(current - video.currentTime);
      if (delta > 0.04) {
        try { video.currentTime = current; }
        catch (_) { /* browsers occasionally reject seek mid-decode */ }
      }
    }
    requestAnimationFrame(tick);
  }

  requestAnimationFrame(tick);
})();
