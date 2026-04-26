/* Smooth scroll using Lenis (loaded via CDN <script> in the page).
   Lenis intercepts wheel/touch events and tweens scroll position with rAF,
   which makes scroll-driven animations (like our scroll-video) feel buttery. */

(function () {
  if (typeof Lenis === "undefined") return; // gracefully no-op if CDN failed

  const lenis = new Lenis({
    duration: 1.15,
    easing: (t) => Math.min(1, 1.001 - Math.pow(2, -10 * t)),
    smoothWheel: true,
    smoothTouch: false,         // touch devices: keep native momentum
    wheelMultiplier: 1.0,
    touchMultiplier: 1.5
  });

  function raf(time) {
    lenis.raf(time);
    requestAnimationFrame(raf);
  }
  requestAnimationFrame(raf);

  window.__lenis = lenis;
})();
