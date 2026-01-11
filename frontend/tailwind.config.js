/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      // === ORGANIC COLOR PALETTE ===
      colors: {
        // Primary - Clay/Terracotta
        clay: {
          DEFAULT: '#C77A5C',
          light: '#D99479',
          dark: '#A85E42',
        },

        // Secondary - Sage/Olive
        sage: {
          DEFAULT: '#8B9B7E',
          light: '#A4B394',
          dark: '#6F7D64',
        },

        // Backgrounds - Warm Neutrals
        sand: '#F5EFE7',
        linen: '#FAF7F2',
        parchment: '#EADFD4',

        // Text - Warm Darks
        charcoal: '#3A3531',
        stone: '#6B6258',
        dust: '#9B9187',

        // Accents - Natural Highlights
        honey: '#D4A574',
        moss: '#6B7A5F',

        // Legacy names (backward compatibility - map to organic colors)
        primary: {
          DEFAULT: '#3A3531',  // charcoal
          light: '#6B6258',    // stone
        },
        secondary: {
          DEFAULT: '#6B6258',  // stone
          light: '#9B9187',    // dust
        },
        accent: {
          DEFAULT: '#C77A5C',  // clay
          light: '#D99479',    // clay-light
          dark: '#A85E42',     // clay-dark
        },
        surface: {
          DEFAULT: '#F5EFE7',  // sand
          card: '#FAF7F2',     // linen
        },
      },

      // === ORGANIC SHADOWS (warm brown tints) ===
      boxShadow: {
        'card': '0 1px 2px 0 rgba(58, 53, 49, 0.04), 0 1px 3px 0 rgba(58, 53, 49, 0.06)',
        'card-hover': '0 8px 24px 0 rgba(58, 53, 49, 0.12), 0 4px 12px 0 rgba(58, 53, 49, 0.10)',
        'elegant': '0 4px 16px 0 rgba(58, 53, 49, 0.10), 0 2px 6px 0 rgba(58, 53, 49, 0.08)',
        'soft': '0 2px 8px 0 rgba(58, 53, 49, 0.08), 0 1px 4px 0 rgba(58, 53, 49, 0.06)',
      },

      // === SOFT BORDER RADIUS ===
      borderRadius: {
        'sm': '8px',
        'md': '12px',
        'lg': '20px',
        'xl': '28px',
        '2xl': '32px',
      },

      // === TYPOGRAPHY ===
      fontFamily: {
        heading: ['Crimson Pro', 'Georgia', 'serif'],
        body: ['Manrope', '-apple-system', 'BlinkMacSystemFont', 'sans-serif'],
      },

      // === TRANSITIONS (organic, slower) ===
      transitionDuration: {
        'elegant': '350ms',
      },

      transitionTimingFunction: {
        'elegant': 'cubic-bezier(0.4, 0, 0.2, 1)',
      },
    },
  },
  plugins: [],
}
