(function () {
    Konva._fixTextRendering = true;

    const px = v => parseFloat(v) || 0;
    const htmlToText = h => (h || '').replace(/<br\s*\/?>/gi, '\n').replace(/<\/p>\s*<p>/gi, '\n').replace(/<[^>]+>/g, '').replace(/&quot;/g, '"').replace(/&amp;/g, '&');
    const loadImage = src => new Promise((ok, err) => { const i = new Image(); i.onload = () => ok(i); i.onerror = () => err(new Error('Cannot load ' + src)); i.src = src; });
    const measureCtx = document.createElement('canvas').getContext('2d');

    function hq(node) {
        node.sceneFunc((ctx, shape) => {
            ctx._context.imageSmoothingQuality = 'high';
            shape._sceneFunc(ctx);
        });
        return node;
    }
    const ICON_EM = 1.4; // [ATTACK] icon size relative to the font size

    function flatten(zone, ox = 0, oy = 0, out = []) {
        const s = zone.style || {}, x = ox + px(s.left), y = oy + px(s.top);
        out.push({ zone, x, y, w: px(s.width), h: px(s.height) });
        (zone.childZones || []).forEach(c => flatten(c, x, y, out));
        return out;
    }

    function parseStyles(text) {
        const segs = [];
        let bold = false, italic = false, buf = '';
        const flush = () => { if (buf) segs.push({ text: buf, bold, italic }); buf = ''; };
        for (let i = 0; i < text.length;) {
            if (text.startsWith('**', i)) {
                if (bold || text.indexOf('**', i + 2) !== -1) { flush(); bold = !bold; } else buf += '**'; // unpaired ** is literal
                i += 2;
                continue;
            }
            if (text[i] === '*' && (italic || text.indexOf('*', i + 1) !== -1)) { flush(); italic = !italic; i += 1; continue; }
            buf += text[i++];
        }
        flush();
        return segs;
    }


    function tokenize(text) {
        const out = [];
        for (const { text: seg, bold, italic } of parseStyles(text)) {
            seg.split(/(\[ATTACK\]|\n|[ \t]+)/i).forEach(p => {
                if (!p) return;
                if (/^\[ATTACK\]$/i.test(p)) out.push({ icon: true });
                else if (p === '\n') out.push({ br: true });
                else if (/^[ \t]+$/.test(p)) out.push({ space: true, bold, italic });
                else out.push({ text: p, bold, italic });
            });
        }
        return out;
    }

    const BOLD_STROKE = 0.03; 
    const fontOf = (t, tok, fs) => `${tok.italic ? 'italic' : t.fontStyle} ${fs}px "${t.family}"`;


    function wrapRich(t, tokens, fs) {
        const lines = [{ items: [], w: 0 }];
        let pendingSpace = 0;
        for (const tok of tokens) {
            let line = lines[lines.length - 1];
            if (tok.br) { lines.push({ items: [], w: 0 }); pendingSpace = 0; continue; }
            measureCtx.font = fontOf(t, tok, fs);
            if (tok.space) { if (line.items.length) pendingSpace = measureCtx.measureText(' ').width; continue; }
            const w = tok.icon ? fs * ICON_EM : measureCtx.measureText(tok.text).width + (tok.bold ? fs * BOLD_STROKE : 0);
            if (line.items.length && line.w + pendingSpace + w > t.box.w) {
                line = { items: [], w: 0 };
                lines.push(line);
                pendingSpace = 0;
            }
            line.items.push({ ...tok, x: line.w + pendingSpace, w });
            line.w += pendingSpace + w;
            pendingSpace = 0;
        }
        return lines;
    }

    class CardRenderer {
        constructor(container, opts = {}) {
            this.container = container;
            this.opts = opts;
            this.stage = new Konva.Stage({ container, width: 1, height: 1 });
            this.artLayer = new Konva.Layer();
            this.topLayer = new Konva.Layer();
            this.stage.add(this.artLayer, this.topLayer);
            this.texts = {};
            this.data = {};
            this.colors = {};
            this.visible = null;
        }

        async setLayout(json, artBox = null) {
            const zones = flatten(json.layoutData);
            this.size = { w: zones[0].w, h: zones[0].h };
            this.artLayer.destroyChildren();
            this.topLayer.destroyChildren();
            this.texts = {};
            this.art = null;
            this.icon = null;
            this.iconBox = null;
            this.iconGroup = null;

            const fonts = new Set();
            for (const z of zones.slice(1)) {
                const key = z.zone.imageDataKey;
                if (key === 'card_art') this._buildArt({ ...z, ...artBox });
                else if (key === 'card_frame') {
                    this.frame = hq(new Konva.Image({ x: z.x, y: z.y, width: z.w, height: z.h, listening: false }));
                    this.topLayer.add(this.frame);
                } else if (key === 'icon') this._buildIcon(z);
                else if (z.zone.type === 'text' && z.zone.textDataKey) {
                    const s = { ...z.zone.style, ...(this.opts.styleOverrides || {})[z.zone.textDataKey] };
                    this._buildText(z, s);
                    fonts.add(`20px ${(s.fontFamily || '').replace(/'/g, '')}`);
                }
            }
            const [attack] = await Promise.all([
                this.attackImg || (this.opts.attackIcon ? loadImage(this.opts.attackIcon) : null),
                ...[...fonts].map(f => document.fonts.load(f))
            ]);
            await document.fonts.ready;
            this.attackImg = attack;
            if (this.iconImg) this.setIcon(this.iconImg);
            if (this.artImg) this.setArt(this.artImg, true);
            this.fit();
            this._refreshTexts();
        }

        async setFrame(url) {
            const token = this._frameToken = (this._frameToken || 0) + 1;
            const img = await loadImage(url);
            if (token === this._frameToken) this.frame.image(img); // a newer frame was requested meanwhile
        }

        _buildArt(z) {
            this.artBox = z;
            const g = new Konva.Group({ clip: { x: z.x, y: z.y, width: z.w, height: z.h } });
            this.artPlaceholder = new Konva.Rect({ x: z.x, y: z.y, width: z.w, height: z.h, fill: '#d9d9d9' }); // shown until art is uploaded
            g.add(this.artPlaceholder);
            this.artGroup = g;
            this.artLayer.add(g);
        }

        _buildIcon(z) {
            this.iconBox = z;
            this.iconGroup = new Konva.Group({ clip: { x: z.x, y: z.y, width: z.w, height: z.h }, listening: false });
            this.topLayer.add(this.iconGroup);
        }

        _buildText(z, s) {
            const fs = px(s.fontSize), lh = px(s.lineHeight) || 1.1;
            const pl = px(s.paddingLeft), pr = px(s.paddingRight), pt = px(s.paddingTop), pb = px(s.paddingBottom);
            const inset = 2;
            const box = { x: z.x + pl + inset, y: z.y + pt + inset, w: z.w - pl - pr - 2 * inset, h: z.h - pt - pb - 2 * inset };
            const limit = { w: box.w + 2 * inset, h: box.h + 2 * inset };
            const g = new Konva.Group({ listening: false });
            if (s.background) g.add(new Konva.Rect({ x: z.x, y: z.y, width: z.w, height: z.h, fill: s.background, cornerRadius: px(s.borderRadius) }));
            const t = {
                key: z.zone.textDataKey, group: g, box, limit, baseSize: fs, lh,
                family: (s.fontFamily || 'sans-serif').replace(/'/g, ''),
                fontStyle: [s.fontStyle, s.fontWeight].filter(Boolean).join(' ') || 'normal',
                align: s.textAlign || 'center',
                vAlign: s.alignItems === 'flex-start' ? 'top' : 'middle',
                singleLine: s.alignItems !== 'flex-start' && limit.h < 2 * fs * lh, 
                lsEm: (s.letterSpacing || '').endsWith('em') ? px(s.letterSpacing) : px(s.letterSpacing) / fs, 
                defaultText: htmlToText(z.zone.text)
            };
            if (t.singleLine) {
                t.node = new Konva.Text({
                    x: box.x, y: box.y, width: box.w, height: box.h,
                    fontFamily: t.family, fontSize: fs, fontStyle: t.fontStyle, lineHeight: lh,
                    align: t.align, verticalAlign: t.vAlign, wrap: 'none', fill: 'black'
                });
            } else {
                t.node = new Konva.Shape({ sceneFunc: ctx => this._drawRich(ctx, t), listening: false });
            }
            g.add(t.node);
            this.topLayer.add(g);
            this.texts[t.key] = t;
        }

        setVisible(keys) {
            this.visible = keys;
            if (this.iconGroup) this.iconGroup.visible(!keys || keys.includes('icon'));
            this._refreshTexts();
        }

        setData(data) { this.data = data; this._refreshTexts(); }
        setColors(colors) { this.colors = colors; this._refreshTexts(); }

        _refreshTexts() {
            for (const [key, t] of Object.entries(this.texts)) {
                const show = !this.visible || this.visible.includes(key);
                t.group.visible(show);
                if (!show) continue;
                t.text = key in this.data ? String(this.data[key] ?? '') : t.defaultText;
                t.color = this.colors[key] || 'black';
                let size = t.baseSize;
                if ((this.opts.compactKeys || []).includes(key) && t.text.length >= 2) size *= this.opts.compactScale || 1;
                if (t.singleLine) this._fitSingle(t, size); else this._fitRich(t, size);
            }
            this.topLayer.batchDraw();
        }

        _fitSingle(t, size) {
            const n = t.node;
            n.text(t.text);
            n.fill(t.color);
            n.fontSize(size);
            n.letterSpacing(t.lsEm * size);
            while (n.measureSize(t.text).width > t.limit.w && n.fontSize() > size * 0.35) {
                n.fontSize(n.fontSize() * 0.95);
                n.letterSpacing(t.lsEm * n.fontSize());
            }
            const extra = t.limit.w - t.box.w;
            n.width(t.limit.w);
            n.x(t.align === 'center' ? t.box.x - extra / 2 - n.letterSpacing() / 2 : t.align === 'right' ? t.box.x - extra : t.box.x);
        }

        _fitRich(t, size) {
            t.tokens = tokenize(t.text);
            let fs = size, lines;
            for (; ;) {
                lines = wrapRich(t, t.tokens, fs);
                const tooBig = lines.length * fs * t.lh > t.limit.h || lines.some(l => l.w > t.limit.w);
                if (!tooBig || fs <= size * 0.6) break;
                fs *= 0.95;
            }
            t.fs = fs;
            t.lines = lines;
        }

        _drawRich(konvaCtx, t) {
            if (!t.lines) return;
            const c = konvaCtx._context, fs = t.fs, L = fs * t.lh, box = t.box;
            measureCtx.font = fontOf(t, {}, fs);
            const m = measureCtx.measureText('M');
            const baseline = L / 2 + (m.fontBoundingBoxAscent - m.fontBoundingBoxDescent) / 2; 
            const alignY = t.vAlign === 'middle' ? (box.h - t.lines.length * L) / 2 : 0;
            c.save();
            c.fillStyle = t.color;
            c.strokeStyle = t.color;
            c.lineWidth = fs * BOLD_STROKE;
            c.lineJoin = 'round';
            c.textBaseline = 'alphabetic';
            t.lines.forEach((line, i) => {
                const x0 = box.x + (t.align === 'left' ? 0 : t.align === 'right' ? box.w - line.w : (box.w - line.w) / 2);
                const y = box.y + alignY + i * L + baseline;
                for (const it of line.items) {
                    if (it.icon) {
                        if (this.attackImg) {
                            c.imageSmoothingEnabled = true;
                            c.imageSmoothingQuality = 'high';

                            c.drawImage(
                                this.attackImg,
                                x0 + it.x,
                                y - fs * 0.35 - it.w / 2,
                                it.w,
                                it.w
                            );
                        }
                    } else {
                        c.font = fontOf(t, it, fs);
                        const x = x0 + it.x + (it.bold ? fs * BOLD_STROKE / 2 : 0); 
                        if (it.bold) c.strokeText(it.text, x, y);
                        c.fillText(it.text, x, y);
                    }
                }
            });
            c.restore();
        }

        setIcon(img) {
            this.iconImg = img;
            if (!this.iconBox) return;
            if (this.icon) { this.icon.destroy(); this.icon = null; }
            if (img) {
                const b = this.iconBox, s = Math.min(b.w / img.width, b.h / img.height);
                this.icon = hq(new Konva.Image({
                    image: img, listening: false, width: img.width * s, height: img.height * s,
                    x: b.x + (b.w - img.width * s) / 2, y: b.y + (b.h - img.height * s) / 2
                }));
                this.iconGroup.add(this.icon);
            }
            this.topLayer.batchDraw();
        }

        setArt(img, keepTransform = false) {
            this.artImg = img;
            if (this.art) { this.art.destroy(); this.art = null; }
            this.artPlaceholder.visible(!img);
            if (!img) { this.artState = null; this.artLayer.batchDraw(); return; }
            this.art = hq(new Konva.Image({ image: img, draggable: true })); 
            this.art.on('dragmove', () => {
                const b = this.artBox, a = this.art;
                this.artState.dx = a.x() + a.width() * a.scaleX() / 2 - (b.x + b.w / 2);
                this.artState.dy = a.y() + a.height() * a.scaleY() / 2 - (b.y + b.h / 2);
                this.onArtChange?.(this.artState);
            });
            this.artGroup.add(this.art);
            this.setArtTransform(keepTransform && this.artState ? this.artState : { dx: 0, dy: 0, zoom: 1 });
        }

        setArtTransform({ dx, dy, zoom }) {
            if (!this.art) return;
            const b = this.artBox, img = this.art.image();
            zoom = Math.min(100, Math.max(0.1, zoom || 1));
            const s = Math.max(b.w / img.width, b.h / img.height) * zoom;
            this.artState = { dx, dy, zoom };
            this.art.scale({ x: s, y: s });
            this.art.position({ x: b.x + b.w / 2 + dx - img.width * s / 2, y: b.y + b.h / 2 + dy - img.height * s / 2 });
            this.artLayer.batchDraw();
            this.onArtChange?.(this.artState);
        }

        resetArt() { this.setArtTransform({ dx: 0, dy: 0, zoom: 1 }); }

        zoomArt(factor) {
            if (!this.art) return;
            const st = this.artState, zoom = Math.min(100, Math.max(0.1, st.zoom * factor)), k = zoom / st.zoom;
            this.setArtTransform({ dx: st.dx * k, dy: st.dy * k, zoom });
        }

        fit() {
            if (!this.size) return;
            const w = this.container.clientWidth || 390, scale = w / this.size.w;
            this.stage.size({ width: w, height: this.size.h * scale });
            this.stage.scale({ x: scale, y: scale });
        }

        toDataURL(exportWidth) {
            return this.stage.toDataURL({ pixelRatio: exportWidth / this.stage.width(), mimeType: 'image/png' });
        }
    }

    window.CardRenderer = CardRenderer;
    window.CardRenderer.tokenize = tokenize; 

    const show = x => x.icon ? '@' : x.br ? '|' : x.space ? '_' : (x.italic ? `/${x.bold ? x.text.toUpperCase() : x.text}/` : x.bold ? x.text.toUpperCase() : x.text);
    [
        ['a **b c** [ATTACK] d**', 'a_B_C_@_d**'],
        ['*i* ***bi*** 2*3', '/i/_/BI/_2*3'],
        ['x **y', 'x_**y']
    ].forEach(([input, want]) => {
        const got = tokenize(input).map(show).join('');
        console.assert(got === want, `CardRenderer.tokenize self-check failed: ${input} -> ${got}, want ${want}`);
    });
})();
