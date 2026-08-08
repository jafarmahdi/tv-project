{{flutter_js}}
{{flutter_build_config}}

async function unregisterLegacyServiceWorkers() {
  if (!("serviceWorker" in navigator)) {
    return;
  }

  const registrations = await navigator.serviceWorker.getRegistrations();
  if (registrations.length == 0) {
    return;
  }

  await Promise.all(
    registrations.map((registration) =>
      registration.unregister().catch(() => false),
    ),
  );

  if (!("caches" in window)) {
    return;
  }

  const cacheNames = await caches.keys();
  await Promise.all(
    cacheNames
      .filter((name) => {
        const key = name.toLowerCase();
        return key.includes("flutter") || key.includes("workbox");
      })
      .map((name) => caches.delete(name).catch(() => false)),
  );
}

(async function bootstrap() {
  await unregisterLegacyServiceWorkers();

  _flutter.loader.load({
    config: {
      renderer: "canvaskit",
    },
  });
})();
