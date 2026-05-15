(function () {
    const overlay = document.getElementById("ce-loader-overlay");
    const tire = document.getElementById("ce-loader-tire");
    if (!overlay || !tire) {
        return;
    }

    const prefersReducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)");
    let reduceMotion = prefersReducedMotion.matches;
    const onMotionChange = (event) => {
        reduceMotion = event.matches;
    };

    if (prefersReducedMotion.addEventListener) {
        prefersReducedMotion.addEventListener("change", onMotionChange);
    } else if (prefersReducedMotion.addListener) {
        prefersReducedMotion.addListener(onMotionChange);
    }

    const state = {
        rotation: 0,
        opacity: 1,
        spinning: true,
        fading: false,
        fadeStart: null,
        lastTick: null,
        startedAt: null
    };

    const spinSpeed = 220;
    const fadeDuration = 650;
    const minVisibleMs = 900;
    const easeOutCubic = (t) => 1 - Math.pow(1 - t, 3);
    let fadeTimerId = null;

    const tick = (timestamp) => {
        if (state.lastTick === null) {
            state.lastTick = timestamp;
        }

        if (state.startedAt === null) {
            state.startedAt = timestamp;
        }

        const delta = Math.min(0.05, (timestamp - state.lastTick) / 1000);
        state.lastTick = timestamp;

        if (state.spinning && !reduceMotion) {
            state.rotation = (state.rotation + spinSpeed * delta) % 360;
            tire.style.transform = `rotate(${state.rotation.toFixed(2)}deg)`;
        }

        if (state.fading) {
            if (state.fadeStart === null) {
                state.fadeStart = timestamp;
            }

            const progress = Math.min(1, (timestamp - state.fadeStart) / fadeDuration);
            const eased = easeOutCubic(progress);
            state.opacity = 1 - eased;
            overlay.style.opacity = state.opacity.toFixed(3);

            if (progress >= 1) {
                overlay.style.display = "none";
                overlay.setAttribute("aria-hidden", "true");
                overlay.setAttribute("aria-busy", "false");
                state.spinning = false;
                state.fading = false;
            }
        }

        if (state.spinning || state.fading) {
            requestAnimationFrame(tick);
        }
    };

    requestAnimationFrame(tick);

    const startFadeOut = () => {
        if (!state.fading && overlay.style.display !== "none") {
            state.fading = true;
        }
    };

    const scheduleFadeOut = () => {
        if (fadeTimerId !== null || overlay.style.display === "none") {
            return;
        }

        const now = performance.now();
        const startedAt = state.startedAt ?? now;
        const elapsed = now - startedAt;
        const remaining = Math.max(0, minVisibleMs - elapsed);

        fadeTimerId = window.setTimeout(() => {
            fadeTimerId = null;
            startFadeOut();
        }, remaining);
    };

    const whenWindowLoaded = new Promise((resolve) => {
        if (document.readyState === "complete") {
            resolve();
            return;
        }
        window.addEventListener("load", resolve, { once: true });
    });

    const whenFontsReady = document.fonts && document.fonts.ready ? document.fonts.ready : Promise.resolve();

    Promise.all([whenWindowLoaded, whenFontsReady]).then(scheduleFadeOut);

    window.addEventListener("pageshow", (event) => {
        if (event.persisted) {
            scheduleFadeOut();
        }
    });
})();
