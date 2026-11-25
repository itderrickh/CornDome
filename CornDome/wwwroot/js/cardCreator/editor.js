
(function () {
    const cfg = window.CardCreatorConfig;
    const MAX_IMAGE_BYTES = 10 * 1024 * 1024;
    const IMAGE_TYPES = ['image/png', 'image/jpeg', 'image/webp', 'image/svg+xml'];
    const $ = id => document.getElementById(id);
    const layouts = {};

    const renderer = new CardRenderer($('cc-stage'), {
        attackIcon: cfg.attackIcon,
        styleOverrides: cfg.styleOverrides,
        compactKeys: cfg.compactKeys,
        compactScale: cfg.compactScale
    });
    window.cardCreator = { renderer }; 
    let currentLayout = null;

    const typeOf = () => cfg.cardTypes.find(t => t.id === parseInt($('cc-type').value, 10));
    const landOf = () => cfg.landscapes.find(l => l.id === parseInt($('cc-landscape').value, 10));

    function fillTypes() {
        $('cc-type').innerHTML = cfg.cardTypes.map(t => `<option value="${t.id}">${t.name}</option>`).join('');
    }

    function fillLandscapes(type) {
        const keep = $('cc-landscape').value;
        debugger;
        const lands = cfg.landscapes.filter(l => !(type.landscapeCard && l.noLandscapeCard));
        $('cc-landscape').innerHTML = lands.map(l => {
            const soon = type.landscapeCard && l.landscapeCardMissing;
            return `<option value="${l.id}" ${soon ? 'disabled' : ''}>${l.name}${soon ? ' (soon)' : ''}</option>`;
        }).join('');
        const usable = [...$('cc-landscape').options].filter(o => !o.disabled);
        $('cc-landscape').value = usable.some(o => o.value === keep) ? keep : usable[0].value;
    }

    async function getLayout(name) {
    if (!layouts[name]) {
        const url = `${cfg.root}/layouts/${name}.json`;

        console.log('[Card Creator] Loading layout:', url);

        const res = await fetch(url, {
            cache: 'no-store'
        });

        if (!res.ok) {
            throw new Error(`Layout ${name} not found: ${res.status}`);
        }

        layouts[name] = await res.json();

        console.log('[Card Creator] Loaded layout:', name, layouts[name]);
    }

    return layouts[name];
}

    function collectData() {
        const v = id => $(id).value.trim();
        const withAuthor = typeOf().fields.includes('author');
        const authors = [withAuthor && v('cc-author') && `Author: ${v('cc-author')}`, v('cc-artist') && `Artist: ${v('cc-artist')}`].filter(Boolean).join('\n');
        return {
            card_name: v('cc-name'),
            card_cost: v('cc-cost'),
            card_type: `${landOf().name} ${typeOf().name}`,
            card_landscape: landOf().id,
            card_cardtype: typeOf().id,
            card_ability: v('cc-ability'),
            card_author: v('cc-author'),
            card_artist: v('cc-artist'),
            card_atk: v('cc-atk'),
            card_def: v('cc-def'),
            authors
        };
    }

    function textColors() {
        const light = !typeOf().noLandscape && landOf().lightText;
        return Object.fromEntries(light ? cfg.lightTextKeys.map(k => [k, 'white']) : []);
    }

    async function applyTemplate() {
        const type = typeOf();
        fillLandscapes(type);
        const land = landOf();
        if (type.layout !== currentLayout) {
            await renderer.setLayout(await getLayout(type.layout), type.windowFill);
            currentLayout = type.layout;
        }
        await renderer.setFrame(`${cfg.root}/frames/${type.frame.replace('{land}', land.value).replace('{file}', land.file)}`);
        renderer.setVisible(type.fields);
        $('cc-landscape-group').hidden = !!type.noLandscape;
        document.querySelectorAll('[data-field]').forEach(el => {
            el.hidden = !el.dataset.field.split(' ').some(k => type.fields.includes(k));
        });
        renderer.setColors(textColors());
        renderer.setData(collectData());
    }

    function readImage(input, errorEl, onLoad) {
        const file = input.files[0];
        const fail = msg => { errorEl.textContent = msg; errorEl.hidden = !msg; };
        if (!file) return;
        if (!IMAGE_TYPES.includes(file.type)) return fail('Please choose a PNG, JPG WebP or SVG image.');
        if (file.size > MAX_IMAGE_BYTES) return fail('Image is too large (max 10 MB).');
        const img = new Image();
        img.onload = () => { fail(''); onLoad(img); };
        img.onerror = () => fail('This file could not be read as an image.');
        img.src = URL.createObjectURL(file);
    }

    const pxPerUnit = () => typeOf().exportWidth / renderer.size.w;

    function showArtState(st) {
        if (!st) return;
        const k = pxPerUnit(), zoom = Math.round(st.zoom * 100);
        const put = (id, v) => { if (document.activeElement !== $(id)) $(id).value = v; };
        put('cc-art-x', Math.round(st.dx * k));
        put('cc-art-y', Math.round(st.dy * k));
        put('cc-art-zoom', zoom);
        put('cc-art-zoom-range', zoom);
    }

    function applyArtInputs(zoomPercent) {
        const k = pxPerUnit(), num = id => parseFloat($(id).value) || 0;
        renderer.setArtTransform({ dx: num('cc-art-x') / k, dy: num('cc-art-y') / k, zoom: (zoomPercent || 100) / 100 });
    }

    function setArt(img) {
        renderer.setArt(img);
        $('cc-art-remove').hidden = !img;
        $('cc-art-editor').hidden = !img;
        if (!img) { $('cc-art').value = ''; $('cc-art-error').hidden = true; }
    }

    function setIcon(img) {
        renderer.setIcon(img);
        $('cc-icon-remove').hidden = !img;
        if (!img) { $('cc-icon').value = ''; $('cc-icon-error').hidden = true; }
    }

    async function clearAll() {
        if (!confirm('Clear the whole card? Text and uploaded images will be removed.')) return;
        $('cc-form').reset(); 
        setArt(null);
        setIcon(null);
        await applyTemplate();
    }

    function exportPng() {
        const a = document.createElement('a');
        a.href = renderer.toDataURL(typeOf().exportWidth);
        const name = typeOf().id === 3 ? landOf().name : $('cc-name').value.trim();
        a.download = (name || 'card').replace(/[^\w\- ]+/g, '').replace(/\s+/g, '_') + '.png';
        a.click();
    }

    async function saveCustomCard() {
        const button = $("cc-save");

        try {
            button.disabled = true;
            button.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Saving...';

            const card = collectData();

            // Replace "renderer" with the variable holding your CardRenderer instance.
            const image = renderer.toDataURL(1200);
            var visibility = parseInt(document.getElementById("cc-visibility").value)
            debugger;
            const request = {
                name: card.card_name,
                cardType: card.card_cardtype,
                landscape: card.card_landscape,
                ability: card.card_ability,
                cost: card.card_cost ? parseInt(card.card_cost, 10) : null,
                attack: card.card_atk ? parseInt(card.card_atk, 10) : null,
                defense: card.card_def ? parseInt(card.card_def, 10) : null,
                author: card.card_author || null,
                artist: card.card_artist || null,
                image: image,
                visibility: visibility
            };

            const token = document.querySelector(
                'input[name="__RequestVerificationToken"]'
            ).value;

            const response = await fetch("/CardCreator?handler=Save", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": token
                },
                body: JSON.stringify(request)
            });

            const result = await response.json();

            if (!response.ok || !result.success) {
                throw new Error(result.message || "Unable to save your card.");
            }

            alert(result.message);
            window.location.href = "/CardCreator/Mine";
        } catch (error) {
            console.error("Error saving custom card:", error);
            alert(error.message || "There was an error saving your card.");
        } finally {
            button.disabled = false;
            button.innerHTML = '<i class="bi bi-save me-2"></i>Save Card';
        }
    }

    async function init() {
        fillTypes();
        await applyTemplate();
        $('cc-type').addEventListener('change', applyTemplate);
        $('cc-landscape').addEventListener('change', applyTemplate);
        $('cc-form').addEventListener('input', e => {
            if (e.target.type !== 'file' && !e.target.closest('#cc-art-editor')) renderer.setData(collectData());
        });
        $('cc-form').addEventListener('submit', e => e.preventDefault());
        $('cc-art').addEventListener('change', e => readImage(e.target, $('cc-art-error'), setArt));
        $('cc-icon').addEventListener('change', e => readImage(e.target, $('cc-icon-error'), setIcon));
        $('cc-art-remove').addEventListener('click', () => setArt(null));
        $('cc-icon-remove').addEventListener('click', () => setIcon(null));
        renderer.onArtChange = showArtState; // drag / wheel on the card -> numbers
        ['cc-art-x', 'cc-art-y', 'cc-art-zoom'].forEach(id => $(id).addEventListener('input', () => applyArtInputs(parseFloat($('cc-art-zoom').value))));
        $('cc-art-zoom-range').addEventListener('input', e => applyArtInputs(parseFloat(e.target.value)));
        $('cc-art-fit').addEventListener('click', () => renderer.resetArt());
        $('cc-clear').addEventListener('click', clearAll);
        $('cc-export').addEventListener('click', exportPng);

        if ($("cc-save")) {
            $("cc-save").addEventListener("click", saveCustomCard);
        }
        
        $('cc-stage').addEventListener('wheel', e => {
            if (!renderer.art) return;
            e.preventDefault();
            renderer.zoomArt(e.deltaY < 0 ? 1.05 : 1 / 1.05);
        }, { passive: false });
        window.addEventListener('resize', () => renderer.fit());
    }

    init().catch(err => {
        console.error(err);
        $('cc-stage').textContent = 'Card Creator failed to load: ' + err.message;
    });
})();
