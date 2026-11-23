window.CardCreatorConfig = {
    root: '/img/card_generator',
    attackIcon: '/img/card_generator/ui/attack_token.svg', 

    landscapes: [
        { id: 'blue_plains', name: 'Blue Plains', file: 'BluePlains' },
        { id: 'cornfield', name: 'Cornfield', file: 'Cornfield' },
        { id: 'useless_swamp', name: 'Useless Swamp', file: 'UselessSwamp', lightText: true },
        { id: 'sandylands', name: 'SandyLands', file: 'SandyLands' },
        { id: 'nicelands', name: 'NiceLands', file: 'NiceLands' },
        { id: 'icylands', name: 'IcyLands', file: 'IcyLands' },
        { id: 'rainbow', name: 'Rainbow', file: 'Rainbow', noLandscapeCard: true },
        { id: 'lavaflats', name: 'LavaFlats', file: 'LavaFlats' }
    ],

    lightTextKeys: ['card_ability', 'authors'],

    styleOverrides: {
        card_cost: { fontSize: '38px' }
    },

    compactKeys: ['card_cost', 'card_atk', 'card_def'],
    compactScale: 0.8,

    cardTypes: [
        { id: 'creature', name: 'Creature', layout: 'creature', frame: '{land}/{file}_Creature.png', exportWidth: 1562,
          fields: ['card_name', 'card_cost', 'card_type', 'card_ability', 'card_atk', 'card_def', 'author', 'authors', 'card_rarity', 'icon'] },
        { id: 'spell', name: 'Spell', layout: 'spell_building', frame: '{land}/{file}_Card.png', exportWidth: 1562,
          fields: ['card_name', 'card_cost', 'card_type', 'card_ability', 'author', 'authors', 'card_rarity', 'icon'] },
        { id: 'building', name: 'Building', layout: 'spell_building', frame: '{land}/{file}_Card.png', exportWidth: 1562,
          fields: ['card_name', 'card_cost', 'card_type', 'card_ability', 'author', 'authors', 'card_rarity', 'icon'] },
        { id: 'hero', name: 'Hero', layout: 'hero', frame: 'hero/Hero_Frame.png', exportWidth: 1544, noLandscape: true,
          windowFill: { x: 9, y: 42, w: 220, h: 208, color: '#ffffff' },
          fields: ['card_name', 'card_ability', 'author', 'authors', 'card_rarity'] },
        { id: 'landscape', name: 'Landscape', layout: 'landscape', frame: '{land}/{file}_Landscape.png', exportWidth: 2100, landscapeCard: true,
          fields: ['authors', 'card_rarity'] }
    ]
};
