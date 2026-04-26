(function initMediaSources() {
  const sources = window.MEDIA_SOURCES || {};
  const mediaMap = [
    {
      key: "heroVideo",
      selector: ".hero-video",
      fallback: "8081347-hd_1920_1080_30fps (1).mp4"
    },
    {
      key: "scrollVideo",
      selector: "#scroll-video video",
      fallback: "scroll-video.mp4"
    }
  ];

  mediaMap.forEach(({ key, selector, fallback }) => {
    const video = document.querySelector(selector);
    if (!video) return;

    const nextSrc = sources[key] || fallback;
    if (!nextSrc) return;

    video.src = nextSrc;
    video.load();

    if (video.hasAttribute("autoplay")) {
      const tryPlay = () => video.play().catch(() => {});
      if (video.readyState >= 2) tryPlay();
      else video.addEventListener("loadeddata", tryPlay, { once: true });
    }
  });
})();
