/**
 * avatar-crop.js
 * Lightweight canvas-based avatar cropper.
 * No external dependencies — uses only the browser Canvas 2D API.
 *
 * API (window.avatarCrop):
 *   openFromInput(inputId, canvasId) — read file directly from <input> in JS (zero SignalR transfer)
 *   open(canvasId, dataUrl)          — load from data URL (fallback)
 *   setZoom(canvasId, value)         — update zoom from a range input (value 0–100)
 *   export(canvasId)                 — returns Promise<string> base64 JPEG (no prefix)
 *   destroy(canvasId)               — clean up state and revoke object URL
 *
 * KEY DESIGN: openFromInput reads the File object directly in JS using
 * URL.createObjectURL() — zero bytes transferred over SignalR before the
 * user clicks "Guardar foto". This eliminates the "Not Responding" freeze.
 */
window.avatarCrop = (() => {
    // Per-canvas state keyed by canvasId
    const state = {};

    // ── helpers ──────────────────────────────────────────────────────────────

    function getState(id) { return state[id]; }

    function draw(s) {
        const { ctx, img, size, offsetX, offsetY, zoom } = s;
        ctx.clearRect(0, 0, size, size);

        // Clip to circle
        ctx.save();
        ctx.beginPath();
        ctx.arc(size / 2, size / 2, size / 2, 0, Math.PI * 2);
        ctx.clip();

        // Scaled image dimensions
        const scaledW = img.naturalWidth  * zoom;
        const scaledH = img.naturalHeight * zoom;

        // Draw centred at current pan offset
        const x = size / 2 - scaledW / 2 + offsetX;
        const y = size / 2 - scaledH / 2 + offsetY;

        ctx.drawImage(img, x, y, scaledW, scaledH);
        ctx.restore();

        // Circle border
        ctx.beginPath();
        ctx.arc(size / 2, size / 2, size / 2 - 1, 0, Math.PI * 2);
        ctx.strokeStyle = 'rgba(255,255,255,0.5)';
        ctx.lineWidth = 2;
        ctx.stroke();
    }

    function clampOffset(s) {
        const { img, size, zoom } = s;
        const scaledW = img.naturalWidth  * zoom;
        const scaledH = img.naturalHeight * zoom;
        const maxX = Math.max(0, (scaledW - size) / 2);
        const maxY = Math.max(0, (scaledH - size) / 2);
        s.offsetX = Math.max(-maxX, Math.min(maxX, s.offsetX));
        s.offsetY = Math.max(-maxY, Math.min(maxY, s.offsetY));
    }

    function attachEvents(s) {
        const canvas = s.canvas;

        // ── Mouse ─────────────────────────────────────────────────────────────
        canvas.addEventListener('mousedown', e => {
            s.dragging = true;
            s.lastX = e.clientX;
            s.lastY = e.clientY;
            canvas.style.cursor = 'grabbing';
        });
        window.addEventListener('mousemove', e => {
            if (!s.dragging) return;
            s.offsetX += e.clientX - s.lastX;
            s.offsetY += e.clientY - s.lastY;
            s.lastX = e.clientX;
            s.lastY = e.clientY;
            clampOffset(s);
            draw(s);
        });
        window.addEventListener('mouseup', () => {
            s.dragging = false;
            canvas.style.cursor = 'grab';
        });

        // ── Touch ─────────────────────────────────────────────────────────────
        canvas.addEventListener('touchstart', e => {
            if (e.touches.length === 1) {
                s.dragging = true;
                s.lastX = e.touches[0].clientX;
                s.lastY = e.touches[0].clientY;
            }
        }, { passive: true });
        canvas.addEventListener('touchmove', e => {
            if (!s.dragging || e.touches.length !== 1) return;
            s.offsetX += e.touches[0].clientX - s.lastX;
            s.offsetY += e.touches[0].clientY - s.lastY;
            s.lastX = e.touches[0].clientX;
            s.lastY = e.touches[0].clientY;
            clampOffset(s);
            draw(s);
        }, { passive: true });
        canvas.addEventListener('touchend', () => { s.dragging = false; });

        canvas.style.cursor = 'grab';
    }

    function initState(canvasId, img) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return null;
        const size    = canvas.width;
        const ctx     = canvas.getContext('2d');
        const fitZoom = Math.max(size / img.naturalWidth, size / img.naturalHeight);
        const s = {
            canvas, ctx, img, size,
            zoom: fitZoom, baseZoom: fitZoom,
            offsetX: 0, offsetY: 0,
            dragging: false, lastX: 0, lastY: 0,
            objectUrl: null   // set by openFromInput
        };
        state[canvasId] = s;
        attachEvents(s);
        draw(s);
        return s;
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /**
     * PRIMARY ENTRY POINT — reads the file directly from the DOM input element.
     * No base64 encoding, no SignalR data transfer.
     * Returns null on success, or an error string.
     */
    function openFromInput(inputId, canvasId) {
        const input = document.getElementById(inputId);
        if (!input || !input.files || input.files.length === 0) {
            return 'No se encontró el archivo seleccionado.';
        }
        const file = input.files[0];
        const allowed = ['image/jpeg', 'image/png', 'image/webp', 'image/gif'];
        if (!allowed.includes(file.type)) {
            return 'Formato no permitido. Usa JPEG, PNG, WebP o GIF.';
        }
        if (file.size > 5 * 1024 * 1024) {
            return 'El archivo supera el límite de 5 MB.';
        }

        // Clean up any previous object URL for this canvas
        const prev = state[canvasId];
        if (prev && prev.objectUrl) { URL.revokeObjectURL(prev.objectUrl); }

        const objectUrl = URL.createObjectURL(file);
        const img = new Image();
        img.onload = () => {
            const s = initState(canvasId, img);
            if (s) s.objectUrl = objectUrl;
        };
        img.onerror = () => URL.revokeObjectURL(objectUrl);
        img.src = objectUrl;

        return null; // success
    }

    /** Fallback: load from a data URL (e.g. when file bytes are already in memory) */
    function open(canvasId, dataUrl) {
        const img = new Image();
        img.onload = () => initState(canvasId, img);
        img.src = dataUrl;
    }

    function setZoom(canvasId, sliderValue) {
        const s = getState(canvasId);
        if (!s) return;
        const t = parseFloat(sliderValue) / 100;
        s.zoom  = s.baseZoom * (1 + t * 2);   // 1× to 3× the fit zoom
        clampOffset(s);
        draw(s);
    }

    function exportCanvas(canvasId) {
        return new Promise((resolve, reject) => {
            const s = getState(canvasId);
            if (!s) { reject('Canvas not initialised'); return; }
            s.canvas.toBlob(blob => {
                if (!blob) { reject('toBlob failed'); return; }
                const reader = new FileReader();
                reader.onloadend = () => {
                    const b64 = reader.result.split(',')[1];
                    resolve(b64);
                };
                reader.readAsDataURL(blob);
            }, 'image/jpeg', 0.92);
        });
    }

    function destroy(canvasId) {
        const s = state[canvasId];
        if (s && s.objectUrl) { URL.revokeObjectURL(s.objectUrl); }
        delete state[canvasId];
    }

    return { openFromInput, open, setZoom, export: exportCanvas, destroy };
})();
