

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


/* ----------------------------------------------------------
   Scroll-driven video scrubbing.

   The hard parts of doing this without it feeling rude:

   1) NEVER queue a new seek while video.seeking === true.
      Browsers don't queue currentTime writes — a new seek cancels
      the in-flight one, so during fast scroll only the very last
      seek finishes and you appear to "jump 50 frames".

   2) Only seek when the delta is bigger than ~one frame.
      Seeking 60×/sec for sub-frame deltas thrashes the decoder
      and produces visible chop.

   3) Pre-warm the decoder (play→pause once) so the first scroll
      into the section doesn't have to cold-boot the codec.

   4) Ease the *progress*, not just the seek target. Linear progress
      makes entering and leaving the section feel like a slap.

   5) Stop ticking when the section is off-screen — saves the CPU
      from hammering an invisible element.
---------------------------------------------------------- */
(function initScrollVideo() {
  const section = document.getElementById("scroll-video");
  if (!section) return;
  const video = section.querySelector("video");
  if (!video) return;

  const captions = Array.from(section.querySelectorAll(".caption"));

  // ---- tunables -----------------------------------------------------------
  // Lenis already smooths the wheel input, so we don't need a heavy lerp on
  // top — that would cause sluggish double-smoothing. Just enough to take
  // the edge off the per-frame jitter introduced by seek granularity.
  const SMOOTHING = 0.18;   // lerp factor per frame; lower = silkier, laggier
  const SEEK_EPS  = 0.04;   // seconds; ~1 frame at 30fps. Below this we skip.
  // Soft "ease in/out" — `Sine` curve. Much gentler than cubic, avoids the
  // mid-scroll "rush" while still softening the very first/last few percent.
  const EASE = (t) => 0.5 - Math.cos(Math.PI * t) / 2;
  // -------------------------------------------------------------------------

  let smoothedTime = 0;
  let isInView    = false;
  let videoReady  = false;

  function getProgress() {
    const rect = section.getBoundingClientRect();
    const scrollable = section.offsetHeight - window.innerHeight;
    if (scrollable <= 0) return 0;
    const raw = Math.max(0, Math.min(1, -rect.top / scrollable));
    return EASE(raw);
  }

  function updateCaptions(p) {
    for (const cap of captions) {
      const at   = parseFloat(cap.dataset.at);
      const span = parseFloat(cap.dataset.span || ".18");
      cap.classList.toggle("visible", p >= (at - span) && p <= (at + span));
    }
  }

  // Pre-warm the decoder: kicking off play() and immediately pausing
  // forces the browser to allocate decoder buffers so the first real
  // seek is fast instead of stalling for half a second.
  function prewarm() {
    if (!video.paused) return;
    video.muted = true;
    const p = video.play();
    if (p && typeof p.then === "function") {
      p.then(() => { video.pause(); video.currentTime = 0; }).catch(() => {});
    }
  }

  function onReady() {
    if (videoReady) return;
    videoReady = video.duration > 0;
    if (videoReady) prewarm();
  }

  if (video.readyState >= 2) onReady();
  video.addEventListener("loadedmetadata", onReady);
  video.addEventListener("canplay", onReady);

  // Pause the rAF cost when section isn't visible.
  if ("IntersectionObserver" in window) {
    new IntersectionObserver(
      ([entry]) => { isInView = entry.isIntersecting; },
      { rootMargin: "200px 0px" }       // start a bit before section enters
    ).observe(section);
  } else {
    isInView = true;
  }

  function tick() {
    requestAnimationFrame(tick);

    if (!videoReady || !isInView) return;

    const target = getProgress() * video.duration;
    updateCaptions(getProgress());

    // Exponential damping toward the target — smooths jitter.
    smoothedTime += (target - smoothedTime) * SMOOTHING;

    // CRITICAL: don't issue another seek while the previous one is still
    // in flight. Stacking seeks is what makes the video appear to jump.
    if (video.seeking) return;

    const delta = Math.abs(smoothedTime - video.currentTime);
    if (delta < SEEK_EPS) return;             // skip imperceptible seeks

    // Clamp to the seekable range the browser actually has buffered,
    // so we never request a frame that isn't there yet.
    const seekEnd = video.seekable.length
      ? video.seekable.end(0)
      : video.duration;
    const safe = Math.max(0, Math.min(seekEnd - 0.05, smoothedTime));
    try { video.currentTime = safe; } catch (_) { /* mid-decode race */ }
  }

  requestAnimationFrame(tick);
})();
