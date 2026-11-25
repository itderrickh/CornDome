window.CardCreatorConfig = {
    root: '/img/card_generator',
    attackIcon: '/img/card_generator/ui/attack_token.svg', 

    landscapes: [
        { id: 0, value: 'blue_plains', name: 'Blue Plains', file: 'BluePlains' },
        { id: 1, value: 'cornfield', name: 'Cornfield', file: 'Cornfield' },
        { id: 2, value: 'useless_swamp', name: 'Useless Swamp', file: 'UselessSwamp', lightText: true },
        { id: 3, value: 'sandylands', name: 'SandyLands', file: 'SandyLands' },
        { id: 4, value: 'nicelands', name: 'NiceLands', file: 'NiceLands' },
        { id: 5, value: 'icylands', name: 'IcyLands', file: 'IcyLands' },
        { id: 6, value: 'rainbow', name: 'Rainbow', file: 'Rainbow', noLandscapeCard: true },
        { id: 7, value: 'lavaflats', name: 'LavaFlats', file: 'LavaFlats' }
    ],

    lightTextKeys: ['card_ability', 'authors'],

    styleOverrides: {
        card_cost: { fontSize: '38px' }
    },

    compactKeys: ['card_cost', 'card_atk', 'card_def'],
    compactScale: 0.8,

    cardTypes: [
        { id: 0, name: 'Creature', layout: 'creature', frame: '{land}/{file}_Creature.png', exportWidth: 1562,
          fields: ['card_name', 'card_cost', 'card_type', 'card_ability', 'card_atk', 'card_def', 'author', 'authors', 'card_rarity', 'icon'] },
        { id: 1, name: 'Spell', layout: 'spell_building', frame: '{land}/{file}_Card.png', exportWidth: 1562,
          fields: ['card_name', 'card_cost', 'card_type', 'card_ability', 'author', 'authors', 'card_rarity', 'icon'] },
        { id: 2, name: 'Building', layout: 'spell_building', frame: '{land}/{file}_Card.png', exportWidth: 1562,
          fields: ['card_name', 'card_cost', 'card_type', 'card_ability', 'author', 'authors', 'card_rarity', 'icon'] },
        { id: 4, name: 'Hero', layout: 'hero', frame: 'hero/Hero_Frame.png', exportWidth: 1544, noLandscape: true,
          windowFill: { x: 9, y: 42, w: 220, h: 208, color: '#ffffff' },
          fields: ['card_name', 'card_ability', 'author', 'authors', 'card_rarity'] },
        { id: 3, name: 'Landscape', layout: 'landscape', frame: '{land}/{file}_Landscape.png', exportWidth: 2100, landscapeCard: true,
          fields: ['card_name', 'authors', 'card_rarity'] }
    ]
};
