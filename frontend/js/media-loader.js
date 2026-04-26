(function initMediaSources() {
  const sources = window.MEDIA_SOURCES || {};
  const mediaMap = [
    {
      key: "heroVideo",
      selector: ".hero-video",
      sourceSelector: "source",
      fallback: "8081347-hd_1920_1080_30fps (1).mp4"
    },
    {
      key: "scrollVideo",
      selector: "#scroll-video video",
      fallback: "scroll-video.mp4"
    }
  ];

  mediaMap.forEach(({ key, selector, sourceSelector, fallback }) => {
    const video = document.querySelector(selector);
    if (!video) return;

    const nextSrc = sources[key] || fallback;
    if (!nextSrc) return;

    if (sourceSelector) {
      const source = video.querySelector(sourceSelector);
      if (source) source.src = nextSrc;
    } else {
      video.src = nextSrc;
    }

    video.load();
  });
})();
